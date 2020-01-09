﻿using System.Collections.Generic;

namespace IL2CDR.Model
{
	public interface IActionScript
	{
		/// <summary>
		/// Called for "Version" line in eventlog. This "Version line" is printed on top of every eventlog TXT file,
		/// which is created every 30 seconds. Thus, this event can be treated also as "HeartBeat" event... 
		/// </summary>
		/// <param name="data"></param>
		void OnVersion(MissionLogEventVersion data);


		void OnInfluenceAreaBoundary(MissionLogEventInfluenceAreaBoundary data);
		void OnInfluenceAreaInfo(MissionLogEventInfluenceAreaInfo data);
		void OnGameObjectSpawn(MissionLogEventGameObjectSpawn data);
		void OnGroupInitInfo(MissionLogEventGroupInitInfo data);
		void OnPlayerPlaneSpawn(MissionLogEventPlaneSpawn data);
		void OnAirfieldInfo(MissionLogEventAirfieldInfo data);
		void OnObjectiveCompleted(MissionLogEventObjectiveCompleted data);
		void OnMissionEnd(MissionLogEventMissionEnd data);
		void OnLanding(MissionLogEventLanding data);
		void OnTakeOff(MissionLogEventTakeOff data);
		void OnPlayerMissionEnd(MissionLogEventPlayerAmmo data);
		void OnKill(MissionLogEventKill data);
		void OnDamage(MissionLogEventDamage data);
		void OnHit(MissionLogEventHit data);
		void OnMissionStart(MissionLogEventStart data);
		void OnPlayerJoin(MissionLogEventPlayerJoin data);
		void OnPlayerLeave(MissionLogEventPlayerLeave data);
		void OnApplicationShutdown(object data);
		void OnApplicationStartup(object data);
		void OnOther(object data);
		void OnAny(object data);
		void OnHistory(object data);
		void OnServerLogStart(Server server);
		void OnButtonClick(string buttonName);
		void OnPlayerListChange(Server server, List<Player> players);
		void OnServerStart(Server server);
		void OnServerStop(Server server);
		void OnRconConnected(Server server);
	}
}