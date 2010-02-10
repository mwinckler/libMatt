using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace libMatt.Data {

	public abstract class Dalc {

		protected class Param {
			private string _name;
			private object _value;

			public Param(string name, object value) {
				Name = name;
				Value = value;
			}

			public string Name {
				get { return _name; }
				set { _name = value; }
			}
			public object Value {
				get { return _value; }
				set { _value = value; }
			}
		}

		protected CommandType DefaultCommandType { get; set; }

		public Dalc(IDataProvider provider) : this(provider, CommandType.Text) { }

		public Dalc(IDataProvider provider, CommandType defaultCommandType) {
			this.DataProvider = provider;
			this.DefaultCommandType = defaultCommandType;
		}


		public IDataProvider DataProvider { get; private set; }

		private void HandleException(Exception ex, string commandText, Param[] parameters) {
			List<string> parms = new List<string>();
			foreach (Param p in parameters) {
				parms.Add(p.Name + ":" + p.Value);
			}
			throw new ApplicationException("commandText: " + commandText +
				"; parameters: {" + String.Join(",", parms.ToArray()) + "}", ex);
		}

		protected System.Data.IDbConnection GetConnection() {
			return DataProvider.CreateConnection();
		}


		protected object ExecuteScalar(string commandText, CommandType commandType, params Param[] parameters) {
			var cmd = GetCommand(commandText, parameters);
			cmd.CommandType = commandType;
			object ret = cmd.ExecuteScalar();
			DisposeCommand(cmd);
			return ret;
		}

		protected object ExecuteScalar(string commandText, params Param[] parameters) {
			return ExecuteScalar(commandText, this.DefaultCommandType, parameters);
		}


		protected void ExecuteNonQuery(string commandText, CommandType commandType, params Param[] parameters) {
			var cmd = GetCommand(commandText, parameters);
			cmd.CommandType = commandType;
			cmd.ExecuteNonQuery();
			DisposeCommand(cmd);
		}

		protected void ExecuteNonQuery(string commandText, params Param[] parameters) {
			ExecuteNonQuery(commandText, this.DefaultCommandType, parameters);
		}

		protected IDataReader ExecuteReader(string commandText, params Param[] parameters) {
			var cmd = GetCommand(commandText, parameters);
			return cmd.ExecuteReader(CommandBehavior.CloseConnection);
		}

		protected DataTable ExecuteDataTable(
				string commandText,
				CommandType commandType,
				Param[] parameters) {

			IDbDataAdapter da = DataProvider.CreateDataAdapter();
			da.SelectCommand = GetCommand(commandText, commandType, parameters);

			DataSet ds = new DataSet();
			da.Fill(ds);
			DisposeCommand(da.SelectCommand);
			return (ds.Tables.Count > 0 ? ds.Tables[0] : null);
		}
		/*
		protected static DataTable ExecuteDatatable(string commandText, CommandType commandType, params Param[] parameters) {
			var dt = new DataTable();
			var da = new SqlDataAdapter();
			da.SelectCommand = GetSqlCommand(commandText, commandType, parameters);
			da.Fill(dt);
			// Must close connection manually - Fill only auto-closes if the connection
			// was closed to begin with (and GetSqlCommand opens it).
			da.SelectCommand.Connection.Close();
			da.Dispose();
			return dt;
		}
		*/

		protected DataTable ExecuteDataTable(string commandText, params Param[] parameters) {
			return ExecuteDataTable(commandText, this.DefaultCommandType, parameters);
		}

		protected IDbCommand GetCommand(string commandText, CommandType commandType, params Param[] parameters) {
			IDbCommand cmd = GetConnection().CreateCommand();
			cmd.CommandText = commandText;
			cmd.CommandType = commandType;
			IDataParameter parm;
			foreach (var p in parameters) {
				parm = cmd.CreateParameter();
				parm.ParameterName = p.Name;
				parm.Value = p.Value;
				cmd.Parameters.Add(parm);
			}
			return cmd;

		}

		protected IDbCommand GetCommand(string commandText, params Param[] parameters) {
			return GetCommand(commandText, this.DefaultCommandType, parameters);
		}

		protected void DisposeCommand(IDbCommand cmd) {
			if (null != cmd) {
				if (null != cmd.Connection) {
					if (cmd.Connection.State != ConnectionState.Closed)
						cmd.Connection.Close();
					cmd.Connection.Dispose();
				}
				cmd.Dispose();
			}
		}

		protected void DisposeReader(IDataReader reader) {
			// Reader should be opened with CommandBehavior.CloseConnection, so conn
			// will be closed upon disposal.
			if (null != reader) {
				if (!reader.IsClosed)
					reader.Close();
				reader.Dispose();
			}
		}




		/*
		protected IDbCommand GetCommand() {
			IDbCommand cmd = GetConnection().CreateCommand();
			cmd.CommandType = CommandType.Text;
			return cmd;
		}

		protected IDbCommand GetCommand(string commandText) {
			IDbCommand cmd = GetCommand();
			cmd.CommandText = commandText;
			return cmd;
		}

		protected IDbCommand GetCommand(
			string commandText,
			Param[] parameters
			) {
			IDbCommand cmd = GetCommand(commandText);
			IDbDataParameter param = null;
			foreach (Param parm in parameters) {
				param = cmd.CreateParameter();
				param.ParameterName = parm.Name;
				param.Value = parm.Value;
				cmd.Parameters.Add(param);
			}
			return cmd;
		}

		protected IDbCommand GetCommand(
			string commandText,
			FieldCollection parameters
			) {
			return GetCommand(commandText, parameters.Fields.ConvertAll<Param>(
				delegate(FieldDefinition fd) { return new Param(fd.FieldName, fd.Value); }
			).ToArray());
		}


		protected DataTable ExecuteDataTable(
			string commandText,
			Param[] parameters) {

			IDbDataAdapter da = DataProvider.CreateDataAdapter();
			da.SelectCommand = GetCommand(commandText, parameters);

			DataSet ds = new DataSet();
			da.Fill(ds);
			DisposeCommand(da.SelectCommand);
			return (ds.Tables.Count > 0 ? ds.Tables[0] : null);
		}

		protected DataTable ExecuteDataTable(string commandText) {
			return ExecuteDataTable(commandText, new Param[] { });
		}

		protected void ExecuteNonQuery(
			string commandText,
			Param[] parameters) {
			IDbCommand cmd = null;
			try {
				cmd = GetCommand(commandText, parameters);
				cmd.ExecuteNonQuery();
			} catch (System.Data.Common.DbException ex) {
				HandleException(ex, commandText, parameters);
			} finally {
				DisposeCommand(cmd);
			}
		}

		protected object ExecuteScalar(
			string commandText,
			Param[] parameters) {
			object ret = null;
			IDbCommand cmd = null;
			try {
				cmd = GetCommand(commandText, parameters);
				ret = cmd.ExecuteScalar();
			} catch (System.Data.Common.DbException ex) {
				HandleException(ex, commandText, parameters);
			} finally {
				DisposeCommand(cmd);
			}
			return ret;
		}


		protected void DisposeCommand(IDbCommand cmd) {
			if (null != cmd) {
				if (null != cmd.Connection && cmd.Connection.State != ConnectionState.Closed)
					cmd.Connection.Close();
				cmd.Connection.Dispose();
				cmd.Dispose();
			}
		}



		*/
	}

}