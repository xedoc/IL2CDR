using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.Threading;
using IL2CDR.Model;

namespace IL2CDR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            NativeMethods.SetProcessDPIAware();

            Net.DemandTCPPermission();

            if (RenderCapability.Tier == 0)
                Timeline.DesiredFrameRateProperty.OverrideMetadata(
                    typeof(Timeline),
                    new FrameworkPropertyMetadata { DefaultValue = 20 });

            Regex.CacheSize = 0;
            
            InitConfiguration();
            var rootDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\IL2CDR";
            CreateDataFolders(rootDataFolder);
            CopyDataFolders(rootDataFolder);
            AppDomain.CurrentDomain.SetData("DataDirectory", rootDataFolder);

            var scriptManager = new ScriptManager();
            scriptManager.LoadScripts();

            WebRequest.DefaultWebProxy = null;
        }
        private void InitConfiguration()
        {
            if( IL2CDR.Properties.Settings.Default.Config == null )
            {
                ResetConfig();
            }
        }
        private void ResetConfig()
        {
            IL2CDR.Properties.Settings.Default.Config = new Config()
            {
                LastMissionLogFile = String.Empty,
                MissonLogFolder = String.Empty,                
            };
             

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
