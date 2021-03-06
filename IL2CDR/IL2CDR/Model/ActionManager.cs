﻿using System;
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
        public void ProcessServerLogStart( Server server )
        {
            scriptManager.RunServerLogStartScripts(server);
        }
        public void ProcessHistory( object data )
        {
            scriptManager.RunHistoryScripts(data);
        }

        public void ProcessButtonClick( string buttonName )
        {
            scriptManager.ProcessButtonClick(buttonName);
        }

        public void ProcessPlayerListChange( Server server, List<Player> players)
        {
            scriptManager.ProcessPlayerListChange(server, players);
        }

        public void RunServerStartScripts( Server server)
        {
            scriptManager.RunServerStartScripts(server);
        }

        public void RunServerStopScript( Server server)
        {
            scriptManager.RunServerStopScripts(server);
        }

        public void RunRconConnectedScripts( Server server )
        {
            scriptManager.RunRconConnectedScripts(server);
        }
    }
}
