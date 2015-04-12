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
        virtual public void OnInfluenceAreaBoundary(MissionLogEventInfluenceAreaBoundary data){}
        /// <summary>
        /// Area country info. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnInfluenceAreaInfo(MissionLogEventInfluenceAreaInfo data){}
        /// <summary>
        /// Game object spawn (bots,ground vehicles etc). For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnGameObjectSpawn(MissionLogEventGameObjectSpawn data){}
        /// <summary>
        /// Formation/group information (member IDs). For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnGroupInitInfo(MissionLogEventGroupInitInfo data){}
        /// <summary>
        /// Player spawn, plane, ammo info. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnPlayerPlaneSpawn(MissionLogEventPlaneSpawn data){}
        /// <summary>
        /// Airfield position and country info. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnAirfieldInfo(MissionLogEventAirfieldInfo data){}
        /// <summary>
        /// Primary or secondary objective fail/success. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnObjectiveCompleted(MissionLogEventObjectiveCompleted data){}
        /// <summary>
        /// Mission end. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnMissionEnd(MissionLogEventMissionEnd data){}
        /// <summary>
        /// Landing. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnLanding(MissionLogEventLanding data){}
        /// <summary>
        /// Take off. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnTakeOff(MissionLogEventTakeOff data){}
        /// <summary>
        /// Sortie end, ammo info. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnPlayerMissionEnd(MissionLogEventPlayerAmmo data){}
        /// <summary>
        /// Kill event. Rising twice for pilot and plane. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnKill(MissionLogEventKill data){}
        /// <summary>
        /// Damage. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnDamage(MissionLogEventDamage data){}
        /// <summary>
        /// Hit, ammo/explosion. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnHit(MissionLogEventHit data){}
        /// <summary>
        /// Mission start. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnMissionStart(MissionLogEventStart data){}
        /// <summary>
        /// Player join, GUIDs of nickname and user account. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnPlayerJoin(MissionLogEventPlayerJoin data) { }
        /// <summary>
        /// Player leave, GUIDs of nickname and user account. For live events only. Doesn't run on history events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnPlayerLeave(MissionLogEventPlayerLeave data) { }
        /// <summary>
        /// Execute an action on application exit.
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnApplicationShutdown(object data) { }
        /// <summary>
        /// Execute an action on application startup.
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnApplicationStartup(object data) { }
        /// <summary>
        /// Process unknown events
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnOther(object data) { }
        /// <summary>
        /// Process known and unknown events in single handler
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnAny(object data) { }
        /// <summary>
        /// Process offline events. Known and unknown
        /// </summary>
        /// <param name="data"></param>
        virtual public void OnHistory(object data) { }
        /// <summary>
        /// Starts once when log parser starts
        /// </summary>
        /// <param name="server">Server object</param>
        virtual public void OnServerLogStart(Server server) { }
        /// <summary>
        /// Button click in script settings
        /// </summary>
        /// <param name="buttonName">Button name specified in config initializer</param>
        virtual public void OnButtonClick(string buttonName) { }
        /// <summary>
        /// Online player list was changed
        /// </summary>
        /// <param name="players"></param>
        
        virtual public void OnPlayerListChange( List<Player> players ){ }


        /// <summary>
        /// Default config 
        /// </summary>
        virtual public ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    IsEnabled = false,
                    Title = "Example script",
                    Description = "This script does nothing",
                };
            }
        }

        /// <summary>
        /// Script config
        /// </summary>
        virtual public ScriptConfig Config
        {
            get;
            set;
        }
    }
}
