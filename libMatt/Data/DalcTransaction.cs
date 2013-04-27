using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace libMatt.Data {
	/// <summary>
	/// DalcTransaction wraps an actual DbTransaction in order to track
	/// whether the transaction has been committed. By default, Dalcs commit
	/// uncommitted transactions on disposal, and this wrapper class provides
	/// that functionality.
	/// </summary>
	internal class DalcTransaction: IDbTransaction {

		public DalcTransaction(Dalc parent, IDbTransaction databaseTransaction) {
			if (parent == null) {
				throw new ArgumentNullException("parent");
			}
			if (databaseTransaction == null) {
				throw new ArgumentNullException("databaseTransaction");
			}

			_dbTrans = databaseTransaction;
			_parentDalc = parent;
		}

		private IDbTransaction _dbTrans;
		private Dalc _parentDalc;

		public bool IsCommitted { get; protected set; }
		public bool IsRolledBack { get; protected set; }
		internal bool IsDisposed { get; private set; }

		internal bool NeedsCommit {
			get {
				return !(IsCommitted || IsRolledBack);
			}
		}

		internal IDbTransaction DbTransaction {
			get { return _dbTrans; }
		}

		#region IDbTransaction Members

		public void Commit() {
			_dbTrans.Commit();
			this.IsCommitted = true;
		}

		public IDbConnection Connection {
			get { return _dbTrans.Connection; }
		}

		public IsolationLevel IsolationLevel {
			get { return _dbTrans.IsolationLevel; }
		}

		public void Rollback() {
			_dbTrans.Rollback();
			this.IsRolledBack = true;
		}

		#endregion

		#region IDisposable Members

		public void Dispose() {
			this.IsDisposed = true;
			_dbTrans.Dispose();
		}

		#endregion
	}
}
