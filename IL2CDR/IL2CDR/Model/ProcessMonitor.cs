using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ProcessMonitor()
        {
            startWatcher = new ManagementEventWatcher("Select * From Win32_ProcessStartTrace");
            stopWatcher = new ManagementEventWatcher("Select * From Win32_ProcessStopTrace");
            Initialize();
        }
        public ProcessMonitor (string processName)
	    {
            startWatcher = new ManagementEventWatcher(String.Format(@"Select * From Win32_ProcessStartTrace WHERE ProcessName LIKE ""{0}""", processName));
            stopWatcher = new ManagementEventWatcher(String.Format(@"Select * From Win32_ProcessStopTrace WHERE ProcessName LIKE ""{0}""", processName));
            Initialize();
        }

        private void Initialize()
        {
            RunningProcesses = new ObservableCollection<ProcessItem>();

            startWatcher.EventArrived += startWatcher_EventArrived;
            stopWatcher.EventArrived += stopWatcher_EventArrived;


        }
        void stopWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            lock( lockProcesses )
                RunningProcesses.RemoveAll(p => p.ProcessId == (UInt32)e.NewEvent["ProcessID"]);
        }

        void startWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            lock (lockProcesses)
                RunningProcesses.Add(new ProcessItem((UInt32)e.NewEvent["ProcessID"], e.NewEvent["ProcessName"] as string));
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
        public ProcessItem( UInt32 id, string name )
        {
            ProcessId = id;
            ProcessName = name;
        }
        public UInt32 ProcessId { get; set; }
        public string ProcessName { get;set; }
    }
}
