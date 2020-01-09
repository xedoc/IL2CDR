using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR.Model;

//namespace IL2CDR.Scripts
//{
public class SimpleEventLogger : IL2CDR.Model.ActionScriptBase
{

	/// <summary>
	/// This property will be requested by Script Manager when server on script initialization
	/// </summary>
	public override ScriptConfig DefaultConfig {
		get {
			return new ScriptConfig() {
				IsEnabled = true,
				Title = "Simple Event Logger",
				Description = "This script logs each intercepted event. This can be used to monitor, if the communication with the DServer is fully functional.",

				//field name must be unique
				ConfigFields = new ConfigFieldList() {
					//name, label, watermark, type, value, isVisible
					{
						"FooText", "Example Text config field", "foo-text-placeholder",
						FieldType.Text, "foo text", true
					}, {
						"FooInt", "Example Integer config field", "foo-int-placeholder",
						FieldType.Number, 33, true
					},
				},
			};

		}
	}




	public override void OnHistory(object data)
	{
		var eventData = data as MissionLogEventHeader;
		Log.WriteInfo("New Event: {0} - {1}", "OnHistory", eventData?.Type);
	}

	public override void OnOther(object data)
	{
		var eventData = data as MissionLogEventHeader;
		Log.WriteInfo("New Event: {0} - {1}", "OnOther", eventData?.Type);
	}

	public override void OnAny(object data)
	{
		var eventData = data as MissionLogEventHeader; 
		Log.WriteInfo("New Event: {0} - {1}", "OnAny", eventData?.Type);
	}

	public override void OnAirfieldInfo(MissionLogEventAirfieldInfo data)
	{
		Log.WriteInfo("New Event: {0}", "OnAirfieldInfo");
	}



	public override void OnApplicationShutdown(object data)
	{
		Log.WriteInfo("New Event: {0}", "OnApplicationShutdown");
	}

	public override void OnApplicationStartup(object data)
	{
		Log.WriteInfo("New Event: {0}", "OnApplicationStartup");
	}

	public override void OnButtonClick(string buttonName)
	{
		Log.WriteInfo("New Event: {0}", "OnButtonClick");
	}

	public override void OnDamage(MissionLogEventDamage data)
	{
		Log.WriteInfo("New Event: {0}", "OnDamage");
	}

	public override void OnGameObjectSpawn(MissionLogEventGameObjectSpawn data)
	{
		Log.WriteInfo("New Event: {0}", "OnGameObjectSpawn");
	}

	public override void OnGroupInitInfo(MissionLogEventGroupInitInfo data)
	{
		Log.WriteInfo("New Event: {0}", "OnGroupInitInfo");
	}

	public override void OnHit(MissionLogEventHit data)
	{
		Log.WriteInfo("New Event: {0}", "OnHit");
	}

	public override void OnInfluenceAreaBoundary(MissionLogEventInfluenceAreaBoundary data)
	{
		Log.WriteInfo("New Event: {0}", "OnInfluenceAreaBoundary");
	}

	public override void OnInfluenceAreaInfo(MissionLogEventInfluenceAreaInfo data)
	{
		Log.WriteInfo("New Event: {0}", "OnInfluenceAreaInfo");
	}

	public override void OnKill(MissionLogEventKill data)
	{
		Log.WriteInfo("New Event: {0}", "OnKill");
	}

	public override void OnLanding(MissionLogEventLanding data)
	{
		Log.WriteInfo("New Event: {0}", "OnLanding");
	}

	public override void OnMissionEnd(MissionLogEventMissionEnd data)
	{
		Log.WriteInfo("New Event: {0}", "OnMissionEnd");
	}

	public override void OnMissionStart(MissionLogEventStart data)
	{
		Log.WriteInfo("New Event: {0}", "OnMissionStart");
	}

	public override void OnObjectiveCompleted(MissionLogEventObjectiveCompleted data)
	{
		Log.WriteInfo("New Event: {0}", "OnObjectiveCompleted");
	}

	public override void OnPlayerJoin(MissionLogEventPlayerJoin data)
	{
		Log.WriteInfo("New Event: {0}", "OnPlayerJoin");
	}

	public override void OnPlayerLeave(MissionLogEventPlayerLeave data)
	{
		Log.WriteInfo("New Event: {0}", "OnPlayerLeave");
	}

	public override void OnPlayerListChange(Server server, List<Player> players)
	{
		Log.WriteInfo("New Event: {0}", "OnPlayerListChange");
	}

	public override void OnPlayerMissionEnd(MissionLogEventPlayerAmmo data)
	{
		Log.WriteInfo("New Event: {0}", "OnPlayerMissionEnd");
	}

	public override void OnPlayerPlaneSpawn(MissionLogEventPlaneSpawn data)
	{
		Log.WriteInfo("New Event: {0}", "OnPlayerPlaneSpawn");
	}

	public override void OnRconConnected(Server server)
	{
		Log.WriteInfo("New Event: {0}", "OnRconConnected");
	}

	public override void OnServerLogStart(Server server)
	{
		Log.WriteInfo("New Event: {0}", "OnServerLogStart");
	}

	public override void OnServerStart(Server server)
	{
		Log.WriteInfo("New Event: {0}", "OnServerStart");
	}

	public override void OnServerStop(Server server)
	{
		Log.WriteInfo("New Event: {0}", "OnServerStop");
	}

	public override void OnTakeOff(MissionLogEventTakeOff data)
	{
		Log.WriteInfo("New Event: {0}", "OnTakeOff");
	}

	public override void OnVersion(MissionLogEventVersion data)
	{
		Log.WriteInfo("New Event: {0}", "OnVersion");
	}
}


//}
