using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if USE_SQLSERVERCE
using System.Data.SqlServerCe;
#endif

namespace libMatt.Data {

#if USE_SQLSERVERCE

	public class SqlCeDataProvider: IDataProvider {

		private string _connStr;

		public SqlCeDataProvider(string connectionString) {
			_connStr = connectionString;
		}

		#region IDataProvider Members

		public System.Data.IDbConnection CreateConnection() {
			var conn = new SqlCeConnection(_connStr);
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
