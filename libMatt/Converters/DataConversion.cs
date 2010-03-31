using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libMatt.Converters {
	public static class DataConversion {

		#region Primitives and strings

		/// <summary>
		/// Safely gets a string representation of the object, returning string.Empty for null or DBNull.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string GetString(this object obj) {
			return (null == obj || DBNull.Value == obj ? string.Empty : obj.ToString());
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
				foreach (string name in Enum.GetNames(enumType)) {
					if (str.ToLower() == name.ToLower()) {
						return true;
					}
				}
			}
			return false;

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
