using IL2CDR.Properties;

namespace IL2CDR.Model
{
	public class SettingsManager
	{
		private Config config;
		private const string IL_2DISPLAY_NAME = "IL-2 Sturmovik Battle";

		public SettingsManager()
		{
			this.InitConfiguration();

			this.config = Settings.Default.Config;
		}

		private void InitConfiguration()
		{
			if (Settings.Default.Config == null) {
				this.ResetConfig();
			}
		}

		private void ResetConfig()
		{
			Settings.Default.Config = new Config();
		}

		public string GetIL2LocationFromRegistry()
		{
			return Installer.GetDirectoryByDisplayName(IL_2DISPLAY_NAME);
		}
	}
}