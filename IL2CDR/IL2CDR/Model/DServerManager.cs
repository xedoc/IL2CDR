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
        private object lockDservers = new object();
        private ProcessMonitor dserverProcMonitor;
        public DServerManager()
        {
            dserverProcMonitor = new ProcessMonitor("DServer.exe");
            DServers = new ObservableCollection<Server>();
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
                    lock (lockDservers)
                        AddServer(GetServer(addedServer));
                }
            }

        }
        public void Start()
        {
            dserverProcMonitor.Start();
            foreach (var server in dserverProcMonitor.RunningProcesses)
                AddServer(GetServer(server));
        }

        private Server GetServer( ProcessItem process )
        {
            var baseDir = Directory.GetParent(Directory.GetParent( process.ProcessPath ).FullName);
            var config = new IL2StartupConfig( String.Concat(baseDir, @"\data\startup.cfg"));
            var rcon = new RconConnection( config );

            return new Server(rcon);
        }
        private void AddServer(Server server)
        {
            lock( lockDservers )
            {
                Task.Factory.StartNew((obj) => { (obj as Server).Login(); }, server).ContinueWith((task, obj) =>
                {
                    DServers.Add(obj as Server);
                },server);
            }
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
