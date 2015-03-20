using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace IL2CDR.Model
{
    public class RconConnection : NotifyPropertyChangeBase, IStopStart
    {
        private object lockConnection = new object();
        private TcpClient connection;
        private NetworkStream netStream;
        private bool isStopped = false;
        private Dictionary<string, string> errorCodes = new Dictionary<string, string>()
        {
            {"1", "OK"},
            {"2", "Unknown result"},
            {"3", "Unknown command"},
            {"4", "Incorrect parameters count"},
            {"5", "Receive buffer error"},
            {"6", "Access denied! Authenticate first!"},
            {"7", "Server is not running" },
            {"8", "This user isn't allowed to execute a command"},
            {"9", "Invalid user"}
        };
        
        public RconConnection(IL2StartupConfig config)
        {
            Config = config;
        }

        /// <summary>
        /// The <see cref="LastErrorDescription" /> property's name.
        /// </summary>
        public const string LastErrorDescriptionPropertyName = "LastErrorDescription";

        private string _lastErrorDescription;

        /// <summary>
        /// Sets and gets the LastErrorDescription property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LastErrorDescription
        {
            get
            {
                return _lastErrorDescription;
            }

            set
            {
                if (_lastErrorDescription == value)
                {
                    return;
                }

                _lastErrorDescription = value;
                RaisePropertyChanged(LastErrorDescriptionPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Config" /> property's name.
        /// </summary>
        public const string ConfigPropertyName = "Config";

        private IL2StartupConfig _config = null;

        /// <summary>
        /// Sets and gets the Config property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IL2StartupConfig Config
        {
            get
            {
                return _config;
            }

            set
            {
                if (_config == value)
                {
                    return;
                }

                _config = value;
                RaisePropertyChanged(ConfigPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsConnected" /> property's name.
        /// </summary>
        public const string IsConnectedPropertyName = "IsConnected";

        private bool _isConnected = false;

        /// <summary>
        /// Sets and gets the IsConnected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }

            set
            {
                if (_isConnected == value)
                {
                    return;
                }

                _isConnected = value;
                RaisePropertyChanged(IsConnectedPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsAuthorized" /> property's name.
        /// </summary>
        public const string IsAuthorizedPropertyName = "IsAuthorized";

        private bool _isAuthorized = false;

        /// <summary>
        /// Sets and gets the IsAuthorized property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsAuthorized
        {
            get
            {
                return _isAuthorized;
            }

            set
            {
                if (_isAuthorized == value)
                {
                    return;
                }

                _isAuthorized = value;
                RaisePropertyChanged(IsAuthorizedPropertyName);
            }
        }

        private void Connect()
        {
            if( Config.RconIP != null && 
                Config.RconPort >= 1 || 
                Config.RconPort <= 65535 )
            {
                Task.Factory.StartNew(() => {
                    Util.Try(() => {
                        connection = new TcpClient(Config.RconIP.ToString(), Config.RconPort);
                        netStream = connection.GetStream();
                    });
                    IsConnected = true;
                }).Wait(2000);
                
            }
        }
        private void Disconnect()
        {
            if (connection != null)
            {
                Util.Try(() => netStream.Close());
                Util.Try(() => connection.Close());
            }
        }
        /// <summary>
        /// Send raw command to server
        /// </summary>
        /// <param name="line">command</param>
        /// <returns>NameValueCollection with parameter name/value pairs</returns>
        public NameValueCollection RawCommand(string line)
        {
            lock( lockConnection )
            {
                LastErrorDescription = "Communication error";

                if (isStopped)
                    return new NameValueCollection();

                if (netStream == null || !netStream.CanWrite)
                    Start();

                if (netStream.CanWrite)
                {
                    Byte[] sendBytes = Encoding.UTF8.GetBytes(String.Concat(line));
                    Byte[] length = BitConverter.GetBytes((ushort)(line.Length + 1));
                    Byte[] zero = { 0 };
                    Byte[] packet = length.Concat(sendBytes).Concat(zero).ToArray();
                    Util.Try(() => netStream.Write(packet, 0, packet.Length), false);
                }
                else
                {
                    Disconnect();
                    return null;
                }
                
                var writeTask = Task.Factory.StartNew<NameValueCollection>(() =>
                {
                    if (netStream.CanRead)
                    {
                        while (!netStream.DataAvailable)
                            Thread.Sleep(1);

                        byte[] bytes = new byte[connection.ReceiveBufferSize];
                        Util.Try(() => netStream.Read(bytes, 0, (int)connection.ReceiveBufferSize), false);
                        UInt16 length = BitConverter.ToUInt16(bytes.Take(2).ToArray(), 0);
                        string response = null;
                        if (length > 2)
                            response = Encoding.UTF8.GetString(bytes.Skip(2).Take((int)length - 1).ToArray());

                        if (!String.IsNullOrWhiteSpace(response))
                        {
                            var result = HttpUtility.ParseQueryString(response);
                            string errorText;
                            LastErrorDescription = String.Empty;
                            
                            if( errorCodes.TryGetValue(result["STATUS"], out errorText) )
                                LastErrorDescription = errorText;

                            return result;
                        }
                        else
                        {
                            return new NameValueCollection();
                        }
                    }
                    else
                    {
                        Disconnect();
                        Start();
                    }
                    return new NameValueCollection();
                });
                writeTask.Wait(1000);
                return writeTask.Result;
            }
        }
        /// <summary>
        /// Authorizes user on rcon server with user/password taken from the startup.cfg
        /// </summary>
        private void Authenticate()
        {
            var result = RawCommand(String.Format("auth {0} {1}", Config.Login, Config.Password));
            Log.WriteInfo("Rcon authentication: {0}", GetResult(result["STATUS"]) );
        }

        private string GetResult(string result)
        {
            string code;
            errorCodes.TryGetValue(result, out code);
            return code ?? "UNKNOWN";
        }

        /// <summary>
        /// Get server console text
        /// </summary>
        /// <returns>Console text</returns>
        public string GetConsole()
        {
            return RawCommand("getconsole")["console"];
        }

        /// <summary>
        /// Check if rcon user is authenticated
        /// </summary>
        /// <returns>true if authenticated, false if not</returns>
        public bool AuthenticationStatus()
        {
            if( RawCommand("mystatus")["authed"] == "1" )
                return true;

            return false;
        }
        /// <summary>
        /// Request player list 
        /// </summary>
        /// <returns>List of Player objects</returns>
        public List<Player> GetPlayerList()
        {
            List<Player> result = new List<Player>();
            var rconResult = RawCommand("getplayerlist");
            if( rconResult["STATUS"] == "1")
            {
                var playerList = rconResult["playerList"];
                if( !String.IsNullOrEmpty( playerList ))
                {
                    var table = playerList.Split('|');
                    if( table.Length > 1 )
                    {
                        result = new List<Player>(
                            table.Select(line => line.Split(',')).Select(x => {
                                    int cid, ping;
                                    Guid nickGuid, userGuid;
                                    PlayerStatus status;
                                    if (x.Length == 6 &&
                                        int.TryParse(x[0], out cid) &&
                                        Guid.TryParse(x[1], out nickGuid) &&
                                        Guid.TryParse(x[2], out userGuid) &&
                                        int.TryParse(x[5], out ping) && 
                                        Enum.TryParse<PlayerStatus>( x[4], out status))
                                    {
                                        return new Player()
                                        {
                                            ClientId = cid,
                                            IsOnline = true,
                                            LoginId = userGuid,
                                            NickId = nickGuid,
                                            NickName = x[3],
                                            Ping = ping,
                                            Status = status,
                                        };
                                    }
                                    else
                                    {
                                        return null;
                                    }
                            }));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Check server status
        /// </summary>
        /// <returns>true if server alive</returns>
        public bool GetServerStatus()
        {
            return RawCommand("serverstatus")["STATUS"] == "1";
        }
                
        /// <summary>
        /// Kick player
        /// </summary>
        /// <param name="id">nickname, client id or nick/user guid</param>
        public void Kick(object id)
        {
            if( id is string || id is Guid || id is int )
            {
                RawCommand(String.Format("kick {0}", id.ToString()));
            }
        }

        /// <summary>
        /// Ban player
        /// </summary>
        /// <param name="id">nickname, client id or nick/user guid</param>
        public void Ban( object id )
        {
            if (id is string || id is Guid || id is int)
            {
                RawCommand(String.Format("ban {0}", id.ToString()));
            }
        }

        /// <summary>
        /// Unban all players
        /// </summary>
        public void UnBanAll()
        {
            RawCommand("unbanall");
        }

        /// <summary>
        /// Call named action in mision
        /// </summary>
        /// <param name="name">name of action</param>
        public void ServerInput(string name)
        {
            RawCommand(String.Format("serverinput {0}", name));
        }

        /// <summary>
        /// Send server stats immediately
        /// </summary>
        public void SendStats()
        {
            RawCommand("sendstatnow");
        }

        /// <summary>
        /// Write chat log to file immediately
        /// </summary>
        public void FlushChatLog()
        {
            RawCommand("cutchatlog");
        }

        /// <summary>
        /// Send chat message
        /// </summary>
        /// <param name="roomType">where to send a message: all, player, coalition or country</param>
        /// <param name="text">message text</param>
        /// <param name="id">optional. Client id, country id, coalition id</param>
        public void ChatMessage(RoomType roomType, string text, object id = null)
        {
            if( id == null )
                id = -1;

            RawCommand(String.Format("chatmsg {0} {1} {2}", (int)roomType, id, text ));
        }

        public void Start()
        {
            isStopped = false;
            Connect();
            Authenticate();
        }

        public void Stop()
        {
            Disconnect();
            isStopped = true;
        }

        public void Restart()
        {
            
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
