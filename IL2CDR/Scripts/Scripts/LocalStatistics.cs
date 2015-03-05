using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

namespace IL2CDR.Scripts
{
    public class LocalStatistics : ActionScriptBase
    {
        private MySQLDatabase mysql;
        public LocalStatistics()
        {
        }
        public override ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    //Enable events processing by this script
                    IsEnabled = true,
                    //Title for script list
                    Title = "Frag counter",
                    //Script description for GUI
                    Description = "Sends frag event to MySQL DB",
                    //Settings in GUI
                    ConfigFields = new ConfigFieldList()
                    {
                        { "host", "Host", "MySQL hostname", FieldType.Text, String.Empty, true},
                        { "user", "User", "MySQL username", FieldType.Text, String.Empty, true},
                        { "pass", "Password", "MySQL password", FieldType.Password, String.Empty, true},
                        { "db", "Database", "MySQL database", FieldType.Text, String.Empty, true},
                        { "port", "Port", "MySQL TCP port", FieldType.Text, 3306, true},
                    },
                };
            }
        }
        public override void OnApplicationStartup(object data)
        {
            mysql = new MySQLDatabase(Config.GetString("host"),
                                        Config.GetString("user"),
                                        Config.GetString("pass"),
                                        Config.GetString("db"), 
                                        Config.GetInt("port"));
            if (mysql.Connect())
                Log.WriteInfo("Connected to the MySQL!");
            else
                Log.WriteError("MySQL connection failed! Check settings!");
        }
        public override void OnApplicationShutdown(object data)
        {
            mysql.Disconnect();
        }
        public override void OnKill(MissionLogEventKill data)
        {
            var attacker = data.Server.Players[data.AttackerId];
            var target = data.Server.Players[data.TargetId];

            var mysqlVars = "SET " +
                            "Kill.EventId ='{0}', " +
                            "Kill.Attacker.Name='{1}', " +
                            "Kill.Target.Name='{2}', " +
                            "Kill.Attacker.Country.Id='{3}', " +
                            "Kill.Target.Country.Id='{4}'";
            
            //Attacker is player
            if( attacker != null )
            {
                //Target is player either ?
                if( target != null )
                {
                    mysql.ExecSql(mysqlVars, data.EventID, attacker.NickName, target.NickName, attacker.Country.Id, target.Country.Id);
                    mysql.ExecSql("CALL RecordPlayerKill()");
                    Log.WriteInfo("Kill event: {0} by {1}", target.NickName, attacker.NickName);
                }
            }
        }
    }

}
