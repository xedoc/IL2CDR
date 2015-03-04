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
        private const string IL2DisplayName = "IL-2 Sturmovik Battle";

        public SettingsManager()
        {
            InitConfiguration();

            config = Settings.Default.Config;

        }

        private void InitConfiguration()
        {
            if (IL2CDR.Properties.Settings.Default.Config == null)
            {
                ResetConfig();
            }
        }
        private void ResetConfig()
        {
            IL2CDR.Properties.Settings.Default.Config = new Config();
        }

        public string GetIL2LocationFromRegistry()
        {
            return Installer.GetDirectoryByDisplayName(IL2DisplayName);
        }
    }
}
