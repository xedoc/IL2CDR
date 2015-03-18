using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IL2CDR.Model
{
    public class ActionManager
    {
        private ScriptManager scriptManager;

        public ActionManager(ScriptManager scriptManager)
        {
            this.scriptManager = scriptManager;
        }
        public void ProcessAction( object data )
        {
            scriptManager.RunActionScripts(data);
        }

        public void ProcessHistory( object data )
        {
            scriptManager.RunHistoryScripts(data);
        }
    }
}
