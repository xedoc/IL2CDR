//css_ref PresentationFramework
//css_ref System.Xaml
//css_ref WindowsBase
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using IL2CDR;
using IL2CDR.Model;

namespace Scripts.Scripts
{
    public class WebRcon : ActionScriptBase
    {
        private const string DOMAIN = "il2.info";
        private const string URL = "http://" + DOMAIN + "/rcon";
        private bool endPoll = false;
        private App app;
        private WebClientBase webClient = new WebClientBase();
        public WebRcon()
        {
            
        }
        public override ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    //Enable events processing by this script
                    IsEnabled = false,
                    //Title for script list
                    Title = "Web Rcon",
                    //Script description for GUI
                    Description = "Execute rcon commands via Web UI",
                    //Settings in GUI
                    ConfigFields = new ConfigFieldList()
                    {
                        //name, label, watermark, type, value, isVisible
                        { "token", "Token", "Server authentication token", FieldType.Text, String.Empty, true},
                    },
                };
            }
        }

        public override void OnApplicationStartup(object data)
        {
            app = data as App;
        }
        public override void OnApplicationShutdown(object data)
        {
            endPoll = true;
        }
        private void PollServer()
        {
            while( true )
            {
                if (endPoll)
                    break;

                webClient.SetCookie("srvtoken", Config.GetString("Token"), DOMAIN);
                var result = webClient.Download(URL);
                Log.WriteInfo(result);
                
                
                //app.DServerManager.Servers.ToList().ForEach( s => s.Rcon.ChatMessage( RoomType.All, "test") )
       
            }
        }
    }
}
