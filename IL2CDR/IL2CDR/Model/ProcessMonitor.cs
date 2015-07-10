using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using IL2CDR.Model;

namespace IL2CDR.Model
{

    public class ProcessMonitor : IStopStart
    {
        private object lockProcesses = new object();
        private ManagementEventWatcher startWatcher;
        private ManagementEventWatcher stopWatcher;

        public ObservableCollection<ProcessItem> RunningProcesses { get; set; }
        public Action<ProcessItem> AddProcess { get; set; }
        public Action<int> RemoveProcess { get; set; }
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
                .Select( p => new ProcessItem( p.Id, p.ProcessName, Path.GetDirectoryName(p.MainModule.FileName), p.CommandLine()))
                );
            Initialize();

        }

        private void Initialize()
        {

            startWatcher.EventArrived += startWatcher_EventArrived;
            stopWatcher.EventArrived += stopWatcher_EventArrived;


        }
        public void Remove(int id)
        {
            RunningProcesses.RemoveAll(p => p.Id == id);
        }
        void stopWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            lock( lockProcesses )
            {
                if( RemoveProcess != null )
                    RemoveProcess((int)((uint)e.NewEvent["ProcessID"]));
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

            var id = (uint)obj["ProcessID"];
            var name =  obj["ProcessName"] as string;
            var process = Process.GetProcessById((int)id);
            var fileName = Path.GetDirectoryName( process.MainModule.FileName );
            var commandLine = process.CommandLine();

            return new ProcessItem((int)id, name, fileName, commandLine);
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
        public ProcessItem( int id, string name,string processPath, string commandLine )
        {
            Id = id;
            Name = name;
            Path = processPath;
            CommandLine = commandLine;
            
        }
        public int Id { get; set; }
        public string Name { get;set; }
        public string Path { get; set; }
        public string CommandLine { get; set; }
    }
}
