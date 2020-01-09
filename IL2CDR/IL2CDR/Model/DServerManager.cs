using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IL2CDR.Properties;

namespace IL2CDR.Model
{
	public class DServerManager : IStopStart
	{
		private readonly object lockDservers = new object();
		private readonly ActionManager actionManager;
		private readonly ProcessMonitor dserverProcMonitor;

		/// <summary>
		/// This property answers the question, whether this component is running or not.
		/// </summary>
		public bool IsRunning { get; private set; }


		public DServerManager()
		{
			this.dserverProcMonitor = new ProcessMonitor("DServer.exe");
			this.Servers = new ObservableCollection<Server>();
			this.dserverProcMonitor.AddProcess = (process) => { this.AddServer(this.GetServer(process)); };
			this.dserverProcMonitor.RemoveProcess = this.RemoveServer;
			this.dserverProcMonitor.Start();
			this.actionManager = (Application.Current as App)
				.Return(x => x.ActionManager, new ActionManager(new ScriptManager()));
		}

		public ObservableCollection<Server> Servers { get; set; }

		public void Start()
		{
			this.IsRunning = true; 
			this.dserverProcMonitor.Start();
			foreach (var serverProcessItem in this.dserverProcMonitor.RunningProcesses.ToList()) {
				this.AddServer(this.GetServer(serverProcessItem));
			}
		}

		private Server GetServer(ProcessItem process)
		{
			var baseDir = Directory.GetParent(Directory.GetParent(process.Path).FullName);
			var config = new IL2StartupConfig(string.Concat(baseDir, @"\data\startup.cfg"));
			var rcon = new RconConnection(config);

			var appSettings = Settings.Default.Config;
			rcon.RconDefaultLogin = appSettings.RconStaticLogin;
			rcon.RconDefaultPassword = appSettings.RconStaticPassword; 

			var server = new Server(config, rcon, process);
			return server;
		}

		private void RemoveServer(int processId)
		{
			lock (this.lockDservers) {
				var server = this.Servers.FirstOrDefault(ds => ds?.Process != null && ds.Process.Id == processId);
				if (server == null) {
					return;
				}

				server.MissionLogService.Stop();
				Util.Try(() => server.Rcon.Stop());
				UI.Dispatch(() => {
					this.Servers.Remove(server);
					this.dserverProcMonitor.Remove(processId);
				});
			}
		}

		private void AddServer(Server server)
		{
			var existingDServer = this.Servers.FirstOrDefault(s => s.Rcon.Il2ServerConfig.GameRootFolder == server.Rcon.Il2ServerConfig.GameRootFolder);
			if (existingDServer != null) {
				try {
					var process = Process.GetProcessById((int) existingDServer.Process.Id);
					//Previous DServer process didn't exit correctly
					if (process != null) {
						process.Kill();
						this.RemoveServer(existingDServer.Process.Id);
						this.actionManager.RunServerStopScript(server);
					}
				} catch (Exception ex) {
					// -- ignore the error 
				}
			}

			UI.Dispatch(() => {
				lock (this.lockDservers) {
					this.Servers.Add(server);
					this.actionManager.RunServerStartScripts(server);
				}
			});

			// -- set appropriate attributes: (taken out of the async block)

			server.ServerId = GuidUtility.Create(GuidUtility.IsoOidNamespace, server.Name);

			var appConfig = Settings.Default.Config;

			if (appConfig.IsMissionLogMonitorEnabled) {
				Log.WriteInfo("Starting MissionLogService for the server {0}", server.Name);
				server.MissionLogService.Start();
			} else {
				Log.WriteInfo("MissionLogService for the server {0} is NOT started! MissionLogMonitor is DISABLED in the application Settings.", server.Name);
			}

			if (appConfig.IsRConEnabled && this.IsRunning) {
				server.Rcon.Start();
			}


			/*
			Task.Factory.StartNew((obj) => {
				if (!(obj is Server srv)) {
					return;
				}

				srv.IsConfigSet = srv.Rcon.Il2ServerConfig.IsConfigReady;
				srv.AccountLogin();
				srv.IsRconConnected = true;
			},  server, TaskCreationOptions.LongRunning);
			*/
		}

		public Server this[string key] => this.GetServerById(key);

		public Server GetServerById(string id)
		{
			return this.Servers.FirstOrDefault(s => s != null &&
													s.ServerId != Guid.Empty &&
													s.ServerId.ToString().Equals(id,
														StringComparison.InvariantCultureIgnoreCase));
		}

		public void Stop()
		{
			this.dserverProcMonitor.Stop();
			foreach (var server in this.Servers) {
				Util.Try(() => server.Rcon.Stop());
				server.MissionLogService.Stop();
			}

			this.IsRunning = false; 
		}

		public void Restart()
		{
			this.Stop();
			this.Start();
		}
	}
}