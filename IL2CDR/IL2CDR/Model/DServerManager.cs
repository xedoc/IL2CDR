using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class DServerManager : IStopStart
    {
        private ProcessMonitor dserverProcMonitor;
        public DServerManager()
        {
            dserverProcMonitor = new ProcessMonitor("DServer.exe");
            dserverProcMonitor.RunningProcesses.CollectionChanged += RunningProcesses_CollectionChanged;
            dserverProcMonitor.Start();
        }
        public ObservableCollection<Server> DServers { get; set; }

        void RunningProcesses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if( e.OldItems != null )
            {
                foreach( ProcessItem removedServer in e.OldItems )
                {
                    Log.WriteInfo("DServer removed. PID: {0}", removedServer.ProcessId);
                }
            }
            if( e.NewItems != null )
            {
                foreach (ProcessItem addedServer in e.NewItems)
                {
                    Log.WriteInfo("DServer added. PID: {0}", addedServer.ProcessId);
                    DServers.Add(GetServer(addedServer));
                }
            }

        }
        public void Start()
        {
            dserverProcMonitor.Start();
            DServers = new ObservableCollection<Server>(
                dserverProcMonitor.RunningProcesses.Select( p => GetServer(p)));
        }

        private Server GetServer( ProcessItem process )
        {
            var baseDir = Directory.GetParent(Directory.GetParent( process.ProcessPath ).FullName);
            var config = new IL2StartupConfig( String.Concat(baseDir, @"\data\startup.cfg"));
            var rcon = new RconConnection( config );

            return new Server(rcon);
        }

        public void Stop()
        {
            dserverProcMonitor.Stop();
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
