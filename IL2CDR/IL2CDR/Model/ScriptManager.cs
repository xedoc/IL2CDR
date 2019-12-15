using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using CSScriptLibrary;
using IL2CDR.Properties;

namespace IL2CDR.Model
{


	public class ScriptManager : IScriptManager, IStopStart
	{
		private readonly Config config;
		private readonly List<ScriptItem> loadedScripts;
		private TextFileTracker tracker;
		private const string SCRIPTS_SUB_FOLDER = @"\Scripts";

		private static readonly Random random = new Random();

		private static Dictionary<EventType, Action<IActionScript, object>> actionScripts =
			new Dictionary<EventType, Action<IActionScript, object>>() {
				{EventType.MissionStart, (script, data) => { script.OnMissionStart(data as MissionLogEventStart); }},
				{EventType.Hit, (script, data) => { script.OnHit(data as MissionLogEventHit); }},
				{EventType.Damage, (script, data) => { script.OnDamage(data as MissionLogEventDamage); }},
				{EventType.Kill, (script, data) => { script.OnKill(data as MissionLogEventKill); }},
				{EventType.PlayerAmmo, (script, data) => { script.OnPlayerMissionEnd(data as MissionLogEventPlayerAmmo); }},
				{EventType.TakeOff, (script, data) => { script.OnTakeOff(data as MissionLogEventTakeOff); }},
				{EventType.Landing, (script, data) => { script.OnLanding(data as MissionLogEventLanding); }},
				{EventType.MissionEnd, (script, data) => { script.OnMissionEnd(data as MissionLogEventMissionEnd); }},
				{EventType.ObjectiveCompleted,
					(script, data) => { script.OnObjectiveCompleted(data as MissionLogEventObjectiveCompleted); }},
				{EventType.AirfieldInfo, (script, data) => { script.OnAirfieldInfo(data as MissionLogEventAirfieldInfo); }},
				{EventType.PlaneSpawn, (script, data) => { script.OnPlayerPlaneSpawn(data as MissionLogEventPlaneSpawn); }},
				{EventType.GroupInit, (script, data) => { script.OnGroupInitInfo(data as MissionLogEventGroupInitInfo); }},
				{EventType.GameObjectSpawn, (script, data) => { script.OnGameObjectSpawn(data as MissionLogEventGameObjectSpawn); }},
				{EventType.InfluenceAreaInfo, (script, data) => { script.OnInfluenceAreaInfo(data as MissionLogEventInfluenceAreaInfo); }},
				{EventType.InfluenceAreaBoundary, (script, data) => { script.OnInfluenceAreaBoundary(data as MissionLogEventInfluenceAreaBoundary); }},
				{ EventType.Version, (script,data) => {script.OnVersion(data as MissionLogEventVersion);}},
				{EventType.Join, (script, data) => { script.OnPlayerJoin(data as MissionLogEventPlayerJoin); }},
				{EventType.Leave, (script, data) => { script.OnPlayerLeave(data as MissionLogEventPlayerLeave); }},
			};





		public ScriptManager()
		{
			CSScript.AssemblyResolvingEnabled = true;
			CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Roslyn;

			this.config = Settings.Default.Config;
			this.loadedScripts = new List<ScriptItem>();
		}


		public List<object> Scripts
		{
			get { return this.loadedScripts.Select(s => s.Script).ToList(); }
		}

		public void RunRconConnectedScripts(Server server)
		{
			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				if (!((IScriptConfig) script).Config.IsEnabled) {
					continue;
				}

				script.OnRconConnected(server);
			}
		}

