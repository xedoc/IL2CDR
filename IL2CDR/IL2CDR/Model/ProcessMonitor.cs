using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{

    public class ProcessMonitor : IStopStart
    {
        private object lockProcesses = new object();
        private ManagementEventWatcher startWatcher;
        private ManagementEventWatcher stopWatcher;

        public ObservableCollection<ProcessItem> RunningProcesses { get; set; }
        public Action<ProcessItem> AddProcess { get; set; }
        public Action<uint> RemoveProcess { get; set; }
        public ProcessMonitor()
        {
            startWatcher = new ManagementEventWatcher("Select * From Win32_ProcessStartTrace");
            stopWatcher = new ManagementEventWatcher("Select * From Win32_ProcessStopTrace");
            RunningProcesses = new ObservableCollection<ProcessItem>();
            Initialize();
        }
        public ProcessMonitor (string processName)
	    {
            startWatcher = new ManagementEventWatcher(String.Format(@"Select * From Win32_ProcessStartTrace WHERE ProcessName LIKE ""{0}""", processName));
            stopWatcher = new ManagementEventWatcher(String.Format(@"Select * From Win32_ProcessStopTrace WHERE ProcessName LIKE ""{0}""", processName));

            var noextName = Path.GetFileNameWithoutExtension(processName);
            RunningProcesses = new ObservableCollection<ProcessItem>(
                Process.GetProcesses().Where(p => p.ProcessName.Equals(noextName, StringComparison.InvariantCultureIgnoreCase))
                .DistinctBy( p => p.MainModule )
                .Select( p => new ProcessItem( (uint)p.Id, p.ProcessName, Path.GetDirectoryName(p.MainModule.FileName)))
                );
            Initialize();

        }

        private void Initialize()
        {

            startWatcher.EventArrived += startWatcher_EventArrived;
            stopWatcher.EventArrived += stopWatcher_EventArrived;


        }
        public void Remove(uint id)
        {
            RunningProcesses.RemoveAll(p => p.ProcessId == id);
        }
        void stopWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            lock( lockProcesses )
            {
                if( RemoveProcess != null )
                    RemoveProcess((uint)e.NewEvent["ProcessID"]);
            }
        }

        void startWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            lock (lockProcesses)
            {
                var process = GetProcessDetails(e.NewEvent);
                if (AddProcess != null)
                    AddProcess(process);

                if( process != null )
                    RunningProcesses.Add(process);
            }
        }

        private ProcessItem GetProcessDetails( ManagementBaseObject obj )
        {
            if( obj == null )    
                return null;

            var id = (UInt32)obj["ProcessID"];
            var name =  obj["ProcessName"] as string;
            var process = Process.GetProcessById((int)id);
            var fileName = Path.GetDirectoryName( process.MainModule.FileName );

            return new ProcessItem((uint)id, name, fileName);
        }

        public void Start()
        {

            startWatcher.Start();
            stopWatcher.Start();
        }

        public void Stop()
        {
            startWatcher.Stop();
            stopWatcher.Stop();
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
    public class ProcessItem
    {
        public ProcessItem( UInt32 id, string name,string processPath )
        {
            ProcessId = id;
            ProcessName = name;
            ProcessPath = processPath;
        }
        public UInt32 ProcessId { get; set; }
        public string ProcessName { get;set; }
        public string ProcessPath { get; set; }
    }
}
