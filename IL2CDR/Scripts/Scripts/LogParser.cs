using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

//namespace IL2CDR.Scripts
//{
	public class LogParser : ActionScriptBase
	{
		private string defaultIL2Dir = null;
		public List<object> History { get; set; }

		public LogParser()
		{
			this.defaultIL2Dir = this.GetIL2Directory();
			if (!string.IsNullOrEmpty(this.defaultIL2Dir)) {
				this.defaultIL2Dir = Path.Combine(this.defaultIL2Dir, @"data");
			}
		}

		public override ScriptConfig DefaultConfig
		{
			get
			{
				return new ScriptConfig() {
					IsEnabled = false,
					Title = "Log Parser",
					Description = "Parse specified mission and print results",

					ConfigFields = new ConfigFieldList() {
						//name, label, watermark, type, value, isVisible
						{
							"LogParser_OpenMission", string.Empty, string.Empty, FieldType.Button, "Open mission report",
							true
						},
					},
				};

			}
		}


		public string GetIL2Directory()
		{
			var title = "IL-2 Sturmovik Battle";
			return Installer.GetDirectoryByDisplayName(title);
		}

		public void OpenMissionReport()
		{
			var missionFileName = Dialogs.OpenFileDialog(this.defaultIL2Dir);

			if (string.IsNullOrWhiteSpace(missionFileName)) {
				Log.WriteError("Mission log filename isn't specified!");
				return;
			}

			var dataService = new MissionLogDataService(Path.GetDirectoryName(missionFileName));

			dataService.Start();
			this.History = dataService.MissionHistory;

			//var sorties = from SortieStart in dataService.MissionHistory.Where(o => o is MissionLogEventPlaneSpawn)
			//                  .Select(o => o as MissionLogEventPlaneSpawn)
			//                  .Select(o => new { o.Player.SortieId, o.EventTime, o.Player.NickName, o.Player.Plane.Bullets })
			//              join SortieEnd in dataService.MissionHistory.Where(o => o is MissionLogEventPlayerAmmo)
			//                  .Select(o => o as MissionLogEventPlayerAmmo)
			//                  .Select(o => new { o.Player.SortieId, o.EventTime, o.Player.NickName, o.Bullets })
			//                  on SortieStart.SortieId equals SortieEnd.SortieId
			//                  select new { SortieStart, SortieEnd };


			//foreach (var item in sorties)
			//{
			//    Log.WriteInfo("{0}\n\t\t\ttime: {1} - {2}\tbullets: {3} ->\t {4}", item.SortieStart.NickName, item.SortieStart.EventTime, item.SortieEnd.EventTime, item.SortieStart.Bullets, item.SortieEnd.Bullets );
			//}
		}

		public override void OnButtonClick(string buttonName)
		{
			if (buttonName.Equals("LogParser_OpenMission")) {
				this.OpenMissionReport();
			}
		}

		public void ParseEvent(object data)
		{
			if (data == null) {
				return;
			}

			if (data is MissionLogEventStart ||
				data is MissionLogEventMissionEnd ||
				data is MissionLogEventKill ||
				data is MissionLogEventPlayerAmmo ||
				data is MissionLogEventTakeOff ||
				data is MissionLogEventLanding ||
				data is MissionLogEventPlaneSpawn ||
				data is MissionLogEventGameObjectSpawn ||
				data is MissionLogEventObjectiveCompleted ||
				data is MissionLogEject ||
				data is MissionLogRemoveBot ||
				!(data is MissionLogEventHeader)) {
				Log.WriteInfo((data as MissionLogEventHeader)?.Type.ToString());

				if (data is MissionLogEventKill kill) {
					//Record kill only if player participate
					if (kill.TargetPlayer == null && kill.AttackerPlayer == null) {
						return;
					}
				}
			}
		}

		public override void OnHistory(object data)
		{
			if (data is MissionLogEventHeader eventHeader) {
				Log.WriteInfo(eventHeader.Type.ToString());
			}
		}
	}
//}