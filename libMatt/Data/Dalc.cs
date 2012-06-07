using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using libMatt.Converters;

namespace libMatt.Data {

	public abstract class Dalc: IDisposable {

		protected class Param {

			public Param(string name, object value): this(name, value, ParameterDirection.Input) { }

			public Param(string name, object value, ParameterDirection direction) {
				this.Name = name;
				this.Value = value;
				this.Direction = direction;
			}

			public string Name { get; set; }
			public object Value { get; set; }
			public ParameterDirection Direction { get; set; }
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
			if (!(trans is DalcTransaction)) {
				_trans = new DalcTransaction(this, trans);
			} else {
				_trans = trans as DalcTransaction;
			}
			this.DefaultCommandType = CommandType.Text;
		}

		public Dalc(IDataProvider provider) : this(provider, CommandType.Text) { }

		public Dalc(IDataProvider provider, CommandType defaultCommandType) {
			this.DataProvider = provider;
			this.DefaultCommandType = defaultCommandType;
		}


		public IDataProvider DataProvider { get; private set; }

		/// <summary>
		/// Iterates through the given parameters and assigns values from the IDbCommand's parameters
		/// to the 
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="cmd"></param>
		private void PopulateOutParameters(Param[] parameters, IDbCommand cmd) {
			// Check for out param values
			foreach (var param in parameters) {
				if (cmd.Parameters.Contains(param.Name)) {
					param.Value = ((IDbDataParameter)cmd.Parameters[param.Name]).Value;
				}
			}
		}

		private void HandleException(Exception ex, string commandText, Param[] parameters) {
			List<string> parms = new List<string>();
			foreach (Param p in parameters) {
				parms.Add(p.Name + ":" + p.Value);
			}
			throw new ApplicationException("commandText: " + commandText +
				"; parameters: {" + String.Join(",", parms.ToArray()) + "}", ex);
		}

		protected System.Data.IDbConnection GetConnection() {
			if (_trans != null && !_trans.IsDisposed) {
				return _trans.Connection;
			} else {
				return DataProvider.CreateConnection();
			}
		}


		protected object ExecuteScalar(string commandText, CommandType commandType, params Param[] parameters) {
			var cmd = GetCommand(commandText, parameters);
			object ret = null;
			try {
				cmd.CommandType = commandType;
				ret = cmd.ExecuteScalar();
				PopulateOutParameters(parameters, cmd);
			} finally {
				DisposeCommand(cmd);
			}
			return ret;
		}

		protected object ExecuteScalar(string commandText, params Param[] parameters) {
			return ExecuteScalar(commandText, this.DefaultCommandType, parameters);
		}


		protected void ExecuteNonQuery(string commandText, CommandType commandType, params Param[] parameters) {
			var cmd = GetCommand(commandText, parameters);
			try {
				cmd.CommandType = commandType;
				cmd.ExecuteNonQuery();
				PopulateOutParameters(parameters, cmd);
			} finally {
				DisposeCommand(cmd);
			}
		}

		protected void ExecuteNonQuery(string commandText, params Param[] parameters) {
			ExecuteNonQuery(commandText, this.DefaultCommandType, parameters);
		}

		protected IDataReader ExecuteReader(string commandText, params Param[] parameters) {
			var cmd = GetCommand(commandText, parameters);
			// Open with CommandBehavior.CloseConnection so that connection is closed on reader close.
			var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			PopulateOutParameters(parameters, cmd);
			return reader;
		}

		/// <summary>
		/// Returns a datatable populated with the results of the command.
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		protected DataTable ExecuteDataTable(
				string commandText,
				CommandType commandType,
				params Param[] parameters) {

			IDbDataAdapter da = DataProvider.CreateDataAdapter();
			da.SelectCommand = GetCommand(commandText, commandType, parameters);
			DataSet ds = new DataSet();
			try {
				da.Fill(ds);
				PopulateOutParameters(parameters, da.SelectCommand);
			} finally {
				DisposeCommand(da.SelectCommand);
			}
			return (ds.Tables.Count > 0 ? ds.Tables[0] : null);
		}

		protected DataTable ExecuteDataTable(string commandText, params Param[] parameters) {
			return ExecuteDataTable(commandText, this.DefaultCommandType, parameters);
		}

		protected IDbCommand GetCommand(string commandText, CommandType commandType, params Param[] parameters) {
			IDbCommand cmd = GetConnection().CreateCommand();
			try {
				if (_trans != null) {
					cmd.Transaction = _trans.DbTransaction;
				}
				cmd.CommandText = commandText;
				cmd.CommandType = commandType;
				IDbDataParameter parm;
				foreach (var p in parameters) {
					parm = cmd.CreateParameter();
					parm.ParameterName = p.Name;
					parm.Value = p.Value;
					parm.Direction = p.Direction;
					// If this is an out parameter, the size must be specified.
					if (p.Direction != ParameterDirection.Input) {
						parm.Size = 4000; // varchar max
					}
					cmd.Parameters.Add(parm);
				}
			} catch (Exception ex) {
				DisposeCommand(cmd);
				throw;
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
		private DalcTransaction _trans;

		public IDbTransaction BeginTransaction() {
			if (_trans != null && !_trans.IsDisposed) {
				throw new InvalidOperationException("[Dalc.BeginTransaction] Cannot begin new transaction: a transaction is already in progress.");
			}
			var conn = this.DataProvider.CreateConnection();
			_trans = new DalcTransaction(this, conn.BeginTransaction());
			_isTransOwner = true;
			return _trans;
		}

		public void Commit() {
			if (_trans != null && !_trans.IsDisposed) {
				_trans.Commit();
			}
		}

		public void Rollback() {
			if (_trans != null && !_trans.IsDisposed) {
				_trans.Rollback();
			}
		}

		#endregion


		#region IDisposable Members

		public void Dispose() {
			// Check for existing connection and any open transactions.
			// Only clean up the transaction if this dalc was the owner.
			if (_isTransOwner && _trans != null && !_trans.IsDisposed) {
				if (_trans.NeedsCommit) {
					this.Commit();
				}
				if (_trans.Connection != null) {
					if (_trans.Connection.State != ConnectionState.Closed) {
						_trans.Connection.Close();
					}
					_trans.Connection.Dispose();
				}
				_trans.Dispose();
				_trans = null;
			}

			GC.SuppressFinalize(this);
		}

		#endregion
	}

}