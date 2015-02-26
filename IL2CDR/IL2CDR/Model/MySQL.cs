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
    public class MySQL
    {
        public MySqlConnection conn;
        public MySqlCommand cmd = new MySqlCommand();
        private static object lockMysql = new object();
        private const String mysqlConnString = @"Data Source={0};Initial Catalog=company;User Id={1};Password={2};UseCompression=true;Keepalive=1;Minimum Pool Size=1;Maximum Pool Size=100;Pooling=true;";

        public MySQL()
        {
            IsConnected = false;
        }

        public String Host { get; set; }
        public String User { get; set; }
        public String Password { get; set; }
        public String Database { get; set; }
        public bool IsConnected { get; set; }
        public void Connect()
        {
            lock (lockMysql)
            {

                if (IsConnected)
                {
                    IsConnected = false;
                    conn.Close();
                    conn = new MySqlConnection(String.Format(mysqlConnString, Host, User, Password));
                }   
                conn = new MySqlConnection(String.Format(mysqlConnString, Host, User, Password));
                conn.OpenAsync().ContinueWith((task) =>
                {
                    IsConnected = true;

                }, TaskContinuationOptions.ExecuteSynchronously);

                while (!IsConnected)
                    Thread.Sleep(16);
            }
        }
        public void Disconnect()
        {
            if (conn != null &&
                conn.State != ConnectionState.Broken &&
                conn.State != ConnectionState.Closed)
            {
                Util.Try(() => conn.Close());
            }
        }

        public void ExecSql(String query)
        {
            Log.WriteInfo(query);
            lock (lockMysql)
            {
                var script = new MySqlScript(conn, query);
                script.ExecuteAsync().Wait();
            }
        }

        public List<NameValueCollection> Select(String query)
        {
            var result = new List<NameValueCollection>();

            lock (lockMysql)
            {
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
                return result;
            }
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

    }
}
