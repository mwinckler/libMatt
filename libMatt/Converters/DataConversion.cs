using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace libMatt.Converters {
	public static class DataConversion {

		#region Primitives and strings

		/// <summary>
		/// If the string is empty/null, returns DBNull.Value, else returns the string.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static object OrDbNull(this string str) {
			return (string.IsNullOrEmpty(str) ? (object)DBNull.Value : str);
		}

		/// <summary>
		/// Safely gets a string representation of the object, returning string.Empty for null or DBNull.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string GetString(this object obj) {
			return (null == obj || DBNull.Value == obj ? string.Empty : obj.ToString());
		}

		/// <summary>
		/// Parses integers from a string containing numbers that
		/// may be separated by commas (indicating single values) or
		/// hyphens (indicating range). For example, "1,2,5-8,12" would
		/// return { 1,2,5,6,7,8,12 }. Non-integer entries in the string will be ignored.
		/// </summary>
		/// <param name="str">(string) The string of numeric (integral) ranges to parse.</param>
		/// <returns>(IEnumerable&lt;int&gt;) The integers parsed from the string.</returns>
		public static IEnumerable<int> ParseRanges(this string str) {
			int i, j;
			string[] ar;
			foreach (var entry in str.Split(',')) {
				ar = entry.Split('-');
				if (int.TryParse(ar[0].Trim(), out i)) {
					if (ar.Length > 1) {
						if (int.TryParse(ar[1].Trim(), out j)) {
							foreach (var k in Enumerable.Range(i, j - i + 1)) {
								yield return k;
							}
						}
					} else {
						// This is a single number.
						yield return i;
					}
				}
			}
		}


		/// <summary>
		/// Returns a new Nullable&lt;int&gt; if the object is null or DBNull.Value, 
		/// else returns the value of obj.ToInteger().
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static int? TryToInteger(this object obj) {
			if (obj == null || obj == DBNull.Value)
				return null;
			return obj.ToInteger();
		}

		/// <summary>
		/// Attempts to convert the object into an integer.
		/// If the object is a double, returns the truncated integer value.
		/// If parsing fails, returns 0.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static int ToInteger(this object obj) {
			return obj.ToInteger(0);
		}

		/// <summary>
		/// Attempts to convert the object into an integer.
		/// If the object is a double, returns the truncated integer value.
		/// If parsing fails, returns defaultValue.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static int ToInteger(this object obj, int defaultValue) {
			if (obj == null || obj == DBNull.Value)
				return defaultValue;
			double d;
			if (double.TryParse(obj.ToString(), out d)) {
				return (int)Math.Floor(d);
			} else {
				int i = 0;
				return (int.TryParse(GetString(obj), out i) ? i : defaultValue);
			}
		}

		/// <summary>
		/// Attempts to parse a long out of the provided value. If unable to parse
		/// a valid long, returns a new Nullable&lt;long&gt;, else returns the parsed value.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static long? TryToLong(this object obj) {
			if (obj == null || obj == DBNull.Value) {
				return null;
			}
			long x;
			if (long.TryParse(obj.ToString(), out x)) {
				return x;
			} else {
				return null;
			}
		}

		/// <summary>
		/// Attempts to convert the object to a long.
		/// If the object is a double, returns the truncated value as a long value.
		/// If the object cannot be parsed as a long, returns 0.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static long ToLong(this object obj) {
			return obj.ToLong(0);
		}

		/// <summary>
		/// Attempts to convert the object to a long.
		/// If the object is a double, returns the truncated value as a long value.
		/// If the object cannot be parsed as a long, returns defaultValue.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static long ToLong(this object obj, long defaultValue) {
			if (obj == null || obj == DBNull.Value)
				return defaultValue;
			double d;
			if (double.TryParse(obj.ToString(), out d)) {
				return (long)Math.Floor(d);
			} else {
				long i = 0;
				return (long.TryParse(GetString(obj), out i) ? i : defaultValue);
			}
		}

		/// <summary>
		/// Attempts to convert the object to a double. On failure, returns 0.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static double ToDouble(this object obj) {
			return ToDouble(obj, 0.0d);
		}

		/// <summary>
		/// Attempts to convert the object to a double. On failure, returns defaultValue.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static double ToDouble(this object obj, double defaultValue) {
			double d = 0;
			return (double.TryParse(GetString(obj), out d) ? d : defaultValue);
		}

		/// <summary>
		/// Attempts to convert the object to a decimal. On failure, returns 0.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static decimal ToDecimal(this object obj) {
			return ToDecimal(obj, 0.0m);
		}


		/// <summary>
		/// Attempts to convert the object to a decimal. On failure, returns defaultValue.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static decimal ToDecimal(this object obj, decimal defaultValue) {
			decimal d = 0m;
			return (decimal.TryParse(GetString(obj), out d) ? d : defaultValue);
		}

		/// <summary>
		/// Attempts to convert the object to a boolean. Will return true for any value starting with 'y' or 't' (case-insensitive), or for a value of '1'.
		/// Else returns false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool ToBoolean(this object obj) {
			if (null == obj || obj == DBNull.Value)
				return false;
			else {
				string s = obj.ToString().ToLower();
				return (s.StartsWith("y") || "1" == s || s.StartsWith("t"));
			}
		}

		/// <summary>
		/// Attempts to convert the object to a DateTime, returning a new Nullable&lt;DateTime&gt; on failure.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static DateTime? ToDateTime(this object obj) {
			if (obj == null || obj == DBNull.Value) {
				return null;
			} else {
				DateTime d;
				return (DateTime.TryParse(GetString(obj), out d) ? (DateTime?)d : null);
			}
		}

		/// <summary>
		/// This method returns the fractional part of a real number.
		/// </summary>
		/// <param name="double"></param>
		/// <returns>double</returns>
		public static double Frac(this double realnum) {
			return (realnum - Math.Truncate(realnum));
		}

		#endregion

		#region Enums

		/// <summary>
		/// Attempts to parse obj into a T. If unable to parse, returns default(T).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		private static T GetEnumType<T>(object obj) {
			string str = GetString(obj);

			if (!IsEnumType(str, typeof(T)) || string.IsNullOrEmpty(str)) {
				return default(T);
			}
			return (T)Enum.Parse(typeof(T), str, true);
		}

		private static bool IsEnumType(string str, Type enumType) {
			if (!String.IsNullOrEmpty(str)) {
				int i;
				if (int.TryParse(str, out i) && Enum.IsDefined(enumType, i)) {
					return true;
				}
				return Enum.IsDefined(enumType, str);
			}
			return false;
		}

		public static T ToEnum<T>(this object obj) {
			string str = GetString(obj);
			if (!IsEnumType(str, typeof(T)))
				throw new ArgumentException("The specified object cannot be converted to type " + typeof(T).Name + ".");
			return GetEnumType<T>(obj);
		}

		/// <summary>
		/// Attempts to parse <c>obj</c> as a <c>T</c>. If success, populates <c>value</c> with the parsed value. Else, returns false.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryParseEnum<T>(this object obj, out T value) {
			value = default(T);
			if (IsEnumType(GetString(obj), typeof(T))) {
				value = GetEnumType<T>(obj);
				return true;
			} else {
				return false;
			}

		}

		#endregion

	}
}
