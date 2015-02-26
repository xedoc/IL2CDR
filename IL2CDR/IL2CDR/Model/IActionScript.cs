using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public interface IActionScript
    {
        void OnVersion(MissionLogEventVersion data);
        void OnInfluenceAreaBoundary(MissionLogEventInfluenceAreaBoundary data);
        void OnInfluenceAreaInfo(MissionLogEventInfluenceAreaInfo data);
        void OnGameObjectSpawn(MissionLogEventGameObjectSpawn data);
        void OnGroupInitInfo(MissionLogEventGroupInitInfo data);
        void OnPlayerPlaneSpawn(MissionLogEventPlayerPlaneSpawn data);
        void OnAirfieldInfo(MissionLogEventAirfieldInfo data);
        void OnObjectiveCompleted(MissionLogEventObjectiveCompleted data);
        void OnMissionEnd(MissionLogEventMissionEnd data );
        void OnLanding(MissionLogEventLanding data);
        void OnTakeOff(MissionLogEventTakeOff data);
        void OnPlayerMissionEnd(MissionLogEventPlayerMissionEnd data );
        void OnKill(MissionLogEventKill data);
        void OnDamage(MissionLogEventDamage data);
        void OnHit(MissionLogEventHit data);
        void OnMissionStart( MissionLogEventStart data );
        void OnUserId(MissionLogUserId data);
        void OnOther(object data);

    }
}
