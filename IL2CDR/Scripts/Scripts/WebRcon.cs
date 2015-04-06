using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

namespace Scripts.Scripts
{
    public class WebRcon : ActionScriptBase
    {
        private const string URL = "http://il2.info/rcon";
        private bool endPoll = false;
        private App app;
        private WebClientBase webClient = new WebClientBase();
        public WebRcon()
        {
            
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

                var result = webClient.Download(URL);
                Log.WriteInfo(result);
                
       
            }
        }
    }
}
