using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

namespace IL2CDR.Scripts
{
    public class Example : ActionScriptBase
    {
        public Example()
        {
        
        }
        public override ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    IsEnabled = false,
                    Title = "Example script",
                    Description = "This script does nothing",
                    
                    //field name must be unique
                    ConfigFields = new ConfigFieldList()
                    {
                        { "fieldName1", "Text field label", "Text watermark", FieldType.Text, "Default value", true},
                        { "fieldName2", "Password field label", "Password watermark", FieldType.Text, String.Empty, true},
                    },
                };
            }
        }
        public override void OnApplicationStartup(object data)
        {            
        }
        public override void OnApplicationShutdown(object data)
        {
        }
        public override void OnKill(MissionLogEventKill data)
        {
            //Don't run script on this server
            if (!IsMyServer(data))
                return;

            //If server is known - run some actions here
        }
        private bool IsMyServer(MissionLogEventHeader data)
        {
            if (data == null)
                return false;

            return data.Server.Name.Contains("Scripted dogfight server");
        }
    }
    public class Test
    {
        public void TestMethod()
        {
            Log.WriteInfo("Test method called from another script");
        }
    }

}
