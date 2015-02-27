using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IL2CDR.Properties;

namespace IL2CDR.Model
{
    public class SettingsManager
    {
        private Config config;
        private IL2StartupConfig startupConfig;
        private const string startupConfigPath = @"data\startup.cfg";
        private const string IL2DisplayName = "IL-2 Sturmovik Battle";

        public SettingsManager()
        {
            config = Settings.Default.Config;
            startupConfig = (Application.Current as App).StartupConfig;
            if (startupConfig != null)
                startupConfig.ReadConfig();

            config.PropertyChanged += config_PropertyChanged;
        }

        void config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( e.PropertyName.Equals( "RootFolder" ))
            {
                if (!Directory.Exists(config.RootFolder))
                    return;
                startupConfig = new IL2StartupConfig(String.Concat(config.RootFolder, startupConfigPath));
                startupConfig.ReadConfig();
            }
        }
        public void BackupStartupConfig()
        {
            var original = String.Concat(config.RootFolder, startupConfigPath);
            var backup = String.Concat(config.RootFolder, startupConfigPath,".bak");
            if( !File.Exists( backup ) )
                Util.Try(() => File.Copy(original, backup,true ));
        }
        public string GetIL2LocationFromRegistry()
        {
            return Installer.GetDirectoryByDisplayName(IL2DisplayName);
        }
    }
}
