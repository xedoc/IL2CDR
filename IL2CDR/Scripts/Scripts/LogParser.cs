﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

namespace IL2CDR.Scripts
{
    public class LogParser : ActionScriptBase
    {
        private string defaultIL2Dir = null;
        public LogParser()
        {
            defaultIL2Dir = GetIL2Directory();
            if (!String.IsNullOrEmpty(defaultIL2Dir))
                defaultIL2Dir = Path.Combine(defaultIL2Dir, @"data");
        }
        public override ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    IsEnabled = false,
                    Title = "Log Parser",
                    Description = "Parse specified mission and print results",
                    
                    ConfigFields = new ConfigFieldList()
                    {
                        //name, label, watermark, type, value, isVisible
                        { "LogParser_OpenMission", String.Empty, String.Empty, FieldType.Button, "Open mission report", true},                        
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
            var missionFileName = Dialogs.OpenFileDialog(defaultIL2Dir);

            if (String.IsNullOrWhiteSpace(missionFileName))
            {
                Log.WriteError("Mission log filename isn't specified!");
                return;
            }

            var dataService = new MissionLogDataService(Path.GetDirectoryName(missionFileName));

            dataService.Start();

            var sorties = from SortieStart in dataService.MissionHistory.Where(o => o is MissionLogEventPlaneSpawn)
                              .Select(o => o as MissionLogEventPlaneSpawn)
                              .Select(o => new { o.Player.SortieId, o.EventTime, o.Player.NickName, o.Player.Plane.Bullets })
                          join SortieEnd in dataService.MissionHistory.Where(o => o is MissionLogEventPlayerAmmo)
                              .Select(o => o as MissionLogEventPlayerAmmo)
                              .Select(o => new { o.Player.SortieId, o.EventTime, o.Player.NickName, o.Bullets })
                              on SortieStart.SortieId equals SortieEnd.SortieId
                              select new { SortieStart, SortieEnd };


            foreach (var item in sorties)
            {
                Log.WriteInfo("{0}\n\t\t\ttime: {1} - {2}\tbullets: {3} ->\t {4}", item.SortieStart.NickName, item.SortieStart.EventTime, item.SortieEnd.EventTime, item.SortieStart.Bullets, item.SortieEnd.Bullets );
            }
        }

        public override void OnButtonClick(string buttonName)
        {
            if (buttonName.Equals( "LogParser_OpenMission" ))
                OpenMissionReport();
        }

        public void ParseEvent(object data)
        {
            if (data == null)
                return;

            if (
                //data is MissionLogEventStart ||
                data is MissionLogEventMissionEnd ||
                //data is MissionLogEventKill ||
                data is MissionLogEventPlayerAmmo ||
                //data is MissionLogEventTakeOff ||
                //data is MissionLogEventLanding ||
                data is MissionLogEventPlaneSpawn ||
                //data is MissionLogEventGameObjectSpawn ||
                //data is MissionLogEventObjectiveCompleted ||
                !(data is MissionLogEventHeader)                
                )
            {

                if (data is MissionLogEventKill)
                {
                    var kill = data as MissionLogEventKill;
                    //Record kill only if player participate
                    if (kill.TargetPlayer == null && kill.AttackerPlayer == null)
                        return;
                }

                    
            }


        }

        public override void OnHistory(object data)
        {
        }
        

    }

}