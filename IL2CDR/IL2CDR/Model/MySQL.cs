using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace IL2CDR.Model
{
    public class MySQLDatabase
    {
        public MySqlConnection conn;
        public MySqlCommand cmd = new MySqlCommand();
        private static object lockMysql = new object();
        private const String mysqlConnString = @"Data Source={0};Initial Catalog={3};User Id={1};Password={2};UseCompression=true;Keepalive=1;Minimum Pool Size=1;Maximum Pool Size=100;Pooling=true;";

        public MySQLDatabase(string host, string user, string password, string database )
        {
            Host = host;
            User = user;
            Password = password;
            Database = database;
            
            IsConfigIncorrect = false;
            IsConnected = false;
        }

        public String Host { get; set; }
        public String User { get; set; }
        public String Password { get; set; }
        public String Database { get; set; }
        public bool IsConnected { get; set; }
        public bool IsConfigIncorrect { get; set; }

        public void Connect()
        {
            lock (lockMysql)
            {
                if( !IsConfigIncorrect )
                { 
                    if (IsConnected )
                    {
                        IsConnected = false;
                        conn.Close();
                        conn = new MySqlConnection(String.Format(mysqlConnString, Host, User, Password, Database));
                    }   
                    conn = new MySqlConnection(String.Format(mysqlConnString, Host, User, Password, Database));
                    Try(() => conn.Open());
                }
            }
        }
        public void Disconnect()
        {
            if (!IsConfigIncorrect &&
                conn != null &&
                conn.State != ConnectionState.Broken &&
                conn.State != ConnectionState.Closed)
            {
                Try(() => conn.Close());
            }
        }

        public void ExecSql(String query)
        {
            if (!CheckConnetion())
            {
                Log.WriteError("Can't execute a query:{0}.\nNot connected to the server", query);
                return;
            }
            lock (lockMysql)
            {
                var script = new MySqlScript(conn, query);
                Try( ()=> script.ExecuteAsync().Wait() );
            }
        }
        public void ExecSql(String query, params object[] args)
        {

            ExecSql(String.Format(query, args));
        }

        public List<NameValueCollection> Select(String query)
        {
            if( !CheckConnetion() )
            {
                Log.WriteError("Can't execute a query:{0}.\nNot connected to the server", query);
                return new List<NameValueCollection>();
            }

            var result = new List<NameValueCollection>();

            lock (lockMysql)
            {
                Try(() => {
                    cmd = new MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var row = new NameValueCollection();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i) == DBNull.Value ? null : (string)reader.GetValue(i);
                        }
                        result.Add(row);
                    }
                    reader.Close();                
                });
                return result;
            }
        }
        public bool CheckConnetion()
        {
            if (IsConfigIncorrect)
                return false;

            if (!IsConnected)
                Connect();

            if (!IsConnected)
                return false;

            if( !conn.Ping() )
            {
                Connect();
            }
            
            return true;

        }
        public List<NameValueCollection> Select(String query, params object[] args)
        {
            return Select(String.Format(query, args));
        }

        public String QuoteSQL(String text)
        {
            Dictionary<String, String> replaceList = new Dictionary<string, string>() {
                {@"'", @"''"},
		        {@"\", @" "},
		        {"\n", @" "},
		        {"\r", @" "},
		        {"\r\n", @" "}
            };
            if (!String.IsNullOrEmpty(text))
                replaceList.ToList().ForEach(pair => text = text.Replace(pair.Key, pair.Value));

            return text;
        }

        private Exception Try( Action action )
        {
            if (action == null)
                return null;

            Exception error = Util.Try(action, false);

            if (error != null && error.InnerException != null)
            {
                var mysqlError = error.InnerException as MySqlException;
                var errorCodes = new int[] { 1049, 1042, 1044, 1045, 1046 };
                foreach (int code in errorCodes)
                {
                    if (code == mysqlError.Number)
                    {
                        IsConfigIncorrect = true;
                        break;
                    }
                    
                }
            }
            
            return error;
        }
    }
}
