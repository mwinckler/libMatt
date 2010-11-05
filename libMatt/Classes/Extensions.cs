using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace libMatt.Classes {
	public static class Extensions {
		/// <summary>
		/// Compares the properties of two objects and looks for differences. It will return an IEnumerable that contains the parameters that differ. Otherwise it will return null.
		/// </summary>
		/// <param name="originalObject"></param>
		/// <param name="comparisonObject"></param>
		/// <returns>IEnumerable<PropertyInfo></returns>
		public static IEnumerable<PropertyInfo> CompareProperties(this object originalObject, object comparisonObject) {
			Type sourceType = originalObject.GetType();
			Type destinationType = comparisonObject.GetType();
			List<PropertyInfo> different = new List<PropertyInfo>();

			if (sourceType == destinationType) {
				PropertyInfo[] sourceProperties = sourceType.GetProperties();
				foreach (PropertyInfo pi in sourceProperties) {
					if ((sourceType.GetProperty(pi.Name).GetValue(originalObject, null) == null && destinationType.GetProperty(pi.Name).GetValue(comparisonObject, null) == null)) {
						//Both equal null, may want to capture this
					} else if (!(sourceType.GetProperty(pi.Name).GetValue(originalObject, null).ToString() == destinationType.GetProperty(pi.Name).GetValue(comparisonObject, null).ToString())) {
						different.Add(pi);
					}
				}
			} else {
				throw new ArgumentException("Comparison object must be of the same type.", "comparisonObject");
			}
			if (different.Count > 0)
				return different;
			else
				return null;
		}

		/// <summary>
		/// Returns true if the object implements the specified interface (or derives from the specified class).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool Implements<T>(this object obj) where T: class {
			if (obj == null)
				return false;

			return typeof(T).IsAssignableFrom(obj.GetType());
		}

	}
}
