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
        public override ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    Enabled = false,
                    Title = "Example script",
                    Description = "This script does nothing",
                };
            }
        }

        public override void OnAirfieldInfo(MissionLogEventAirfieldInfo data)
        {
            Log.WriteInfo("Log message from script");
        }
    }

}
