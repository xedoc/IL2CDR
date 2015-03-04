using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace IL2CDR.Model
{
    public class IL2StartupConfig
    {
        private const string reParameter = @"[\s|\t|=|""]+(.*?)[\r\n|""]+";
        private const string reSection = @"\[KEY[\s|\t|=]*{0}(.*?)[\n|\r]*\[END";
        private const string defaultChatLogfolder = "";
        private const string defaultMissionTextLogFolder = "";
        private string configFilePath;
        private string configContent;

        public string MissionTextLogFolder { get; set; }
        public string ChatLogFolder { get; set; }
        public string GameRootFolder { get; set; }

        public bool IsMissionTextLogEnabled { get; set; }
        public bool IsChatLogEnabled { get; set; }

        public bool IsRconEnabled { get; set; }
        public IPAddress RconIP { get; set; }
        public int RconPort { get; set; }
        
        public string Login { get; set;  }
        public string Password { get; set; }

        public bool IsConfigReady { get; set; }

        public IL2StartupConfig(string configFilePath)
        {
            this.configFilePath = configFilePath;

            Initialize();
        }
        private void Initialize()
        {
            ReadConfig();
            if (!IsMissionTextLogEnabled || !IsChatLogEnabled || !IsRconEnabled ||
                 RconPort <= 0 || RconPort > 65535 ||
                 RconIP == null ||
                 RconIP.Equals(default(IPAddress))
                 )
            {
                IsConfigReady = false;
                EnableRequiredOptions();
            }
            else
            {
                IsConfigReady = true;
            }
        }
        public void ReadConfig()
        {
            if (String.IsNullOrWhiteSpace(configFilePath) ||
                !File.Exists(configFilePath))
                return;

            Util.Try(() => {
                configContent = File.ReadAllText(configFilePath);

                GameRootFolder = Directory.GetParent(Path.GetDirectoryName(configFilePath)).FullName;
                
                MissionTextLogFolder = BuildDataFolder( GetString( "text_log_folder" ));
                ChatLogFolder = BuildDataFolder( GetString("chatlog_folder"));

                IsMissionTextLogEnabled = GetBool("chatlog");
                IsChatLogEnabled = GetBool("mission_text_log");
                Login = GetString("login");
                Password = GetString("password");
                IsRconEnabled = GetBool("rcon_start");
                RconIP = GetIPAddress("rcon_ip");
                RconPort = GetInt("rcon_port");
            });
        }

        private string BuildDataFolder( string relativeDataFolder )
        {
            if (String.IsNullOrWhiteSpace(relativeDataFolder))
                return Path.GetDirectoryName(configFilePath);
            else if (!relativeDataFolder.Contains(":"))
                return Path.Combine(  Path.GetDirectoryName(configFilePath), relativeDataFolder);
            else
                return Path.GetDirectoryName(configFilePath);

        }

        public void EnableRequiredOptions()
        {
            Backup();

            var boolFields = new string[] {"chatlog", "mission_text_log", "rcon_start"};
            foreach (var field in boolFields)
                SetBool("system", field, true);

            if (RconIP == null || RconIP.Equals(default(IPAddress)))
                SetString("system", "rcon_ip", "127.0.0.1");
            
            if (RconPort <= 0 || RconPort > 65535)
            {
                int minPort = 8800;
                int maxPort = 8891;
                for (; minPort < maxPort;minPort ++ )
                {
                    Net.TestTCPPort("localhost", minPort, (entry, e) => {
                        if (e == null && entry.AddressList.Count() == 0)
                        {
                            SetInt("system", "rcon_port", minPort);
                            minPort = maxPort;
                        }
                    });
                }
            }

            ReadConfig();
        }

        private void SetBool(string section, string name, bool value)
        {
            SetValue(section, name, value ? "1" : "0");
        }
        private void SetInt(string section, string name, int value)
        {
            SetValue(section, name, value.ToString());
        }
        private void SetIPAddress(string section, string name, IPAddress ip)
        {
            SetValue(section, name, String.Concat("\"", ip.ToString(), "\""));
        }
        private void SetString( string section, string name, string value)
        {
            SetValue(section, name, String.Concat("\"", value, "\""));
        }
        private void SetValue(string section, string name, string value)
        {
            var sectionContent = Re.GetSubString(configContent, String.Format(reSection,section));
            if (String.IsNullOrWhiteSpace(sectionContent))
                return;

            var replacement = Regex.Replace(sectionContent, String.Concat(@"[\s|\t]*",name, reParameter), Environment.NewLine);
            replacement = String.Concat(replacement, String.Format("{2}\t{0} = {1}", name, value, Environment.NewLine));
            configContent = configContent.Replace(sectionContent, replacement);

            Util.Try(() => File.WriteAllText(configFilePath, configContent));
        }
        private int GetInt(string name)
        {
            int result = 0;
            var textParam = GetString(name);
            if (String.IsNullOrWhiteSpace(textParam))
                return 0;

            int.TryParse(textParam, out result);
            return result;            
        }
        private IPAddress GetIPAddress( string name )
        {
            IPAddress ipAddress = null;
            var textParam = GetString(name);
            if (String.IsNullOrWhiteSpace(textParam))
                return null;

            IPAddress.TryParse(textParam, out ipAddress);

            return ipAddress;
        }
        private bool GetBool(string name )
        {
            var textParam = GetString(name);
            if (String.IsNullOrWhiteSpace(textParam))
                return false;

            return !(textParam.Equals("0") || textParam.Equals("false", StringComparison.InvariantCultureIgnoreCase));

        }
        private string GetString(string name)
        {
            if (String.IsNullOrWhiteSpace(name) || 
                String.IsNullOrWhiteSpace(configContent))
                return String.Empty;
            else
                return Re.GetSubString(configContent, String.Concat(name, reParameter)).With(x => x.Trim());
        }

        public void Backup()
        {
            var original = String.Concat(configFilePath);
            var backup = String.Concat(configFilePath, ".bak");
            if (!File.Exists(backup) && File.Exists(original))
                Util.Try(() => File.Copy(original, backup, true));
        }
    }
}
