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



		/// <summary>
		/// Merges the source dictionary onto the target dictionary and returns the result.
		/// This function creates a new dictionary and returns the result in that, leaving 
		/// mergeTarget unmodified.
		/// </summary>
		/// <typeparam name="K">(Generic Type) The type of the dictionary key.</typeparam>
		/// <typeparam name="V">(Generic Type) The type of the dictionary value.</typeparam>
		/// <param name="mergeTarget">(Dictionary&lt;K, V&gt;) The dictionary onto which to merge values.</param>
		/// <param name="mergeSource">(Dictinoary&lt;K, V&gt;) The dictionary from which to take values for merge. Key-values from source override key-values from target.</param>
		/// <returns>A dictionary representing the union of the two dictionaries, with source overriding target where key collisions occur..</returns>
		public static Dictionary<K, V> Merge<K, V>(this Dictionary<K, V> mergeTarget, Dictionary<K, V> mergeSource) {
			return Merge(mergeTarget, mergeSource, false);
		}


		/// <summary>
		/// Merges the source dictionary onto the target dictionary and returns the result.
		/// If inPlace is specified true, this function modifies mergeTarget itself, else
		/// it creates a new dictionary and returns the result in that, leaving mergeTarget
		/// unmodified.
		/// </summary>
		/// <typeparam name="K">(Generic Type) The type of the dictionary key.</typeparam>
		/// <typeparam name="V">(Generic Type) The type of the dictionary value.</typeparam>
		/// <param name="mergeTarget">(Dictionary&lt;K, V&gt;) The dictionary onto which to merge values.</param>
		/// <param name="mergeSource">(Dictinoary&lt;K, V&gt;) The dictionary from which to take values for merge. Key-values from source override key-values from target.</param>
		/// <param name="inPlace">(bool) Whether to perform the merge in-place on mergeTarget. If true, this function modifies mergeTarget itself.</param>
		/// <returns>A dictionary representing the union of the two dictionaries, with source overriding target where key collisions occur..</returns>
		public static Dictionary<K, V> Merge<K, V>(this Dictionary<K, V> mergeTarget, Dictionary<K, V> mergeSource, bool inPlace) {
			var ret = (inPlace ? mergeTarget : new Dictionary<K,V>(mergeTarget));

			foreach (var kv in mergeSource) {
				ret[kv.Key] = kv.Value;
			}
			return ret;
		}

	}
}
