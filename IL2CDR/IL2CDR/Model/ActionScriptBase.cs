using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
	[Serializable]
	public class ActionScriptBase : IActionScript, IScriptConfig
	{
		/// <summary>
		/// Area position/boundaries info. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnInfluenceAreaBoundary(MissionLogEventInfluenceAreaBoundary data)
		{
		}

		/// <summary>
		/// Area country info. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnInfluenceAreaInfo(MissionLogEventInfluenceAreaInfo data)
		{
		}

		/// <summary>
		/// Game object spawn (bots,ground vehicles etc). For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnGameObjectSpawn(MissionLogEventGameObjectSpawn data)
		{
		}

		/// <summary>
		/// Formation/group information (member IDs). For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnGroupInitInfo(MissionLogEventGroupInitInfo data)
		{
		}

		/// <summary>
		/// Player spawn, plane, ammo info. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnPlayerPlaneSpawn(MissionLogEventPlaneSpawn data)
		{
		}

		/// <summary>
		/// Airfield position and country info. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnAirfieldInfo(MissionLogEventAirfieldInfo data)
		{
		}

		/// <summary>
		/// Primary or secondary objective fail/success. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnObjectiveCompleted(MissionLogEventObjectiveCompleted data)
		{
		}

		/// <summary>
		/// Mission end. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnMissionEnd(MissionLogEventMissionEnd data)
		{
		}

		/// <summary>
		/// Landing. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnLanding(MissionLogEventLanding data)
		{
		}

		/// <summary>
		/// Take off. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnTakeOff(MissionLogEventTakeOff data)
		{
		}

		/// <summary>
		/// Sortie end, ammo info. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnPlayerMissionEnd(MissionLogEventPlayerAmmo data)
		{
		}

		/// <summary>
		/// Kill event. Rising twice for pilot and plane. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnKill(MissionLogEventKill data)
		{
		}

		/// <summary>
		/// Damage. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnDamage(MissionLogEventDamage data)
		{
		}

		/// <summary>
		/// Hit, ammo/explosion. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnHit(MissionLogEventHit data)
		{
		}

		/// <summary>
		/// Mission start. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnMissionStart(MissionLogEventStart data)
		{
		}

		/// <summary>
		/// Player join, GUIDs of nickname and user account. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnPlayerJoin(MissionLogEventPlayerJoin data)
		{
		}

		/// <summary>
		/// Player leave, GUIDs of nickname and user account. For live events only. Doesn't run on history events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnPlayerLeave(MissionLogEventPlayerLeave data)
		{
		}

		/// <summary>
		/// Execute an action on application exit.
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnApplicationShutdown(object data)
		{
		}

		/// <summary>
		/// Execute an action on application startup.
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnApplicationStartup(object data)
		{
		}

		/// <summary>
		/// Process unknown events
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnOther(object data)
		{
		}

		/// <summary>
		/// Process known and unknown events in single handler
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnAny(object data)
		{
		}

		/// <summary>
		/// Process offline events. Known and unknown
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnHistory(object data)
		{
		}

		/// <summary>
		/// Starts once when log parser starts
		/// </summary>
		/// <param name="server">Server object</param>
		public virtual void OnServerLogStart(Server server)
		{
		}

		/// <summary>
		/// Button click in script settings
		/// </summary>
		/// <param name="buttonName">Button name specified in config initializer</param>
		public virtual void OnButtonClick(string buttonName)
		{
		}

		/// <summary>
		/// Online player list was changed
		/// </summary>
		/// <param name="players"></param>       
		public virtual void OnPlayerListChange(Server server, List<Player> players)
		{
		}

		/// <summary>
		/// Server started (rcon isn't connected yet)
		/// </summary>
		/// <param name="server">Server object</param>
		public virtual void OnServerStart(Server server)
		{
		}

		/// <summary>
		/// Rcon connected, got server name
		/// </summary>
		/// <param name="server">Server object</param>
		public virtual void OnRconConnected(Server server)
		{
		}

		/// <summary>
		/// Server stopped
		/// </summary>
		/// <param name="server">Serve robject</param>
		public virtual void OnServerStop(Server server)
		{
		}


		/// <summary>
		/// Called for "Version" line in eventlog. This "Version line" is printed on top of every eventlog TXT file,
		/// which is created every 30 seconds. Thus, this event can be treated also as "HeartBeat" event... 
		/// </summary>
		/// <param name="data"></param>
		public virtual void OnVersion(MissionLogEventVersion data)
		{
		}

		/// <summary>
		/// Default config 
		/// </summary>
		public virtual ScriptConfig DefaultConfig =>
			new ScriptConfig() {
				IsEnabled = false,
				Title = "Example script",
				Description = "This script does nothing",
			};

		/// <summary>
		/// Script config
		/// </summary>
		public virtual ScriptConfig Config { get; set; }
	}
}