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
    public class ScriptManager : IScriptManager, IStopStart
    {
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
            //{ EventType.Version, (script,data) => {script.OnVersion(data as MissionLogEventVersion);}},
            { EventType.Join, (script,data) => {script.OnPlayerJoin(data as MissionLogEventPlayerJoin);}},
            { EventType.Leave, (script,data) => {script.OnPlayerLeave(data as MissionLogEventPlayerLeave);}},
        };

        public ScriptManager()
        {
            CSScript.AssemblyResolvingEnabled = true;  
            Scripts = new List<object>();
            config = Settings.Default.Config;
        }


        public List<object> Scripts { get; set; }
        public void RunServerLogStartScripts(Server server)
        {
            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach (IActionScript script in actScripts)
                script.OnServerLogStart(server);
        }
        public void RunStartupMethod()
        {
            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach (IActionScript script in actScripts)
                script.OnApplicationStartup(null);
        }
        public void RunShutdownMethod()
        {
            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach (IActionScript script in actScripts)
                script.OnApplicationShutdown(null);
        }

        public void RunActionScripts( object data )
        {
            var header = data as MissionLogEventHeader;
            if (header == null)
                return;

            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach( IActionScript script in actScripts )
            {
                if (!(script as IScriptConfig).Config.IsEnabled)
                    continue;

                if (actionScripts.ContainsKey(header.Type))
                    actionScripts[header.Type](script, data);
                else
                    script.OnOther(data);
                
                script.OnAny(data);
            }
        }

        public void RunHistoryScripts( object data )
        {
            var header = data as MissionLogEventHeader;
            if (header == null)
                return;

            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach (IActionScript script in actScripts)
            {
                if (!(script as IScriptConfig).Config.IsEnabled)
                    continue;

                script.OnHistory(data);
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
                    var savedScriptConfig = config.ScriptConfigs.FirstOrDefault( script => script.FileName.Equals( scriptFileName, StringComparison.InvariantCultureIgnoreCase ));                    
                    var scriptConfig = scriptObject as IScriptConfig;
                    var defaultConfig = scriptConfig.With( x => x.DefaultConfig);
                    if (defaultConfig == null)
                        defaultConfig = new ScriptConfig();

                    if (savedScriptConfig == null)
                    {
                        if (scriptConfig != null)
                        {
                            defaultConfig.FileName = scriptFileName;
                            config.ScriptConfigs.Add(defaultConfig);
                            (scriptObject as IScriptConfig).Config = defaultConfig;
                        }
                    }
                    else
                    {
                        (scriptObject as IScriptConfig).Config = savedScriptConfig;
                        var oldFields = savedScriptConfig.ConfigFields.Select(f => f.Name).Except(defaultConfig.ConfigFields.Select(f => f.Name));
                        var newFields = defaultConfig.ConfigFields.Select(f => f.Name).Except(savedScriptConfig.ConfigFields.Select(f => f.Name));
                        foreach (var oldField in oldFields)
                        {
                            savedScriptConfig.ConfigFields.RemoveAll(f => f.Name.Equals(oldField));
                        }
                        
                        foreach (var newField in newFields)
                        {
                            defaultConfig.ConfigFields.FirstOrDefault(f => f.Name.Equals(newField)).Do(
                                x => savedScriptConfig.ConfigFields.Add(x));                            
                        }

                    }

                    Scripts.Add(scriptObject);                
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

        public void Start()
        {
            RunStartupMethod();
        }

        public void Stop()
        {
            RunShutdownMethod();
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
