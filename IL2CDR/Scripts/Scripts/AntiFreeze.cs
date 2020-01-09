using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

//namespace IL2CDR.Scripts
//{
	/// <summary>
	/// Monitors IL2 DServer. Restarts server if freeze detected
	/// </summary>
	public class AntiFreeze : ActionScriptBase
	{
		private readonly Dictionary<int, ServerTimer> serverTimers;
		private readonly object lockServers = new object();

		public AntiFreeze()
		{
			this.serverTimers = new Dictionary<int, ServerTimer>();
		}

		/// <summary>
		/// This property will be requested by Script Manager when server on script initialization
		/// </summary>
		public override ScriptConfig DefaultConfig
		{
			get
			{
				return new ScriptConfig() {
					IsEnabled = true,
					Title = "AntiFreeze script",
					Description = "Automatically restart server if freeze/slow response detected",

					//field name must be unique
					ConfigFields = new ConfigFieldList() {
						//name, label, watermark, type, value, isVisible
						{
							"RestartTimeout", "Inactivity timeout(seconds)", "seconds to wait since last mission log entry",
							FieldType.Text, 60, true
						}, {
							"WaitMissionEnd", "Wait mission end (server is active but slow)",
							"If server is active but slow",
							FieldType.Flag, false, true
						},
					},
				};

			}
		}

		/// <summary>
		/// Raised when server start detected (no rcon connection yet)
		/// </summary>
		/// <param name="server">Server object</param>
		public override void OnServerStart(Server server)
		{
			var process = server?.With(x => x.Process);
			if (process == null) {
				return;
			}

			this.StartTimer(server);
		}

		public override void OnServerStop(Server server)
		{
			lock (this.lockServers) {
				this.serverTimers.Remove(server.Process.Id);
			}
		}

		/// <summary>
		/// History event handler. Minimum interval is 35 seconds normally. Otherwise consider server freeze.
		/// </summary>
		/// <param name="data">Mission event data object. Check MissionLogEvent.cs for possible classes</param>
		public override void OnHistory(object data)
		{
			var server = (data as MissionLogEventHeader)
				.With(x => x.Server);
			if (server?.Process == null) {
				return;
			}

			if (!this.IsTimerStarted(server)) {
				this.StartTimer(server);
			}

			this.ResetTimer(server);
		}

		/// <summary>
		/// Check if specified server timer is already started
		/// </summary>
		/// <param name="server">Server object</param>
		/// <returns>true if timer is already registered and running</returns>
		private bool IsTimerStarted(Server server)
		{
			ServerTimer serverTimer = null;
			lock (this.lockServers) {
				return this.serverTimers.TryGetValue(server.Process.Id, out serverTimer);
			}
		}

		/// <summary>
		/// Reset timer to default timeout
		/// </summary>
		/// <param name="server">Server object</param>
		private void ResetTimer(Server server)
		{
			ServerTimer serverTimer = null;
			lock (this.lockServers) {
				if (this.serverTimers.TryGetValue(server.Process.Id, out serverTimer)) {
					serverTimer.ResetTimer();
				}
			}
		}

		/// <summary>
		/// Start server restart timer
		/// </summary>
		/// <param name="server">Server object</param>
		private void StartTimer(Server server)
		{
			lock (this.lockServers) {
				if (!this.serverTimers.Any(x => x.Key.Equals(server.Process.Id))) {
					Log.WriteInfo(
						"AntiFreeze started for process ID:{0}. Server will be restarted if there won't be mission events for {1} seconds",
						server.Process.Id, this.Config.GetInt("RestartTimeout"));

					this.serverTimers.Add(server.Process.Id,
						new ServerTimer(this.Config, server));
				}
			}
		}
	}

	/// <summary>
	/// Server item. Used in the list of timers
	/// </summary>
	public class ServerTimer
	{
		public Timer Timer { get; set; }
		public Server Server { get; set; }
		public readonly ScriptConfig Config;
		public volatile bool IsStopping;

		public ServerTimer(ScriptConfig config, Server server)
		{
			if (config == null) {
				throw new ArgumentNullException("ScriptConfig object is null");
			}

			this.Server = server;
			this.Config = config;
			this.IsStopping = false;
			this.Timer = new Timer((sender) => { this.RestartServer(sender as ServerTimer); },
				this, this.Config.GetInt("RestartTimeout") * 1000,
				Timeout.Infinite);
		}

		/// <summary>
		/// Restart timer
		/// </summary>
		public void ResetTimer()
		{
			if (this.IsStopping) {
				return;
			}

			this.Timer.Change(this.Config.GetInt("RestartTimeout") * 1000, Timeout.Infinite);
		}

		/// <summary>
		/// Restart server if freeze/slowness detected
		/// </summary>
		private void RestartServer(ServerTimer timer)
		{
			if (this.IsStopping) {
				return;
			}

			this.IsStopping = true;
			if (timer == null || timer.Server == null) {
				return;
			}

			Log.WriteInfo("Stopping frozen server {0}...", timer.Server.Name);

			var id = timer.With(x => x.Server).With(x => x.Process).Return(x => x.Id, 0);
			if (id == 0) {
				Log.WriteInfo("Unable to restart server {0}. Unknown process id.", timer.Server.Name);
				return;
			}

			Util.Try(() => {
				var process = Process.GetProcessById(id);

				if (process == null) {
					Log.WriteInfo("Unable to restart server {0}. Process not found.", timer.Server.Name);
					return;
				}

				if (string.IsNullOrWhiteSpace(this.Server.Process.CommandLine)) {
					Log.WriteInfo("Unable to restart a server {0}. Empty command line!", timer.Server.Name);
					return;
				}

				process.Kill();
			}, false);

			Util.Try(() => {
				Thread.Sleep(1000);
				Log.WriteInfo("Starting server {0}...", timer.Server.Name);
				var exeFile = Re.GetSubString(this.Server.Process.CommandLine, @"^(.*exe).*$");
				var arguments = Re.GetSubString(this.Server.Process.CommandLine, @"^.*?\s(.*)$");
				var directory = this.Server.Process.Path;
				this.StartProcess(exeFile, arguments, directory);
			});
		}

		private int StartProcess(string exeFile, string arguments, string directory)
		{
			exeFile = exeFile.Replace(@"""", "");
			var process = new Process();
			process.StartInfo = new ProcessStartInfo();
			process.StartInfo.WorkingDirectory = directory;
			process.StartInfo.FileName = exeFile.Trim();
			process.StartInfo.Arguments = arguments.Trim();
			process.StartInfo.UseShellExecute = false;
			if (!string.IsNullOrWhiteSpace(process.StartInfo.WorkingDirectory)) {
				Directory.SetCurrentDirectory(process.StartInfo.WorkingDirectory);
			}

			process.Start();
			return process.Id;
		}
	}
//}