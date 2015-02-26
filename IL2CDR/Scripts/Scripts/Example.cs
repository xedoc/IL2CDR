using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

namespace IL2CDR.Scripts
{
    public class Example : IActionScript, IScriptConfig
    {
        public ScriptConfig DefaultConfig
        {
            get {
                return new ScriptConfig() { 
                    Enabled = false,
                    Title = "Example script",
                    Description = "This script does nothing",
                };
            }
        }

        public ScriptConfig Config
        {
            get;
            set;
        }

        public void OnVersion(MissionLogEventVersion data)
        {
        }

        public void OnInfluenceAreaBoundary(MissionLogEventInfluenceAreaBoundary data)
        {
        }

        public void OnInfluenceAreaInfo(MissionLogEventInfluenceAreaInfo data)
        {
        }

        public void OnGameObjectSpawn(MissionLogEventGameObjectSpawn data)
        {
        }

        public void OnGroupInitInfo(MissionLogEventGroupInitInfo data)
        {
        }

        public void OnPlayerPlaneSpawn(MissionLogEventPlayerPlaneSpawn data)
        {            
        }

        public void OnAirfieldInfo(MissionLogEventAirfieldInfo data)
        {
        }

        public void OnObjectiveCompleted(MissionLogEventObjectiveCompleted data)
        {
        }

        public void OnMissionEnd(MissionLogEventMissionEnd data)
        {
        }

        public void OnLanding(MissionLogEventLanding data)
        {
        }

        public void OnTakeOff(MissionLogEventTakeOff data)
        {
        }

        public void OnPlayerMissionEnd(MissionLogEventMissionEnd data)
        {
        }

        public void OnKill(MissionLogEventKill data)
        {
        }

        public void OnDamage(MissionLogEventDamage data)
        {
        }

        public void OnHit(MissionLogEventHit data)
        {
        }

        public void OnMissionStart(MissionLogEventStart data)
        {
        }


        public void OnPlayerMissionEnd(MissionLogEventPlayerMissionEnd data)
        {
            
        }

        public void OnUserId(MissionLogUserId data)
        {
            
        }

        public void OnOther(object data)
        {
            
        }
    }
}
