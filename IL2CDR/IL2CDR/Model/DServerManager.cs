using System;
using System.Collections.Generic;
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
           
        }

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
                }
            }

        }


        public void Start()
        {
            dserverProcMonitor.Start();
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
