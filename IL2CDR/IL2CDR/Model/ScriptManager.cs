using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CSScriptLibrary;
using IL2CDR.Properties;

namespace IL2CDR.Model
{
    public class ScriptManager : IScriptManager, IStopStart
    {
        private Config config;
        private List<ScriptItem> loadedScripts;
        private TextFileTracker tracker;
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
            config = Settings.Default.Config;
            loadedScripts = new List<ScriptItem>();
        }


        public List<object> Scripts { 
            get 
            {
                return loadedScripts.Select(s => s.Script).ToList();
            } 
        }
        public void RunServerLogStartScripts(Server server)
        {
            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach (IActionScript script in actScripts)
            {
                if (!(script as IScriptConfig).Config.IsEnabled)
                    continue;

                script.OnServerLogStart(server);
            }
        }
        public void RunStartupMethod()
        {
            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach (IActionScript script in actScripts)
            {
                if (!(script as IScriptConfig).Config.IsEnabled)
                    continue;

                script.OnApplicationStartup( Application.Current as App );
            }
        }
        public void RunShutdownMethod()
        {
            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach (IActionScript script in actScripts)
                script.OnApplicationShutdown( Application.Current as App );
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

        public void ProcessButtonClick( string name )
        {
            var actScripts = Scripts.Where(s => s is IActionScript && s is IScriptConfig);
            foreach (IActionScript script in actScripts)
            {
                if (!(script as IScriptConfig).Config.IsEnabled)
                    continue;

                script.OnButtonClick(name);
            }
        }

        private void DisposeScriptAssembly(string fileName)
        {
                var asm = loadedScripts.FirstOrDefault(script => script.Path.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                    .With(x => x.AsmHelper);
                if (asm != null)
                {
                    loadedScripts.RemoveAll(s => s.Path.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
                    Util.Try(() => asm.Dispose());
                }

        }
        public void SwitchScript(string scriptPath)
        {
            var savedScriptConfig = GetScriptConfig(scriptPath);
            if( savedScriptConfig != null )
            {
                if (savedScriptConfig.IsEnabled)
                    LoadScript(scriptPath);
                else
                    UnloadScript(scriptPath);
            }

        }
        public void UnloadScript(string scriptPath)
        {
            Log.WriteInfo("Unloading script {0}...", scriptPath);
            DisposeScriptAssembly(scriptPath);
        }
        public object LoadScript(string scriptPath)
        {
            var curTime = File.GetLastWriteTime(scriptPath).ToString();
            if (loadedScripts.Any(script => script.Path.Equals(scriptPath, StringComparison.InvariantCultureIgnoreCase) && curTime == script.Id))
                return null;

            Log.WriteInfo("Loading script {0}...", scriptPath);

            DisposeScriptAssembly(scriptPath);
            var assemblyPath = Path.Combine(Path.GetDirectoryName(scriptPath), Path.GetFileNameWithoutExtension(scriptPath) + ".dll");
            AsmHelper asmHelper = new AsmHelper(CSScript.LoadCodeFrom(scriptPath, null, false, null));
            var obj = asmHelper.CreateObject("*");

            IScriptConfig configObj = obj as IScriptConfig;
            if (configObj != null)
                configObj.Config = GetScriptConfig(scriptPath, obj);

            loadedScripts.Add(new ScriptItem(
                scriptPath, 
                File.GetLastWriteTime(scriptPath).ToString(), 
                asmHelper, 
                obj
                ));

            return obj;
        }

        public ScriptConfig GetScriptConfig(string scriptPath, object scriptObject = null )
        {
            var savedScriptConfig = config.ScriptConfigs.FirstOrDefault(script => 
                        script.FileName.Equals(scriptPath, StringComparison.InvariantCultureIgnoreCase) ||
                        script.FileName.Equals(Path.GetFileName(scriptPath), StringComparison.InvariantCultureIgnoreCase ));

            if (scriptObject == null)
                return savedScriptConfig;

            var scriptConfig = scriptObject as IScriptConfig;
            var defaultConfig = scriptConfig.With(x => x.DefaultConfig);
            if (defaultConfig == null)
                defaultConfig = new ScriptConfig();

            if (savedScriptConfig == null && scriptConfig != null )
            {
                defaultConfig.FileName = scriptPath;
                config.ScriptConfigs.Add(defaultConfig);
                (scriptObject as IScriptConfig).Config = defaultConfig;
                return defaultConfig;
            }
            else
            {
                (scriptObject as IScriptConfig).Config = savedScriptConfig;
                var oldFields = savedScriptConfig.ConfigFields.Select(f => f.Name).Except(defaultConfig.ConfigFields.Select(f => f.Name));
                var newFields = defaultConfig.ConfigFields.Select(f => f.Name).Except(savedScriptConfig.ConfigFields.Select(f => f.Name));
                foreach (var oldField in oldFields.ToList())
                {
                    savedScriptConfig.ConfigFields.RemoveAll(f => f.Name.Equals(oldField));
                }

                foreach (var newField in newFields.ToList())
                {
                    defaultConfig.ConfigFields.FirstOrDefault(f => f.Name.Equals(newField)).Do(
                        x => savedScriptConfig.ConfigFields.Add(x));
                }
                return savedScriptConfig;
            }
            
        }
        public void LoadScripts()
        {
            string folder = AppDomain.CurrentDomain.GetData("DataDirectory") + scriptsSubFolder;
            if( !Directory.Exists( folder ))
            {
                folder = @".\Scripts\";
            }

            var scriptFiles = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);

            config.ScriptConfigs.RemoveAll(script => 
                !scriptFiles.Contains(script.FileName) && 
                !scriptFiles.Contains(Path.Combine(folder,script.FileName)));

            foreach (var scriptPath in scriptFiles)
            {
                var scriptFileName = Path.GetFileName(scriptPath);
                Util.Try(() => {
                    LoadScript(scriptPath);  
                });
            }
            tracker = new TextFileTracker(folder, "*.cs");
            tracker.OnChanged = (fileName) =>
            {
                Util.Try(() =>
                {
                    Thread.Sleep(1000);
                    LoadScript(fileName);
                });
            };
            tracker.OnFileCreation = (fileName) =>
            {
                Util.Try(() =>
                {
                    Thread.Sleep(1000);
                    LoadScript(fileName);
                });
            };

            tracker.Start();

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
            if(tracker != null )
                tracker.Stop();
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
    public class ScriptItem
    {
        public ScriptItem(string name, string id, AsmHelper asm, object scriptObject)
        {
            Path = name;
            Id = id;
            AsmHelper = asm;
            Script = scriptObject;
        }
        public string Path { get; set; }
        public string Id { get; set; }
        public AsmHelper AsmHelper { get; set; }
        public object Script { get; set; }

    }
    public static class InterfaceExtensions
    {
        public static T TryAlignToInterface<T>(this AsmHelper helper, object obj) where T : class
        {
            try
            {
                return helper.AlignToInterface<T>(obj);
            }
            catch
            {
                return null;
            }
        }
    }
}
