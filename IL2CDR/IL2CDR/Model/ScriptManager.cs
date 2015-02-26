using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSScriptLibrary;
using IL2CDR.Properties;

namespace IL2CDR.Model
{
    public class ScriptManager : IScriptManager
    {
        private List<object> scripts;
        private const string scriptsSubFolder = @"\Scripts";
        private static Random random = new Random();
        private static Dictionary<EventType, Action<IActionScript, object>> actionScripts = 
            new Dictionary<EventType, Action<IActionScript,object>>() {
            { EventType.MissionStart, (script,data) => { script.OnMissionStart(data as MissionLogEventStart); }},
            { EventType.Hit, (script,data) => {script.OnHit(data as MissionLogEventHit);}},
            { EventType.Damage, (script,data) => {script.OnDamage(data as MissionLogEventDamage);}},
            { EventType.Kill, (script,data) => {script.OnKill(data as MissionLogEventKill);}},
            { EventType.PlayerMissionEnd, (script,data) => { script.OnPlayerMissionEnd(data as MissionLogEventPlayerMissionEnd);}},
            { EventType.TakeOff, (script,data) => {script.OnTakeOff(data as MissionLogEventTakeOff);}},
            { EventType.Landing, (script,data) => {script.OnLanding(data as MissionLogEventLanding);}},
            { EventType.MissionEnd, (script,data) => {script.OnMissionEnd(data as MissionLogEventMissionEnd);}},
            { EventType.ObjectiveCompleted, (script,data) => {script.OnObjectiveCompleted(data as MissionLogEventObjectiveCompleted);}},
            { EventType.AirfieldInfo, (script,data) => {script.OnAirfieldInfo(data as MissionLogEventAirfieldInfo);}},
            { EventType.PlayerPlaneSpawn, (script,data) => {script.OnPlayerPlaneSpawn(data as MissionLogEventPlayerPlaneSpawn);}},
            { EventType.GroupInitInfo, (script,data) => {script.OnGroupInitInfo(data as MissionLogEventGroupInitInfo);}},
            { EventType.GameObjectSpawn, (script,data) => {script.OnGameObjectSpawn(data as MissionLogEventGameObjectSpawn);}},
            { EventType.InfluenceAreaInfo, (script,data) => {script.OnInfluenceAreaInfo(data as MissionLogEventInfluenceAreaInfo);}},
            { EventType.InfluenceAreaBoundary, (script,data) => {script.OnInfluenceAreaBoundary(data as MissionLogEventInfluenceAreaBoundary);}},
            { EventType.Version, (script,data) => {script.OnVersion(data as MissionLogEventVersion);}},
            { EventType.UserId, (script,data) => {script.OnUserId( data as MissionLogUserId);}},
        };

        public ScriptManager()
        {
            scripts = new List<object>();
        }


        public List<object> Scripts { get; set; }

        public void RunActionScripts( object data )
        {
            var header = data as MissionLogEventBase;
            if (header == null)
                return;

            foreach( IActionScript script in Scripts.Where(s => s is IActionScript && s is IScriptConfig) )
            {
                if (!(script as IScriptConfig).Config.Enabled)
                    continue;

                if (actionScripts.ContainsKey(header.Type))
                    actionScripts[header.Type](script, data);
                else
                    script.OnOther(data);
            }
        }

        public void LoadScripts()
        {
            string folder = AppDomain.CurrentDomain.GetData("DataDirectory") + scriptsSubFolder;

            var scriptFiles = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);
            foreach (var scriptPath in scriptFiles)
            {
                Util.Try(() => {
                    Log.WriteInfo("Loading script {0}...", scriptPath);
                    var scriptObject = CSScript.Evaluator.LoadFile(scriptPath);
                    scripts.Add(scriptObject);                
                });
            }
        }

        public List<IActionScript> ActionScripts
        {
            get
            {
                return Scripts.Where(script => script is IActionScript)
                    .Select(script => script as IActionScript).ToList();
            }
        }
    }
}
