using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using IL2CDR;
using IL2CDR.Model;

namespace IL2CDR.Scripts
{
    public class GlobalStatistics : ActionScriptBase
    {
        private ConcurrentQueue<string> jsonPackets;
        private Timer sendTimer;
        private const int SENDTIMEOUT = 5 * 60 * 1000;
        private object lockSend = new object();

        public GlobalStatistics()
        {
            jsonPackets = new ConcurrentQueue<string>();
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
                        { "url", "URL", "Stats URL", FieldType.Text, String.Empty, true},
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

        private void SendDataToServer()
        {
            lock( lockSend )
            {
                string content = String.Empty;
                while( !jsonPackets.IsEmpty )
                {
                    string packet;
                    if( jsonPackets.TryDequeue( out packet ) )
                    {
                        content = String.Concat(content, packet);
                    }
                }
                //TODO: post via WebClient
            }
        }

        private void SendTimerCallback( object sender )
        {
            if (jsonPackets.Count > 0)
                SendDataToServer();
        }
        public override void OnAny(object data)
        {
            if (data == null)
                return;
            
            var json = new JavaScriptSerializer().Serialize(data);
            jsonPackets.Enqueue(json);
            if (jsonPackets.Count >= 10)
                SendDataToServer();
        }
        

    }

}
