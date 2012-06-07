using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using libMatt.Converters;

namespace libMattTests {
	[TestFixture]
	public class ConverterTest {

		#region Strings

		[Test]
		public void GetStringFromNull() {
			object obj = null;
			Assert.AreEqual(string.Empty, obj.GetString());
		}

		[Test]
		public void GetStringFromDbNull() {
			var obj = DBNull.Value;
			Assert.AreEqual(string.Empty, obj.GetString());
		}

		[Test]
		public void GetStringFromObject() {
			object str = "something";
			Assert.AreEqual(str, str.GetString());
		}

		[Test]
		public void ParseRangeFromString() {
			var parseResults = "1,2,5-8,12".ParseRanges().ToList();
			Assert.Contains(1, parseResults);
			Assert.Contains(2, parseResults);
			Assert.Contains(5, parseResults);
			Assert.Contains(6, parseResults);
			Assert.Contains(7, parseResults);
			Assert.Contains(8, parseResults);
			Assert.Contains(12, parseResults);
		}

		#endregion

		#region Ints

		[Test]
		public void VerifyDefaultToIntegerIsZero() {
			string str = "not an int";
			Assert.AreEqual(0, str.ToInteger());
		}

		[Test]
		public void GetIntFromNull() {
			object obj = null;
			Assert.AreEqual(0, obj.ToInteger(0));
		}

		[Test]
		public void GetIntFromValidString() {
			var str = "1234";
			Assert.AreEqual(1234, str.ToInteger());
		}

		[Test]
		public void GetIntFromInvalidString() {
			var str = "not an int";
			Assert.AreEqual(0, str.ToInteger());
		}

		[Test]
		public void GetIntFromDouble() {
			double d = 1234.6;
			Assert.AreEqual(1234, d.ToInteger());
		}

		[Test]
		public void GetIntFromNullWithDefault() {
			Assert.AreEqual(4, ((object)null).ToInteger(4));
		}


		[Test]
		public void TryToIntFromNull() {
			Assert.IsNull(DBNull.Value.TryToInteger());
		}

		[Test]
		public void TryToIntFromValidString() {
			Assert.IsTrue("1234".TryToInteger().HasValue);
			Assert.AreEqual(1234, "1234".TryToInteger().Value);
		}

		#endregion


		#region Doubles

		[Test]
		public void GetDoubleFromNull() {
			Assert.AreEqual(0, ((object)null).ToDouble());
		}

		[Test]
		public void GetDoubleFromDbNull() {
			Assert.AreEqual(0, DBNull.Value.ToDouble());
		}

		[Test]
		public void GetDoubleFromInvalidString() {
			Assert.AreEqual(0, "this is not a double".ToDouble());
		}

		[Test]
		public void GetDoubleFromValidString() {
			Assert.AreEqual(12345.6, "12345.6".ToDouble());
		}

		[Test]
		public void GetDoubleFromInt() {
			Assert.AreEqual(12345, 12345.ToDouble());
		}

		[Test]
		public void GetFractionalPartOfDouble() {
			Assert.AreEqual(0.678, Math.Round(12345.678.Frac(), 3));
		}

		#endregion
	}
}
