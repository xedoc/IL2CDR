using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace IL2CDR.Model
{
	public class RconConnection : NotifyPropertyChangeBase, IStopStart
	{
		private readonly object lockConnection = new object();
		private TcpClient connection;
		private NetworkStream netStream;
		//private bool isStopped;
		private readonly object cmdLock = new object();

		private readonly Dictionary<string, string> errorCodes = new Dictionary<string, string>() {
			{"1", "OK"},
			{"2", "Unknown result"},
			{"3", "Unknown command"},
			{"4", "Incorrect parameters count"},
			{"5", "Receive buffer error"},
			{"6", "Access denied! Authenticate first!"},
			{"7", "Server is not running"},
			{"8", "This user isn't allowed to execute a command"},
			{"9", "Invalid user"}
		};


		/// <summary>
		/// This property answers the question, whether this component is running or not.
		/// </summary>
		public bool IsRunning { get; private set; }


		public RconConnection(IL2StartupConfig config)
		{
			this.Config = config;
		}

		/// <summary>
		/// The <see cref="LastErrorDescription" /> property's name.
		/// </summary>
		public const string LAST_ERROR_DESCRIPTION_PROPERTY_NAME = "LastErrorDescription";

		private string lastErrorDescription;

		/// <summary>
		/// Sets and gets the LastErrorDescription property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public string LastErrorDescription
		{
			get => this.lastErrorDescription;

			set
			{
				if (this.lastErrorDescription == value) {
					return;
				}

				this.lastErrorDescription = value;
				this.RaisePropertyChanged(LAST_ERROR_DESCRIPTION_PROPERTY_NAME);
			}
		}


		private IL2StartupConfig _config = null;

		/// <summary>
		/// Sets and gets the Config property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public IL2StartupConfig Config
		{
			get => this._config;

			set
			{
				if (this._config == value) {
					return;
				}

				this._config = value;
				this.RaisePropertyChanged(nameof(this.Config));
			}
		}


		private bool _isConnected = false;

		/// <summary>
		/// Sets and gets the IsConnected property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public bool IsConnected
		{
			get => this._isConnected;

			set
			{
				if (this._isConnected == value) {
					return;
				}

				this._isConnected = value;
				this.RaisePropertyChanged(nameof(this.IsConnected));
			}
		}



		private bool _isAuthorized = false;

		/// <summary>
		/// Sets and gets the IsAuthorized property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public bool IsAuthorized
		{
			get => this._isAuthorized;

			set
			{
				if (this._isAuthorized == value) {
					return;
				}

				this._isAuthorized = value;
				this.RaisePropertyChanged(nameof(this.IsAuthorized));
			}
		}

		private void Connect()
		{
			if (this.Config.RconIP != null && this.Config.RconPort >= 1 || this.Config.RconPort <= 65535) {
				Task.Factory.StartNew(() => {
					Util.Try(() => {
						this.connection = new TcpClient(this.Config.RconIP.ToString(), this.Config.RconPort);
						this.netStream = this.connection.GetStream();
					}, false);
					this.IsConnected = true;
				}).Wait(2000);
			}
		}

		private void Disconnect()
		{
			if (this.connection != null) {
				Util.Try(() => this.netStream.Close());
				Util.Try(() => this.connection.Close());
			}
		}

		/// <summary>
		/// Send raw command to server
		/// </summary>
		/// <param name="line">command</param>
		/// <returns>NameValueCollection with parameter name/value pairs</returns>
		public NameValueCollection RawCommand(string line)
		{
			this.LastErrorDescription = "Communication error";

			if (!this.IsRunning) {
				return new NameValueCollection();
			}

			if (this.netStream == null || !this.netStream.CanWrite) {
				this.Start();
			}

			lock (this.lockConnection) {
				if (this.netStream.CanWrite) {
					var sendBytes = Encoding.UTF8.GetBytes(line);
					var length = BitConverter.GetBytes((ushort) (line.Length + 1));
					byte[] zero = {0};
					var packet = length.Concat(sendBytes).Concat(zero).ToArray();
					Util.Try(() => this.netStream.Write(packet, 0, packet.Length), false);
				} else {
					this.Disconnect();
					return new NameValueCollection();
				}
			}

			var writeTask = Task.Factory.StartNew((obj) => {
				try {
					lock (this.lockConnection) {
						if (!(obj is RconConnection rcon) ||
							rcon.connection == null ||
							rcon.netStream == null) {
							return new NameValueCollection();
						}

						if (rcon.netStream.CanRead) {
							while (!rcon.netStream.DataAvailable) {
								Thread.Sleep(1);
							}

							if (rcon.connection.ReceiveBufferSize <= 0) {
								return new NameValueCollection();
							}

							var bytes = new byte[rcon.connection.ReceiveBufferSize];
							Util.Try(() => rcon.netStream.Read(bytes, 0, rcon.connection.ReceiveBufferSize),
								false);
							var length = BitConverter.ToUInt16(bytes.Take(2).ToArray(), 0);
							string response = null;
							if (length > 2) {
								response = Encoding.UTF8.GetString(bytes.Skip(2).Take(length - 1).ToArray());
							}

							if (!string.IsNullOrWhiteSpace(response)) {
								var result = HttpUtility.ParseQueryString(response);
								this.LastErrorDescription = string.Empty;

								if (this.errorCodes.TryGetValue(result["STATUS"], out var errorText)) {
									this.LastErrorDescription = errorText;
								}

								return result;
							} else {
								return new NameValueCollection();
							}
						} else {
							rcon.Disconnect();
							rcon.Start();
						}

						return new NameValueCollection();
					}
				} catch (Exception e) {
					Log.WriteError("{0}\n{1}", e.Message, e.StackTrace);
					return new NameValueCollection();
				}
			}, this);

			if (writeTask.Wait(3000)) {
				return writeTask.Result;
			} else {
				return new NameValueCollection();
			}
		}

		/// <summary>
		/// Authorizes user on rcon server with user/password taken from the startup.cfg
		/// </summary>
		private void Authenticate()
		{
			var result = this.RawCommand($"auth {this.Config.Login} {this.Config.Password}");
			if (result != null && result.Count > 0) {
				Log.WriteInfo("Rcon authentication: {0}", this.GetResult(result["STATUS"]));
			} else {
				Log.WriteInfo("Rcon authentication failed!");
			}
		}

		private string GetResult(string result)
		{
			this.errorCodes.TryGetValue(result, out var code);
			return code ?? "UNKNOWN";
		}

		/// <summary>
		/// Get server console text
		/// </summary>
		/// <returns>Console text</returns>
		public string GetConsole()
		{
			return this.RawCommand("getconsole")["console"];
		}

		/// <summary>
		/// Check if rcon user is authenticated
		/// </summary>
		/// <returns>true if authenticated, false if not</returns>
		public bool AuthenticationStatus()
		{
			if (this.RawCommand("mystatus")["authed"] == "1") {
				return true;
			}

			return false;
		}

		/// <summary>
		/// Request player list 
		/// </summary>
		/// <returns>List of Player objects</returns>
		public List<Player> GetPlayerList()
		{
			lock (this.cmdLock) {
				var result = new List<Player>();
				var rconResult = this.RawCommand("getplayerlist");
				if (rconResult["STATUS"] != "1") {
					return result;
				}

				var playerList = rconResult["playerList"];
				if (string.IsNullOrEmpty(playerList)) {
					return result;
				}

				var table = playerList.Split('|').Skip(1).ToArray();
				if (table.Length > 0) {
					result = new List<Player>(
						table.Select(line => line.Split(','))
							.Select(x => {
									if (x.Length == 6 &&
										int.TryParse(x[0], out var cid) &&
										Guid.TryParse(HttpUtility.UrlDecode(x[5]), out var nickGuid) &&
										Guid.TryParse(HttpUtility.UrlDecode(x[4]), out var userGuid) &&
										int.TryParse(x[2], out var ping) &&
										Enum.TryParse<PlayerStatus>(x[1], out var status)) {
										return new Player() {
											ClientId = cid,
											IsOnline = true,
											LoginId = userGuid,
											NickId = nickGuid,
											NickName = x[3],
											Ping = ping,
											Status = status,
										};
									} else {
										return null;
									}
								}
							)
					);
				}

				return result;
			}
		}

		/// <summary>
		/// Check server status
		/// </summary>
		/// <returns>true if server alive</returns>
		public bool GetServerStatus()
		{
			return this.RawCommand("serverstatus")["STATUS"] == "1";
		}

		/// <summary>
		/// Kick player
		/// </summary>
		/// <param name="id">nickname, client id or nick/user guid</param>
		public void Kick(object id)
		{
			if (id is string || id is Guid || id is int) {
				this.RawCommand($"kick {id}");
			}
		}

		/// <summary>
		/// Ban player
		/// </summary>
		/// <param name="id">nickname, client id or nick/user guid</param>
		public void Ban(object id)
		{
			if (id is string || id is Guid || id is int) {
				this.RawCommand($"ban {id}");
			}
		}

		/// <summary>
		/// Unban all players
		/// </summary>
		public void UnBanAll()
		{
			this.RawCommand("unbanall");
		}

		/// <summary>
		/// Call named action in mision
		/// </summary>
		/// <param name="name">name of action</param>
		public void ServerInput(string name)
		{
			this.RawCommand($"serverinput {name}");
		}

		/// <summary>
		/// Send server stats immediately
		/// </summary>
		public void SendStats()
		{
			this.RawCommand("sendstatnow");
		}

		/// <summary>
		/// Write chat log to file immediately
		/// </summary>
		public void FlushChatLog()
		{
			this.RawCommand("cutchatlog");
		}

		/// <summary>
		/// Send chat message
		/// </summary>
		/// <param name="roomType">where to send a message: all, player, coalition or country</param>
		/// <param name="text">message text</param>
		/// <param name="id">optional. Client id, country id, coalition id</param>
		public void ChatMessage(RoomType roomType, string text, object id = null)
		{
			if (id == null) {
				id = -1;
			}

			this.RawCommand($"chatmsg {(int) roomType} {id} {text}");
		}

		public void Start()
		{
			if (this.IsRunning) {   // <-- "Idempotent check" -- not to start this service multiple times. 
				return;
			}

			this.Connect();
			this.Authenticate();
			this.IsRunning = true;
		}

		public void Stop()
		{
			if (!this.IsRunning) {		// <-- "Idempotent check" -- not to stop this service multiple times. 
				return; 
			}

			this.IsRunning = false;
			this.Disconnect();
			//this.isStopped = true;
		}

		public void Restart()
		{
			this.Stop();
			this.Start();
		}
	}

	public enum RoomType
	{
		All = 0,
		Client = 1,
		Coalition = 2,
		Country = 3
	}
}