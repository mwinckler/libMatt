using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace libMatt.Formatters {
	public static class AttributeHelper {

		/// <summary>
		/// Attempts to find an attribute of type T on the provided object. If not found, returns null.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this object obj) where T: Attribute {
			if (obj == null)
				return null;

			if (obj is Type) {
				return (from attr in ((Type)obj).GetCustomAttributes(false)
						where attr is T
						select attr as T).DefaultIfEmpty(null).First();
			} else {
				return (from attr in obj.GetType().GetCustomAttributes(false)
						where attr is T
						select attr as T).DefaultIfEmpty(null).First();
			}
		}


		public static T GetAttribute<T>(this Enum obj) where T : Attribute {
			if (obj == null)
				return null;

			T attr = Attribute.GetCustomAttribute(
							obj.GetType().GetField(obj.ToString()),
						typeof(T)) as T;

			return attr;
		}

	}
}
