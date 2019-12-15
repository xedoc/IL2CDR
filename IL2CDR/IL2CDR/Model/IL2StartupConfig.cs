using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace IL2CDR.Model
{
	public class IL2StartupConfig
	{
		private const string RE_PARAMETER = @"[\s|\t|=|""]+(.*?)[\r\n|""]+"; // <-- TODO: Buggy! Does not support empty string ("") as parameter value! 
		private const string RE_SECTION = @"\[KEY[\s|\t|=]*{0}(.*?)[\n|\r]*\[END";
		private const string DEFAULT_CHAT_LOGFOLDER = "";
		private const string DEFAULT_MISSION_TEXT_LOG_FOLDER = "";

		private readonly string configFilePath;
		private string configContent;

		public string MissionTextLogFolder { get; set; }
		public string ChatLogFolder { get; set; }
		public string GameRootFolder { get; set; }

		public bool IsMissionTextLogEnabled { get; set; }
		public bool IsChatLogEnabled { get; set; }

		public bool IsRconEnabled { get; set; }
		public IPAddress RconIP { get; set; }
		public int RconPort { get; set; }

		public string Login { get; set; }
		public string Password { get; set; }

		public bool IsConfigReady { get; set; }

		public IL2StartupConfig(string configFilePath)
		{
			this.configFilePath = configFilePath;

			this.Initialize();
		}

		private void Initialize()
		{
			this.ReadConfig();
			if (!this.IsMissionTextLogEnabled || !this.IsChatLogEnabled || !this.IsRconEnabled || this.RconPort <= 0 ||
				this.RconPort > 65535 || this.RconIP == null || this.RconIP.Equals(default(IPAddress))
			) {
				this.IsConfigReady = false;
				this.EnableRequiredOptions();
			} else {
				this.IsConfigReady = true;
			}
		}

		public void ReadConfig()
		{
			if (string.IsNullOrWhiteSpace(this.configFilePath) ||
				!File.Exists(this.configFilePath)) {
				return;
			}

			Util.Try(() => {
				this.configContent = File.ReadAllText(this.configFilePath);

				this.GameRootFolder = Directory.GetParent(Path.GetDirectoryName(this.configFilePath)).FullName;

				this.MissionTextLogFolder = this.BuildDataFolder(this.GetString("text_log_folder"));
				this.ChatLogFolder = this.BuildDataFolder(this.GetString("chatlog_folder"));

				this.IsMissionTextLogEnabled = this.GetBool("chatlog");
				this.IsChatLogEnabled = this.GetBool("mission_text_log");
				this.Login = this.GetString("login");
				this.Password = this.GetString("password");
				this.IsRconEnabled = this.GetBool("rcon_start");
				this.RconIP = this.GetIPAddress("rcon_ip");
				this.RconPort = this.GetInt("rcon_port");
			});
		}

		private string BuildDataFolder(string relativeDataFolder)
		{
			if (string.IsNullOrWhiteSpace(relativeDataFolder)) {
				return Path.GetDirectoryName(this.configFilePath);
			} else if (!relativeDataFolder.Contains(":")) {
				return Path.Combine(Path.GetDirectoryName(this.configFilePath), relativeDataFolder);
			} else {
				return Path.GetDirectoryName(this.configFilePath);
			}
		}

		public void EnableRequiredOptions()
		{
			this.Backup();

			var boolFields = new string[] {"chatlog", "mission_text_log", "rcon_start"};
			foreach (var field in boolFields) {
				this.SetBool("system", field, true);
			}

			if (this.RconIP == null || this.RconIP.Equals(default(IPAddress))) {
				this.SetString("system", "rcon_ip", "127.0.0.1");
			}

			if (this.RconPort <= 0 || this.RconPort > 65535) {
				var minPort = 8800;
				var maxPort = 8891;
				for (; minPort < maxPort; minPort++) {
					Net.TestTCPPort("localhost", minPort, (entry, e) => {
						if (e == null && !entry.AddressList.Any()) {
							this.SetInt("system", "rcon_port", minPort);
							minPort = maxPort;
						}
					});
				}
			}

			this.ReadConfig();
		}

		private void SetBool(string section, string name, bool value)
		{
			this.SetValue(section, name, value ? "1" : "0");
		}

		private void SetInt(string section, string name, int value)
		{
			this.SetValue(section, name, value.ToString());
		}

		private void SetIPAddress(string section, string name, IPAddress ip)
		{
			this.SetValue(section, name, string.Concat("\"", ip.ToString(), "\""));
		}

		private void SetString(string section, string name, string value)
		{
			this.SetValue(section, name, string.Concat("\"", value, "\""));
		}

		private void SetValue(string section, string name, string value)
		{
			var sectionContent = Re.GetSubString(this.configContent, string.Format(RE_SECTION, section));
			if (string.IsNullOrWhiteSpace(sectionContent)) {
				return;
			}

			var replacement = Regex.Replace(sectionContent, string.Concat(@"[\s|\t]*", name, RE_PARAMETER),
				Environment.NewLine);
			replacement = string.Concat(replacement, string.Format("{2}\t{0} = {1}", name, value, Environment.NewLine));
			this.configContent = this.configContent.Replace(sectionContent, replacement);

			Util.Try(() => File.WriteAllText(this.configFilePath, this.configContent));
		}

		private int GetInt(string name)
		{
			var textParam = this.GetString(name);
			if (string.IsNullOrWhiteSpace(textParam)) {
				return 0;
			}

			int.TryParse(textParam, out var result);
			return result;
		}

		private IPAddress GetIPAddress(string name)
		{
			var textParam = this.GetString(name);
			if (string.IsNullOrWhiteSpace(textParam)) {
				return null;
			}

			IPAddress.TryParse(textParam, out var ipAddress);

			return ipAddress;
		}

		private bool GetBool(string name)
		{
			var textParam = this.GetString(name);
			if (string.IsNullOrWhiteSpace(textParam)) {
				return false;
			}

			return !(textParam.Equals("0") || textParam.Equals("false", StringComparison.InvariantCultureIgnoreCase));
		}

		private string GetString(string name)
		{
			if (string.IsNullOrWhiteSpace(name) ||
				string.IsNullOrWhiteSpace(this.configContent)) {
				return string.Empty;
			} else {
				return Re.GetSubString(this.configContent, string.Concat(name, RE_PARAMETER)).With(x => x.Trim());
			}
		}

		public void Backup()
		{
			var original = this.configFilePath;

			var backup = string.Concat(this.configFilePath, ".bak");
			if (!File.Exists(backup) && File.Exists(original)) {
				Util.Try(() => File.Copy(original, backup, true));
			}
		}
	}
}