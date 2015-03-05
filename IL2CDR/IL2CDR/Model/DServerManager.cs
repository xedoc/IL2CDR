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
            DServers = new ObservableCollection<Server>();
            dserverProcMonitor.AddProcess = (process) => {
                AddServer(GetServer(process));
            };
            dserverProcMonitor.RemoveProcess = (id) =>
            {
                RemoveServer(id);
            };
            dserverProcMonitor.Start();
        }
        public ObservableCollection<Server> DServers { get; set; }

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
                var server = DServers.FirstOrDefault(ds => ds != null && ds.Process != null && ds.Process.ProcessId == processId);
                if( server != null )
                {
                    server.MissionLogService.Stop();
                    Util.Try(() => server.Rcon.Stop());
                    UI.Dispatch(() => {
                        DServers.Remove(server);
                        dserverProcMonitor.Remove(processId);
                    });
                }
            }
        }
        private void AddServer(Server server)
        {
            var existingDServer = DServers.FirstOrDefault(s => s.Rcon.Config.GameRootFolder.Equals(server.Rcon.Config.GameRootFolder));
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
                    DServers.Add(server);
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
                
                srv.MissionLogService.Start();
                srv.IsRconConnected = true;

            }, server);
        }
        public void Stop()
        {
            dserverProcMonitor.Stop();
            foreach( var server in DServers )
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
