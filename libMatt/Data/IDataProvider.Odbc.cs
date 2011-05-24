using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace libMatt.Data {
	public class OdbcDataProvider: IDataProvider {

		private string _conn_str;

		public OdbcDataProvider(string connectionString) {
			_conn_str = connectionString;
		}

		#region IDataProvider Members

		public System.Data.IDbConnection CreateConnection() {
			OdbcConnection conn = new OdbcConnection(_conn_str);
			conn.Open();
			return conn;
		}

		public System.Data.IDbDataAdapter CreateDataAdapter() {
			return new OdbcDataAdapter();
		}

		#endregion
	}
}
