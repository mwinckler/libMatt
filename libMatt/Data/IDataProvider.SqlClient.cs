using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace libMatt.Data {
	/// <summary>
	/// Summary description for IDataProvider
	/// </summary>
	public class SqlDataProvider : IDataProvider {

		private string _conn_str;

		public SqlDataProvider(string connectionString) {
			_conn_str = connectionString;
		}

		#region IDataProvider Members

		protected SqlConnection CreateSqlConnection() {
			SqlConnection connection = new SqlConnection(_conn_str);
			connection.Open();
			return connection;
		}

		public IDbConnection CreateConnection() {
			return CreateSqlConnection();
		}

		public IDbDataAdapter CreateDataAdapter() {
			return new SqlDataAdapter();
		}

		#endregion
	}

}