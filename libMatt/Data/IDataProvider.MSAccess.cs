using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace libMatt.Data
{
	public class MSAccessDataProvider: IDataProvider {

		private string _connStr;

		public MSAccessDataProvider(string connectionString) {
			_connStr = connectionString;
		}


		#region IDataProvider Members

		public System.Data.IDbConnection CreateConnection() {
			var conn = new OleDbConnection(_connStr);
			conn.Open();
			return conn;
		}

		public System.Data.IDbDataAdapter CreateDataAdapter() {
			return new OleDbDataAdapter();
		}

		#endregion
	}
}
