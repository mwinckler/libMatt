using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace libMatt.Data {

#if USE_SQLSERVERCE

	public class SqlCeDataProvider: IDataProvider {

		private string _conn_str;

		public SqlCeDataProvider(string connectionString) {
			_conn_str = connectionString;
		}

		#region IDataProvider Members

		public System.Data.IDbConnection CreateConnection() {
			var conn = new SqlCeConnection(_conn_str);
			conn.Open();
			return conn;
		}

		public System.Data.IDbDataAdapter CreateDataAdapter() {
			return new SqlCeDataAdapter();
		}

		#endregion
	}

#endif
}
