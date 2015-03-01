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
        private Config config;
        private const string scriptsSubFolder = @"\Scripts";
        private static Random random = new Random();
        private static Dictionary<EventType, Action<IActionScript, object>> actionScripts = 
            new Dictionary<EventType, Action<IActionScript,object>>() {
            { EventType.MissionStart, (script,data) => { script.OnMissionStart(data as MissionLogEventStart); }},
            { EventType.Hit, (script,data) => {script.OnHit(data as MissionLogEventHit);}},
            { EventType.Damage, (script,data) => {script.OnDamage(data as MissionLogEventDamage);}},
            { EventType.Kill, (script,data) => {script.OnKill(data as MissionLogEventKill);}},
            { EventType.PlayerAmmo, (script,data) => { script.OnPlayerMissionEnd(data as MissionLogEventPlayerAmmo);}},
            { EventType.TakeOff, (script,data) => {script.OnTakeOff(data as MissionLogEventTakeOff);}},
            { EventType.Landing, (script,data) => {script.OnLanding(data as MissionLogEventLanding);}},
            { EventType.MissionEnd, (script,data) => {script.OnMissionEnd(data as MissionLogEventMissionEnd);}},
            { EventType.ObjectiveCompleted, (script,data) => {script.OnObjectiveCompleted(data as MissionLogEventObjectiveCompleted);}},
            { EventType.AirfieldInfo, (script,data) => {script.OnAirfieldInfo(data as MissionLogEventAirfieldInfo);}},
            { EventType.PlaneSpawn, (script,data) => {script.OnPlayerPlaneSpawn(data as MissionLogEventPlaneSpawn);}},
            { EventType.GroupInit, (script,data) => {script.OnGroupInitInfo(data as MissionLogEventGroupInitInfo);}},
            { EventType.GameObjectSpawn, (script,data) => {script.OnGameObjectSpawn(data as MissionLogEventGameObjectSpawn);}},
            { EventType.InfluenceAreaInfo, (script,data) => {script.OnInfluenceAreaInfo(data as MissionLogEventInfluenceAreaInfo);}},
            { EventType.InfluenceAreaBoundary, (script,data) => {script.OnInfluenceAreaBoundary(data as MissionLogEventInfluenceAreaBoundary);}},
            { EventType.Version, (script,data) => {script.OnVersion(data as MissionLogEventVersion);}},
            { EventType.Disconnect, (script,data) => {script.OnUserId( data as MissionJoin);}},
        };

        public ScriptManager()
        {
            scripts = new List<object>();
            config = Settings.Default.Config;
        }


        public List<object> Scripts { get; set; }

        public void RunActionScripts( object data )
        {
            var header = data as MissionLogEventHeader;
            if (header == null)
                return;
            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach( IActionScript script in actScripts )
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
                var scriptFileName = Path.GetFileName(scriptPath);
                Util.Try(() => {
                    Log.WriteInfo("Loading script {0}...", scriptPath);
                    var scriptObject = CSScript.Evaluator.LoadFile(scriptPath);
                    if( !config.ScriptConfigs.Any( script => script.FileName.Equals( scriptFileName, StringComparison.InvariantCultureIgnoreCase )))
                    {
                        var scriptConfig = scriptObject as IScriptConfig;
                        if( scriptConfig != null )
                        {
                            var defaultConfig = scriptConfig.DefaultConfig;
                            defaultConfig.FileName = scriptFileName;                            
                            config.ScriptConfigs.Add(defaultConfig);
                        }
                    }

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
