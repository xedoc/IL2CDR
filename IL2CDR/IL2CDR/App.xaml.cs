using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.Threading;
using IL2CDR.Model;
using IL2CDR.Properties;

namespace IL2CDR
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		//IStopStart services
		public DServerManager DServerManager { get; set; }
		public MissionLogDataService MissionLogDataService { get; set; }

		public ScriptManager ScriptManager { get; set; }
		public ActionManager ActionManager { get; set; }
		public AppLogDataService AppLogDataService { get; set; }
		public SettingsManager SettingsManager { get; set; }
		public IL2StartupConfig StartupConfig { get; set; }

		static App()
		{
			DispatcherHelper.Initialize();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var appConfig = Settings.Default.Config;

			// -- initialization of the default Config value for Log truncation: 
			if (appConfig.ApplicationLogBufferSize == 0) {
				appConfig.ApplicationLogBufferSize = 500;
			}

			this.SettingsManager = new SettingsManager();

			this.AppLogDataService = new AppLogDataService() { MaxNumberOfLines = Math.Max(appConfig.ApplicationLogBufferSize, 100) };

			Log.WriteInfo("Application is starting...");


			Regex.CacheSize = 0;
			WebRequest.DefaultWebProxy = null;
			var rootDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\IL2CDR";
			AppDomain.CurrentDomain.SetData("DataDirectory", rootDataFolder);

			this.CreateDataFolders(rootDataFolder);
			this.CopyDataFolders(rootDataFolder);

			NativeMethods.SetProcessDPIAware();
			Net.DemandTCPPermission();
			if (RenderCapability.Tier == 0) {
				Timeline.DesiredFrameRateProperty.OverrideMetadata(
					typeof(Timeline),
					new FrameworkPropertyMetadata {DefaultValue = 20});
			}

			this.ScriptManager = new ScriptManager();
			this.ScriptManager.LoadScripts();
			this.ActionManager = new ActionManager(this.ScriptManager);

			this.DServerManager = new DServerManager();

			this.ScriptManager.Start();
			this.DServerManager.Start();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			IStopStart[] stopServices = {this.DServerManager, this.ScriptManager};
			foreach (var service in stopServices) {
				service.Stop();
			}
		}

		private void CopyDataFolders(string destinationDir)
		{
			var copyFolders = new Tuple<string, string>[] {
				new Tuple<string, string>(@".\Scripts", @"\Scripts\"),
			};

			try {
				foreach (var copyPair in copyFolders) {
					if (!Directory.Exists(destinationDir + copyPair.Item2)) {
						Directory.CreateDirectory(destinationDir + copyPair.Item2);
					}

					var sourceFiles = Directory.GetFiles(copyPair.Item1);
					foreach (var file in sourceFiles) {
						File.Copy(file, destinationDir + copyPair.Item2 + Path.GetFileName(file), true);
					}
				}
			} catch (Exception e) {
				Log.WriteError("Data file copy error: {0}", e.Message);
			}
		}

		private void CreateDataFolders(string rootDataFolder)
		{
			var appDataFolders = new string[] {
				@"\Scripts",
			};

			foreach (var folder in appDataFolders) {
				if (!Directory.Exists(rootDataFolder + folder)) {
					try {
						Directory.CreateDirectory(rootDataFolder + folder);
					} catch (Exception e) {
						Log.WriteError(@"Can't create data directory {0}: {1}", folder, e.Message);
					}
				}
			}
		}
	}
}