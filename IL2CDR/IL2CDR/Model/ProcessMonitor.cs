using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace IL2CDR.Model
{
	public class ProcessMonitor : IStopStart
	{
		private readonly object lockProcesses = new object();
		private readonly ManagementEventWatcher startWatcher;
		private readonly ManagementEventWatcher stopWatcher;

		public ObservableCollection<ProcessItem> RunningProcesses { get; set; }
		public Action<ProcessItem> AddProcess { get; set; }
		public Action<int> RemoveProcess { get; set; }

		public ProcessMonitor()
		{
			this.startWatcher = new ManagementEventWatcher("Select * From Win32_ProcessStartTrace");
			this.stopWatcher = new ManagementEventWatcher("Select * From Win32_ProcessStopTrace");
			this.RunningProcesses = new ObservableCollection<ProcessItem>();
			this.Initialize();
		}

		public ProcessMonitor(string processName)
		{
			this.startWatcher = new ManagementEventWatcher($@"Select * From Win32_ProcessStartTrace WHERE ProcessName LIKE ""{processName}""");
			this.stopWatcher = new ManagementEventWatcher( $@"Select * From Win32_ProcessStopTrace WHERE ProcessName LIKE ""{processName}""");

			var noextName = Path.GetFileNameWithoutExtension(processName);
			this.RunningProcesses = new ObservableCollection<ProcessItem>(
				Process.GetProcesses()
					.Where(p => p.ProcessName.Equals(noextName, StringComparison.InvariantCultureIgnoreCase) && p.MainModule != null)
					.DistinctBy(p => p.MainModule)
					.Select(p => new ProcessItem(p.Id, p.ProcessName, Path.GetDirectoryName(p.MainModule?.FileName), p.CommandLine()))
			);
			this.Initialize();
		}

		private void Initialize()
		{
			this.startWatcher.EventArrived += this.startWatcher_EventArrived;
			this.stopWatcher.EventArrived += this.stopWatcher_EventArrived;
		}

		public void Remove(int id)
		{
			this.RunningProcesses.RemoveAll(p => p.Id == id);
		}

		private void stopWatcher_EventArrived(object sender, EventArrivedEventArgs e)
		{
			lock (this.lockProcesses) {
				this.RemoveProcess?.Invoke((int) (uint) e.NewEvent["ProcessID"]);
			}
		}

		private void startWatcher_EventArrived(object sender, EventArrivedEventArgs e)
		{
			lock (this.lockProcesses) {
				var process = this.GetProcessDetails(e.NewEvent);
				this.AddProcess?.Invoke(process);

				if (process != null) {
					this.RunningProcesses.Add(process);
				}
			}
		}

		private ProcessItem GetProcessDetails(ManagementBaseObject obj)
		{
			if (obj == null) {
				return null;
			}

			var id = (uint) obj["ProcessID"];
			var name = obj["ProcessName"] as string;
			var process = Process.GetProcessById((int) id);
			var fileName = Path.GetDirectoryName(process.MainModule?.FileName);
			var commandLine = process.CommandLine();

			return new ProcessItem((int) id, name, fileName, commandLine);
		}

		public void Start()
		{
			this.startWatcher.Start();
			this.stopWatcher.Start();
		}

		public void Stop()
		{
			this.startWatcher.Stop();
			this.stopWatcher.Stop();
		}

		public void Restart()
		{
			this.Stop();
			this.Start();
		}
	}

	public class ProcessItem
	{
		public ProcessItem(int id, string name, string processPath, string commandLine)
		{
			this.Id = id;
			this.Name = name;
			this.Path = processPath;
			this.CommandLine = commandLine;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
		public string CommandLine { get; set; }
	}
}