using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
	public class TextFileTracker : IStopStart
	{
		private FileSystemWatcher watcher;
		private readonly Dictionary<string, long> filePositions;
		private ConcurrentQueue<string> logLinesQueue;
		public string Folder { get; }
		private readonly string mask;

		public Func<string, bool> Preprocess { get; set; }
		public Action<string> OnNewLine { get; set; }
		public Action<string> OnFileCreation { get; set; }
		public Action<string> OnChanged { get; set; }
		public string CurrentFileName { get; set; }


		/// <summary>
		/// This property answers the question, whether this component is running or not.
		/// </summary>
		public bool IsRunning { get; private set; }


		public TextFileTracker(string folder, string mask)
		{
			this.Folder = folder;
			this.mask = mask;
			this.filePositions = new Dictionary<string, long>();
		}

		public void SetupFolderWatcher()
		{
			if (string.IsNullOrWhiteSpace(this.Folder)) {
				return;
			}

			this.watcher = new FileSystemWatcher(this.Folder, this.mask) {
				NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName
			};
			this.watcher.Changed += this.watcher_Changed;
			this.watcher.Created += this.watcher_Changed;
			this.watcher.EnableRaisingEvents = true;
		}

		private void watcher_Changed(object sender, FileSystemEventArgs e)
		{
			var path = e.FullPath;
			this.CurrentFileName = path;
			if (e.ChangeType == WatcherChangeTypes.Created) {
				this.OnFileCreation?.Invoke(e.FullPath);
			}

			if (e.ChangeType != WatcherChangeTypes.Deleted) {
				if (this.Preprocess != null) {
					var handled = false;
					Util.Try(() => handled = this.Preprocess(File.ReadAllText(path)));
					if (handled) {
						return;
					}
				}
			}

			if (e.ChangeType == WatcherChangeTypes.Changed) {
				this.OnChanged?.Invoke(e.FullPath);

				if (this.OnNewLine != null) {
					this.ReadNewLines(e.FullPath);
					while (this.logLinesQueue.TryDequeue(out var line)) {
						this.OnNewLine(line);
					}
				}
			}
		}

		private void ReadNewLines(string path)
		{
			if (!this.filePositions.ContainsKey(path)) {
				this.filePositions.Add(path, 0);
			}

			var openException = Util.Try(() => {
				using (Stream stream = File.Open(path, FileMode.Open)) {
					stream.Seek(this.filePositions[path], 0);
					using (var reader = new StreamReader(stream)) {
						while (!reader.EndOfStream) {
							var e = Util.Try(() => this.logLinesQueue.Enqueue(reader.ReadLine()));
							if (e != null) {
								Log.WriteError("Error reading line from {0} {1}", path, e.Message);
							}
						}

						if (reader.BaseStream != null && reader.BaseStream.Position >= 0) {
							this.AddFileOffset(path, reader.BaseStream.Position);
						}
					}
				}
			});

			if (openException != null) {
				Log.WriteError("Can't open a file {0} {1}", path, openException.Message);
			}
		}

		public void AddFileOffset(string path, long offset)
		{
			if (this.filePositions != null &&
				!string.IsNullOrWhiteSpace(path) &&
				offset >= 0) {
				if (!this.filePositions.ContainsKey(path)) {
					this.filePositions.Add(path, offset);
				} else {
					this.filePositions[path] = offset;
				}
			}
		}

		public void Start()
		{
			this.IsRunning = true; 
			this.logLinesQueue = new ConcurrentQueue<string>();
			this.filePositions.Clear();
			this.SetupFolderWatcher();
		}

		public void Stop()
		{
			if (this.watcher != null) {
				this.watcher.EnableRaisingEvents = false;
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