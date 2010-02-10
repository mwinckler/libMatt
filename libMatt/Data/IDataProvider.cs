using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace libMatt.Data {
	/// <summary>
	/// Summary description for IDataProvider
	/// </summary>
	public interface IDataProvider {
		IDbConnection CreateConnection();
		IDbDataAdapter CreateDataAdapter();
	}

}