using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

namespace IL2CDR.Scripts
{
    public class Example : ActionScriptBase
    {
        private MySQLDatabase mysql;
        public Example()
        {
            mysql = new MySQLDatabase("localhost", "user", "password", "database");
        }
        public override ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    IsEnabled = true,
                    Title = "Frag counter",
                    Description = "Sends frag event to MySQL DB",
                };
            }
        }
        public override void OnApplicationStartup(object data)
        {            
            mysql.Connect();
        }
        public override void OnApplicationShutdown(object data)
        {
            mysql.Disconnect();
        }
        public override void OnKill(MissionLogEventKill data)
        {
            Log.WriteInfo("Kill event: {0} by {1}", data.AttackerId, data.TargetId);
            
            mysql.ExecSql("CALL RecordKill('{0}','{1}')", data.AttackerId, data.TargetId);
        }
    }

}