		public void RunServerStopScripts(Server server)
		{
			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				if (!((IScriptConfig) script).Config.IsEnabled) {
					continue;
				}

				script.OnServerStop(server);
			}
		}

		public void RunServerStartScripts(Server server)
		{
			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				if (!((IScriptConfig) script).Config.IsEnabled) {
					continue;
				}

				script.OnServerStart(server);
			}
		}

		public void RunServerLogStartScripts(Server server)
		{
			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				if (!((IScriptConfig) script).Config.IsEnabled) {
					continue;
				}

				script.OnServerLogStart(server);
			}
		}

		public void RunStartupMethod()
		{
			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				if (!((IScriptConfig) script).Config.IsEnabled) {
					continue;
				}

				script.OnApplicationStartup(Application.Current as App);
			}
		}

		public void RunShutdownMethod()
		{
			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				script.OnApplicationShutdown(Application.Current as App);
			}
		}

		public void RunActionScripts(object data)
		{
			var header = data as MissionLogEventHeader;
			if (header == null) {
				return;
			}

			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				if (!((IScriptConfig) script).Config.IsEnabled) {
					continue;
				}

				if (actionScripts.ContainsKey(header.Type)) {
					actionScripts[header.Type](script, data);
				} else {
					script.OnOther(data);
				}

				script.OnAny(data);
			}
		}

		public void RunHistoryScripts(object data)
		{
			var header = data as MissionLogEventHeader;
			if (header == null) {
				return;
			}

			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				if (!((IScriptConfig) script).Config.IsEnabled) {
					continue;
				}

				script.OnHistory(data);
			}
		}

		public void ProcessButtonClick(string name)
		{
			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				if (!((IScriptConfig) script).Config.IsEnabled) {
					continue;
				}

				script.OnButtonClick(name);
			}
		}

		public void ProcessPlayerListChange(Server server, List<Player> players)
		{
			if (server == null || players == null) {
				return;
			}

			var actScripts = this.Scripts.Where(s => s is IActionScript && s is IScriptConfig);
			foreach (IActionScript script in actScripts) {
				if (!((IScriptConfig) script).Config.IsEnabled) {
					continue;
				}

				script.OnPlayerListChange(server, players);
			}
		}

		private void DisposeScriptAssembly(string fileName)
		{
			var asm = this.loadedScripts.FirstOrDefault(script => script.Path.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))?.AsmHelper;
										
			if (asm != null) {
				this.loadedScripts.RemoveAll(s => s.Path.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
				Util.Try(() => asm.Dispose());
			}
		}

		public void SwitchScript(string scriptPath)
		{
			var savedScriptConfig = this.GetScriptConfig(scriptPath);
			if (savedScriptConfig != null) {
				if (savedScriptConfig.IsEnabled) {
					this.LoadScript(scriptPath);
				} else {
					this.UnloadScript(scriptPath);
				}
			}
		}

		public void UnloadScript(string scriptPath)
		{
			Log.WriteInfo("Unloading script {0}...", scriptPath);
			this.DisposeScriptAssembly(scriptPath);
		}

		public object LoadScript(string scriptPath)
		{
			if (scriptPath == null) {
				throw new ArgumentNullException(nameof(scriptPath));
			}

			if (!scriptPath.Contains(":")) {
				var folder = AppDomain.CurrentDomain.GetData("DataDirectory") + SCRIPTS_SUB_FOLDER;
				scriptPath = $@"{folder}\{scriptPath}";
			}


			var curTime = File.GetLastWriteTime(scriptPath).ToString(CultureInfo.InvariantCulture);
			if ( this.loadedScripts.Any(script => script.Path.Equals(scriptPath, StringComparison.InvariantCultureIgnoreCase) && curTime == script.Id) ) {
				return null;
			}

			Log.WriteInfo("Loading script {0}...", scriptPath);

			this.DisposeScriptAssembly(scriptPath);
			var assemblyPath = Path.Combine(Path.GetDirectoryName(scriptPath), Path.GetFileNameWithoutExtension(scriptPath) + ".dll");


			object obj = CSScript.Evaluator.LoadFile(scriptPath);
			if (obj is ActionScriptBase) {
				Log.WriteInfo("Script {0} is correctly an instance of ActionScriptBase!", scriptPath);
			}


			//var asmHelper = new AsmHelper(CSScript.LoadCodeFrom(scriptPath, null, false, null));
			//var asmHelper = new AsmHelper(CSScript.LoadFile(scriptPath));
			//var obj = asmHelper.CreateObject("*");

			if (obj is IScriptConfig configObj) {
				configObj.Config = this.GetScriptConfig(scriptPath, obj);
			}

			this.loadedScripts.Add(new ScriptItem(
				scriptPath,
				File.GetLastWriteTime(scriptPath).ToString(CultureInfo.InvariantCulture),
				null, //asmHelper,  <-- from 25.11.2019, the compilation switched from CSScript.LoadFile to CSScript.Evaluator.LoadFile, which need not the AsmHelper... 
				obj
			));

			return obj;
		}

		public ScriptConfig GetScriptConfig(string scriptPath, object scriptObject = null)
		{
			var savedScriptConfig = this.config.ScriptConfigs
												.FirstOrDefault(script => script.FileName.Equals(scriptPath, StringComparison.InvariantCultureIgnoreCase) 
																		|| script.FileName.Equals(Path.GetFileName(scriptPath), StringComparison.InvariantCultureIgnoreCase)
									);

			if (scriptObject == null) {
				return savedScriptConfig;
			}

			var scriptConfig = scriptObject as IScriptConfig;
			if (scriptConfig == null) {
				// -- What to do? 
				return savedScriptConfig; 
			}


			var defaultConfig = scriptConfig.With(x => x.DefaultConfig) ?? new ScriptConfig();

			if (savedScriptConfig == null) {
				defaultConfig.FileName = scriptPath;
				this.config.ScriptConfigs.Add(defaultConfig);
				scriptConfig.Config = defaultConfig;
				return defaultConfig;
			}


			scriptConfig.Config = savedScriptConfig;
			var oldFields = savedScriptConfig.ConfigFields.Select(f => f.Name)
				.Except(defaultConfig.ConfigFields.Select(f => f.Name));
			var newFields = defaultConfig.ConfigFields.Select(f => f.Name)
				.Except(savedScriptConfig.ConfigFields.Select(f => f.Name));
			foreach (var oldField in oldFields.ToList()) {
				savedScriptConfig.ConfigFields.RemoveAll(f => f.Name.Equals(oldField));
			}

			foreach (var newField in newFields.ToList()) {
				defaultConfig.ConfigFields.FirstOrDefault(f => f.Name.Equals(newField)).Do(
					x => savedScriptConfig.ConfigFields.Add(x));
			}

			return savedScriptConfig;
		}

		public void LoadScripts()
		{
			var folder = AppDomain.CurrentDomain.GetData("DataDirectory") + SCRIPTS_SUB_FOLDER;
			if (!Directory.Exists(folder)) {
				folder = @".\Scripts\";
			}

			var scriptFiles = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories)
				.Where(path => !Path.GetFileName(path).StartsWith("."))
				.ToList();

			this.config.ScriptConfigs.RemoveAll(script =>
														!scriptFiles.Contains(script.FileName) &&
														!scriptFiles.Contains(Path.Combine(folder, script.FileName)));

			foreach (var scriptPath in scriptFiles) {
				var scriptFileName = Path.GetFileName(scriptPath);
				Util.Try(() => { this.LoadScript(scriptPath); });
			}

			this.tracker = new TextFileTracker(folder, "*.cs") {
				OnChanged = (fileName) => {
					Util.Try(() => {
						Thread.Sleep(1000);
						this.LoadScript(fileName);
					});
				},
				OnFileCreation = (fileName) => {
					Util.Try(() => {
						Thread.Sleep(1000);
						this.LoadScript(fileName);
					});
				}
			};

			this.tracker.Start();
		}

		public List<IActionScript> ActionScripts => this.Scripts.OfType<IActionScript>().ToList();

		public void Start()
		{
			this.RunStartupMethod();
		}

		public void Stop()
		{
			this.RunShutdownMethod();
			this.tracker?.Stop();
		}

		public void Restart()
		{
			this.Stop();
			this.Start();
		}
	}

	public class ScriptItem
	{
		public ScriptItem(string name, string id, AsmHelper asm, object scriptObject)
		{
			this.Path = name;
			this.Id = id;
			this.AsmHelper = asm;
			this.Script = scriptObject;
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
			try {
				return helper.AlignToInterface<T>(obj);
			} catch {
				return null;
			}
		}
	}
}