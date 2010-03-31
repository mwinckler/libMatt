using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace libMatt.Data {

	public abstract class Dalc: IDisposable {

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

		/// <summary>
		/// Instantiates dalc to use an existing transaction.
		/// 
		/// Note that this will cause the dalc to ignore the transaction
		/// during Dispose, as it is assumed that a different Dalc instance
		/// is the owner of the transaction.
		/// </summary>
		/// <param name="trans"></param>
		public Dalc(IDbTransaction trans) {
			_trans = trans;
			this.DefaultCommandType = CommandType.Text;
		}

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
			if (_trans != null) {
				return _trans.Connection;
			} else {
				return DataProvider.CreateConnection();
			}
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

		protected DataTable ExecuteDataTable(string commandText, params Param[] parameters) {
			return ExecuteDataTable(commandText, this.DefaultCommandType, parameters);
		}

		protected IDbCommand GetCommand(string commandText, CommandType commandType, params Param[] parameters) {
			IDbCommand cmd = GetConnection().CreateCommand();
			if (_trans != null) {
				cmd.Transaction = _trans;
			}
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
				// Do not dispose connection if this command is part of an ongoing transaction.
				if (_trans == null && null != cmd.Connection) {
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


		#region Transactions

		private bool _isTransOwner = false;
		private IDbTransaction _trans;

		public IDbTransaction BeginTransaction() {
			if (_trans != null) {
				throw new InvalidOperationException("[Dalc.BeginTransaction] Cannot begin new transaction: a transaction is already in progress.");
			}
			var conn = this.DataProvider.CreateConnection();
			_trans = conn.BeginTransaction();
			_isTransOwner = true;
			return _trans;
		}
		/* Better not to expose these - the caller should deal with the transaction object directly.
		public void Commit() {
			if (_trans != null) {
				_trans.Commit();
			}
		}

		public void Rollback() {
			if (_trans != null) {
				_trans.Rollback();
			}
		}
		*/
		#endregion


		#region IDisposable Members

		public void Dispose() {
			// Check for existing connection and any open transactions.
			if (_isTransOwner && _trans != null) {
				if (_trans.Connection != null) {
					if (_trans.Connection.State != ConnectionState.Closed) {
						_trans.Connection.Close();
					}
					_trans.Connection.Dispose();
				}
				_trans.Dispose();
			}
		}

		#endregion
	}

}