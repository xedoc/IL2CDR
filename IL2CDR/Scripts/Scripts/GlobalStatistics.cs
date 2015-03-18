using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

namespace IL2CDR.Scripts
{
    public class GlobalStatistics : ActionScriptBase
    {
        private const string DOMAIN = "localhost";
        private const string URL = "http://" + DOMAIN + ":3992/e/";
        private const string BACKLOGFILE = "eventback.log";
        private ConcurrentQueue<object> events;
        private Timer sendTimer;
        private const int SENDTIMEOUT = 5 * 60 * 1000;
        private object lockSend = new object();
        private string lastPacket = String.Empty;

        public GlobalStatistics()
        {
            events = new ConcurrentQueue<object>();
            sendTimer = new Timer(SendTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
        }
        public override ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    //Enable events processing by this script
                    IsEnabled = true,
                    //Title for script list
                    Title = "Global statistics",
                    //Script description for GUI
                    Description = "Send events to il2.info",
                    //Settings in GUI
                    ConfigFields = new ConfigFieldList()
                    {
                        { "token", "Token", "Server authentication token", FieldType.Text, String.Empty, true},
                    },
                };
            }
        }
        public override void OnApplicationStartup(object data)
        {
            sendTimer.Change(0, SENDTIMEOUT);
        }
        public override void OnApplicationShutdown(object data)
        {
            sendTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void SendDataToServer()
        {
            lock( lockSend )
            {
                string result = "FAIL";
                if( String.IsNullOrWhiteSpace( lastPacket ) )
                    lastPacket = GetNextPacket();

                using( WebClientBase webClient = new WebClientBase())
                {
                    webClient.ContentType = ContentType.JsonUTF8;
                    webClient.KeepAlive = false;
                    webClient.SetCookie("srvtoken", "test", DOMAIN);
                    if (lastPacket.Length >= 500)
                    {
                        result = webClient.Upload(URL, lastPacket);
                    }                        
                    else
                    {
                        result = webClient.Upload(URL, lastPacket);
                    }                        
                }
                if( !String.IsNullOrWhiteSpace(result) )
                {
                    Log.WriteInfo("Packet sent to statistics server. Result: {0}", result);
                    if (result.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
                        lastPacket = String.Empty;

                }

            }
        }

        public string GetNextPacket()
        {
            if (events.IsEmpty)
                return null;

            string content = String.Empty;
            var jsonPackets = new List<object>();

            while (!events.IsEmpty)
            {
                object obj;
                if (events.TryDequeue(out obj))
                {
                    jsonPackets.Add(obj);
                }
            }
            return Json.Serialize(jsonPackets.ToList());
        }

        public void AddToQueue( object data )
        {
            events.Enqueue(data);

            if (events.Count >= 10)
                SendDataToServer();
        }

        private void SendTimerCallback( object sender )
        {
            if (events.Count > 0)
                SendDataToServer();
        }
        public override void OnHistory(object data)
        {
            if (data == null || 
                !(data is MissionLogEventHeader) ||
                data is MissionLogEventVersion)
                return;

            AddToQueue(data);
        }
        


    }
   

}
