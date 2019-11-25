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
		private readonly ScriptManager scriptManager;

		public ActionManager(ScriptManager scriptManager)
		{
			this.scriptManager = scriptManager;
		}

		public void ProcessAction(object data)
		{
			this.scriptManager.RunActionScripts(data);
		}

		public void ProcessServerLogStart(Server server)
		{
			this.scriptManager.RunServerLogStartScripts(server);
		}

		public void ProcessHistory(object data)
		{
			this.scriptManager.RunHistoryScripts(data);
		}

		public void ProcessButtonClick(string buttonName)
		{
			this.scriptManager.ProcessButtonClick(buttonName);
		}

		public void ProcessPlayerListChange(Server server, List<Player> players)
		{
			this.scriptManager.ProcessPlayerListChange(server, players);
		}

		public void RunServerStartScripts(Server server)
		{
			this.scriptManager.RunServerStartScripts(server);
		}

		public void RunServerStopScript(Server server)
		{
			this.scriptManager.RunServerStopScripts(server);
		}

		public void RunRconConnectedScripts(Server server)
		{
			this.scriptManager.RunRconConnectedScripts(server);
		}
	}
}