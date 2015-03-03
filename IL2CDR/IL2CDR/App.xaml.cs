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
        public StatusDataService StatusDataService { get; set; }

        static App()
        {
            DispatcherHelper.Initialize();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            AppLogDataService = new AppLogDataService();
            Log.WriteInfo("Application is starting...");

            StatusDataService = new StatusDataService();
            SettingsManager = new SettingsManager();

            Regex.CacheSize = 0;
            WebRequest.DefaultWebProxy = null;
            var rootDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\IL2CDR";
            AppDomain.CurrentDomain.SetData("DataDirectory", rootDataFolder);

            CreateDataFolders(rootDataFolder);
            CopyDataFolders(rootDataFolder);

            NativeMethods.SetProcessDPIAware();
            Net.DemandTCPPermission();
            if (RenderCapability.Tier == 0)
                Timeline.DesiredFrameRateProperty.OverrideMetadata(
                    typeof(Timeline),
                    new FrameworkPropertyMetadata { DefaultValue = 20 });

            DServerManager = new DServerManager();
            DServerManager.Start();

            SettingsManager.BackupStartupConfig();

            StartupConfig = new IL2StartupConfig(String.Format(@"{0}data\startup.cfg", Settings.Default.Config.RootFolder));
            StartupConfig.ReadConfig();

            ScriptManager = new ScriptManager();
            ScriptManager.LoadScripts();
            ActionManager = new ActionManager(ScriptManager);

            if (!String.IsNullOrWhiteSpace(Settings.Default.Config.RootFolder))
            {
                MissionLogDataService = new MissionLogDataService(String.Concat(Settings.Default.Config.RootFolder, StartupConfig.MissionTextLogFolder));
                if( Settings.Default.Config.IsMissionLogMonitorEnabled )
                    MissionLogDataService.Start();
            }
        }
        protected override void OnExit(ExitEventArgs e)
        {
            IStopStart[] stopServices = { DServerManager, MissionLogDataService };
            foreach (var service in stopServices)
                service.Stop();
        }

        private void CopyDataFolders(string destinationDir)
        {

            var copyFolders = new Tuple<string, string>[] {
                new Tuple<string,string>(@".\Scripts", @"\Scripts\"),
            };

            try
            {
                foreach (var copyPair in copyFolders)
                {
                    if (!Directory.Exists(destinationDir + copyPair.Item2))
                        Directory.CreateDirectory(destinationDir + copyPair.Item2);

                    var sourceFiles = Directory.GetFiles(copyPair.Item1);
                    foreach (var file in sourceFiles)
                    {
                        File.Copy(file, destinationDir + copyPair.Item2 + Path.GetFileName(file), true);
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteError("Data file copy error: {0}", e.Message);

            }


        }
        private void CreateDataFolders(string rootDataFolder)
        {
            var appDataFolders = new string[] {
                @"\Scripts", 
            };

            foreach (var folder in appDataFolders)
            {
                if (!Directory.Exists(rootDataFolder + folder))
                {
                    try
                    {
                        Directory.CreateDirectory(rootDataFolder + folder);
                    }
                    catch (Exception e)
                    {
                        Log.WriteError(@"Can't create data directory {0}: {1}", folder, e.Message);
                    }
                }

            }
        }

    }
}
