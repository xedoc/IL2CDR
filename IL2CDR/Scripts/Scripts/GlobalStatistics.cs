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
        private const string URL = "http://" + DOMAIN + ":49191/e/?XDEBUG_SESSION_START=55A2686E";
        private const string BACKLOGFILE = "eventback.log";
        private ConcurrentQueue<object> events;
        private Timer sendTimer;
        private const int SENDTIMEOUT = 5 * 60 * 1000;
        private object lockSend = new object();
        private string lastPacket = String.Empty;
        private string token = String.Empty;
        private bool dictionarySent = false;

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
        public override void OnServerLogStart(Server server)
        {
            var packet = new { Type = 9999, Server = server };
            AddToQueue(packet);
        }

        public string Token {
            get
            {
                if (Config == null)
                    return String.Empty;

                return Config.GetString("token");
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

                if (String.IsNullOrWhiteSpace(lastPacket))
                    return;

                using( WebClientBase webClient = new WebClientBase())
                {
                    webClient.ContentType = ContentType.JsonUTF8;
                    webClient.KeepAlive = false;
                    webClient.SetCookie("srvtoken", Token, DOMAIN);
                    result = webClient.Upload(URL, lastPacket);
                }
                if( !String.IsNullOrWhiteSpace(result) )
                {
                    Log.WriteInfo("Packet sent to statistics server. Size(bytes): {0}, Result: {1}", lastPacket.Length, result);
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
            if (String.IsNullOrWhiteSpace(Token))
                return;

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

            if (!dictionarySent)
            {
                //Send game objects classification info
                var packet = new
                {
                    Token = Token,
                    Type = 9998,
                    ObjectInfo = GameInfo.ObjectsClassification.Select(
                    pair => new
                    {
                        ObjectId = GuidUtility.Create(GuidUtility.IsoOidNamespace, pair.Key),
                        Name = pair.Key,
                        Class = pair.Value.Classification.ToString("g"),
                        Purpose = pair.Value.Purpose
                    }).ToArray()
                };

                AddToQueue(packet);
            }

            AddToQueue(data);
        }
        


    }
   

}
