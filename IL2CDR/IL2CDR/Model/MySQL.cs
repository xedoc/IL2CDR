using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace IL2CDR.Model
{
	public class MySQLDatabase
	{
		public MySqlConnection conn;
		public MySqlCommand cmd = new MySqlCommand();
		private static readonly object lockMysql = new object();

		private const string MYSQL_CONN_STRING = @"Data Source={0};Port={4};Initial Catalog={3};User Id={1};Password={2};UseCompression=true;Keepalive=1;Minimum Pool Size=1;Maximum Pool Size=100;Pooling=true;";

		public MySQLDatabase(string host, string user, string password, string database, int port = 3306)
		{
			this.Host = host;
			this.User = user;
			this.Password = password;
			this.Database = database;
			this.Port = port;

			this.IsConfigIncorrect = false;
			this.IsConnected = false;
		}

		public int Port { get; set; }
		public string Host { get; set; }
		public string User { get; set; }
		public string Password { get; set; }
		public string Database { get; set; }
		public bool IsConnected { get; set; }
		public bool IsConfigIncorrect { get; set; }

		public bool Connect()
		{
			lock (lockMysql) {
				if (!this.IsConfigIncorrect) {
					if (this.IsConnected) {
						this.IsConnected = false;
						this.conn.Close();
						this.conn = new MySqlConnection(string.Format(MYSQL_CONN_STRING, this.Host, this.User,
							this.Password, this.Database, this.Port));
					}

					this.conn = new MySqlConnection(string.Format(MYSQL_CONN_STRING, this.Host, this.User, this.Password,
						this.Database, this.Port));
					if (this.Try(() => this.conn.Open()) == null) {
						this.IsConnected = true;
					}
				}

				return this.IsConnected;
			}
		}

		public void Disconnect()
		{
			if (!this.IsConfigIncorrect && this.conn != null && this.conn.State != ConnectionState.Broken &&
				this.conn.State != ConnectionState.Closed) {
				this.Try(() => this.conn.Close());
			}
		}

		public void ExecSql(string query)
		{
			if (!this.CheckConnetion()) {
				Log.WriteError("Can't execute a query:{0}.\nNot connected to the server", query);
				return;
			}

			lock (lockMysql) {
				var script = new MySqlScript(this.conn, query);
				this.Try(() => script.ExecuteAsync().Wait());
			}
		}

		public void ExecSql(string query, params object[] args)
		{
			this.ExecSql(string.Format(query, args));
		}

		public List<NameValueCollection> Select(string query)
		{
			if (!this.CheckConnetion()) {
				Log.WriteError("Can't execute a query:{0}.\nNot connected to the server", query);
				return new List<NameValueCollection>();
			}

			var result = new List<NameValueCollection>();

			lock (lockMysql) {
				this.Try(() => {
					this.cmd = new MySqlCommand(query, this.conn);
					var reader = this.cmd.ExecuteReader();

					while (reader.Read()) {
						var row = new NameValueCollection();
						for (var i = 0; i < reader.FieldCount; i++) {
							row[reader.GetName(i)] =
								reader.GetValue(i) == DBNull.Value ? null : (string) reader.GetValue(i);
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
			if (this.IsConfigIncorrect) {
				return false;
			}

			if (!this.IsConnected) {
				this.Connect();
			}

			if (!this.IsConnected) {
				return false;
			}

			if (!this.conn.Ping()) {
				this.Connect();
				if (!this.IsConnected) {
					return false;
				}
			}


			return true;
		}

		public List<NameValueCollection> Select(string query, params object[] args)
		{
			return this.Select(string.Format(query, args));
		}

		public string QuoteSQL(string text)
		{
			var replaceList = new Dictionary<string, string>() {
				{@"'", @"''"},
				{@"\", @" "},
				{"\n", @" "},
				{"\r", @" "},
				{"\r\n", @" "}
			};
			if (!string.IsNullOrEmpty(text)) {
				replaceList.ToList().ForEach(pair => text = text.Replace(pair.Key, pair.Value));
			}

			return text;
		}

		private Exception Try(Action action)
		{
			if (action == null) {
				return new NullReferenceException();
			}

			var error = Util.Try(action, false);

			if (error?.InnerException != null) {
				var mysqlError = error.InnerException as MySqlException;
				if (mysqlError == null) {
					this.IsConfigIncorrect = true;
					return error;
				}

				var errorCodes = new int[] {1049, 1042, 1044, 1045, 1046};
				if (errorCodes.Contains(mysqlError.Number)) {
					this.IsConfigIncorrect = true;
				}
			}

			return error;
		}
	}
}