using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class DServerManager : IStopStart
    {
        private object lockDservers = new object();
        private ProcessMonitor dserverProcMonitor;
        public DServerManager()
        {
            dserverProcMonitor = new ProcessMonitor("DServer.exe");
            Servers = new ObservableCollection<Server>();
            dserverProcMonitor.AddProcess = (process) => {
                AddServer(GetServer(process));
            };
            dserverProcMonitor.RemoveProcess = (id) =>
            {
                RemoveServer(id);
            };
            dserverProcMonitor.Start();
        }
        public ObservableCollection<Server> Servers { get; set; }

        public void Start()
        {
            dserverProcMonitor.Start();
            foreach (var server in dserverProcMonitor.RunningProcesses)
            {
                AddServer(GetServer(server));
            }
        }

        private Server GetServer( ProcessItem process )
        {
            var baseDir = Directory.GetParent(Directory.GetParent( process.ProcessPath ).FullName);
            var config = new IL2StartupConfig( String.Concat(baseDir, @"\data\startup.cfg"));
            var rcon = new RconConnection( config );
            var server = new Server(rcon, process);
            return new Server(rcon, process);
        }
        private void RemoveServer( uint processId)
        {
            lock( lockDservers )
            {
                var server = Servers.FirstOrDefault(ds => ds != null && ds.Process != null && ds.Process.ProcessId == processId);
                if( server != null )
                {
                    server.MissionLogService.Stop();
                    Util.Try(() => server.Rcon.Stop());
                    UI.Dispatch(() => {
                        Servers.Remove(server);
                        dserverProcMonitor.Remove(processId);
                    });
                }
            }
        }
        private void AddServer(Server server)
        {
            var existingDServer = Servers.FirstOrDefault(s => s.Rcon.Config.GameRootFolder.Equals(server.Rcon.Config.GameRootFolder));
            if( existingDServer != null )
            {
                var process = Process.GetProcessById((int)existingDServer.Process.ProcessId);
                //Previous DServer process didn't exit correctly
                if( process != null)
                {
                    process.Kill();
                    RemoveServer(existingDServer.Process.ProcessId);
                }
            }
            UI.Dispatch(() =>
            {
                lock (lockDservers)
                    Servers.Add(server);
            });
            Task.Factory.StartNew((obj) => {
                var srv = (obj as Server);
                if (srv == null)
                    return;

                srv.IsConfigSet = srv.Rcon.Config.IsConfigReady;
                (obj as Server).Login();             

            }, server, TaskCreationOptions.LongRunning).ContinueWith((task, obj) =>
            {
                var srv = (obj as Server);
                if (srv == null)
                    return;

                srv.ServerId = GuidUtility.Create(GuidUtility.IsoOidNamespace, srv.Name);
                Log.WriteInfo("Start server monitor for server {0}", srv.Name);
                srv.MissionLogService.Start();
                srv.IsRconConnected = true;

            }, server);
        }
        public Server this[string key]
        {
            get {
                    return GetServerById(key);
                }   
        }
        public Server GetServerById( string id )
        {
            return Servers.FirstOrDefault(s => s != null &&
                s.ServerId != null &&
                s.ServerId.ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }
        public void Stop()
        {
            dserverProcMonitor.Stop();
            foreach( var server in Servers )
            {
                Util.Try(() => server.Rcon.Stop());
                server.MissionLogService.Stop();
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
