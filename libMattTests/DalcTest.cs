using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.SqlServerCe;
using libMatt.Converters;
using libMatt.Data;

namespace libMattTests {
#if USE_SQLSERVERCE
	[TestFixture]
	public class DalcTest {
		private string _dbFilename = "libMattTest.sdf";
		private string _connstr;
		private SqlCeConnection _conn;
		SqlCeDalc _indepDalc;
		#region Supporting Classes

		protected class SqlCeDalc : Dalc {

			public SqlCeDalc(string connStr): this(new libMatt.Data.SqlCeDataProvider(connStr)) { }
			public SqlCeDalc(IDataProvider provider) : base(provider) { }
			public SqlCeDalc(System.Data.IDbTransaction trans) : base(trans) { }

			public void InsertOneRow(string str) {
				ExecuteNonQuery("insert into test_table (str) values (@str)", new Param("@str", str));
			}

			public int GetRowCount() {
				return ExecuteScalar("select count(id) from test_table;").ToInteger();
			}

			public string GetStrById(int id) {
				return ExecuteScalar("select str from test_table where id = @id;", new Param("@id", id)).GetString();
			}

		}

		#endregion

		public DalcTest() {
			_connstr = string.Format("Data Source={0};Persist Security Info=False;", _dbFilename);
		}

		[TestFixtureSetUp()]
		public void Init() {
			var engine = new SqlCeEngine(_connstr);
			if (System.IO.File.Exists(_dbFilename)) {
				System.IO.File.Delete(_dbFilename);
			}
			engine.CreateDatabase();

			_conn = new SqlCeConnection(_connstr);
			_conn.Open();

			var cmd = new SqlCeCommand("create table test_table (id int identity primary key, str nvarchar(100));", _conn);
			cmd.ExecuteNonQuery();
			cmd.Dispose();

			if (_conn != null) {
				_conn.Close();
				_conn.Dispose();
			}

			_indepDalc = new SqlCeDalc(_connstr);
		}

		[TestFixtureTearDown()]
		public void Cleanup() {
			try {
				_indepDalc.Dispose();
				if (System.IO.File.Exists(_dbFilename)) {
					System.IO.File.Delete(_dbFilename);
				}
			} catch (Exception ex) {

			}

		}

		[Test]
		public void InsertViaTransactionCommit() {
			int rowCount = _indepDalc.GetRowCount();

			using (var dalc = new SqlCeDalc(_connstr)) {
				using (var trans = dalc.BeginTransaction()) {
					dalc.InsertOneRow("a row");

					int newRowCount = dalc.GetRowCount();
					Assert.AreEqual(rowCount + 1, newRowCount);
					trans.Commit();
				}
			}
			// Ensure row was indeed committed
			Assert.AreEqual(rowCount + 1, _indepDalc.GetRowCount());

		}

		[Test]
		public void InsertViaTransactionRollback() {
			int rowCount = _indepDalc.GetRowCount();

			using (var dalc = new SqlCeDalc(_connstr)) {
				using (var trans = dalc.BeginTransaction()) {
					dalc.InsertOneRow("a row");

					int newRowCount = dalc.GetRowCount();
					Assert.AreEqual(rowCount + 1, newRowCount);
					trans.Rollback();
				}

			}
			// Ensure row was NOT committed
			Assert.AreEqual(rowCount, _indepDalc.GetRowCount());
		}

		/// <summary>
		/// Inserts two rows into test_table. 
		/// 
		/// If commit, then commits transaction (expected rowcount == rowcount +2),
		/// else rolls back (expected rowcount == rowcount)
		/// </summary>
		/// <param name="commit"></param>
		private void DoSharedTransactionWork(bool commit) {
			using (var dalc = new SqlCeDalc(_connstr)) {
				using (var trans = dalc.BeginTransaction()) {
					// Do some work in the outer dalc
					dalc.InsertOneRow("outer");

					using (var innerDalc = new SqlCeDalc(trans)) {
						innerDalc.InsertOneRow("inner");
					}

					if (commit) {
						trans.Commit();
					} else {
						trans.Rollback();
					}
				}
			}
		}

		[Test]
		public void InsertViaSharedTransactionCommitOuter() {
			int rowCount = _indepDalc.GetRowCount();

			DoSharedTransactionWork(true);

			Assert.AreEqual(rowCount + 2, _indepDalc.GetRowCount());
		}

		[Test]
		public void InsertViaSharedTransactionRollback() {
			int rowCount = _indepDalc.GetRowCount();

			DoSharedTransactionWork(false);
			
			Assert.AreEqual(rowCount, _indepDalc.GetRowCount());
		}


	}
#endif
}
