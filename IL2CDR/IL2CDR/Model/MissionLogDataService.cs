using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace IL2CDR.Model
{
    public class MissionLogDataService : IMissionLogDataService, IStopStart
    {
        private string watchPath = String.Empty;
        private FileSystemWatcher watcher;
        private Dictionary<string, int> filePositions;
        private ConcurrentQueue<string> logLinesQueue;

        public Action<string> OnNewLine { get; set; }

        public MissionLogDataService()
        {
            filePositions = new Dictionary<string, int>();
        }
        public void SetupFolderWatcher()
        {
            watcher = new FileSystemWatcher(watchPath, "*.txt");
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            watcher.Changed += watcher_Changed;
            watcher.EnableRaisingEvents = true;
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            if( e.ChangeType == WatcherChangeTypes.Changed )
            {
                ReadNewLines(e.FullPath);
                string line = null;
                while( logLinesQueue.TryDequeue( out line ))
                {
                    if (OnNewLine != null)
                        OnNewLine(line);
                }
                    
            }
            
        }
        private void ReadNewLines( string path )
        {            
            if( !filePositions.ContainsKey(path))
                filePositions.Add(path, 0);

            var openException = Util.Try(() => {
                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    stream.Seek(filePositions[path], 0);
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            var e = Util.Try(() => logLinesQueue.Enqueue(reader.ReadLine()));
                            if (e != null)
                                Log.WriteError("Error reading line from {0} {1}", path, e.Message);
                        }
                    }
                }            
            });

            if (openException != null)
                Log.WriteError("Can't open log file {0} {1}", path, openException.Message);

        }
        public void Start()
        {
            logLinesQueue = new ConcurrentQueue<string>();
            filePositions.Clear();
            SetupFolderWatcher();
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
