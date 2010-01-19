using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libMatt.Classes {

	/// <summary>
	/// The DynamicDictionary is a generic Dictionary with a twist: when accessing a null
	/// bucket, a new value will be constructed, returned, and added to the Dictionary
	/// instead of throwing an exception. For this reason, V must have a default accessible
	/// parameterless constructor.
	/// </summary>
	/// <typeparam name="K">Key type.</typeparam>
	/// <typeparam name="V">Type of value. This type must have a default accessible parameterless constructor.</typeparam>
	/// <example>
	/// <p>Here is an example of simple usage, a DynamicDictionary&lt;int, string&gt;:</p>
	/// <code lang="cs" tabSize="4">
	///		var dict = new DynamicDictionary&lt;int, List&lt;string&gt;&gt;();
	///		dict[0].Add("a new string");
	///		Console.WriteLine(dict[0][0]);
	///		Console.WriteLine(dict[1].Count);
	///		// Prints:
	///		// "a new string"
	///		// 0
	/// </code>
	/// <p>A more complicated usage can simulate a flexible multidimensional jagged array:</p>
	/// <code>
	///		var dict = new DynamicDictionary&lt;string, DynamicDictionary&lt;int,List&lt;string&gt;&gt;&gt;;
	///		dict["key 1"][4].Add("The parent DynamicDictionary was automatically created, as well as this list itself!");
	/// </code>
	/// </example>
	/// <remarks>
	/// Be aware that in some cases, when there is no value for a key you may not
	/// actually want to instantiate and store a new value. (Sometimes you may simply want to
	/// check to see whether the value exists.) In this case, you can use the
	/// <see cref="System.Collections.Generic.Dictionary.TryGetValue">TryGetValue</see> method
	/// for the desired result. Alternatively, if you want the default value of <c>V</c> in
	/// the event that the bucket is null, you can use <c>DynamicDictionary[key, false]</c>.
	/// </remarks>
	public class DynamicDictionary<K, V> : Dictionary<K, V> where V : new() {
		public DynamicDictionary() { }

		/// <summary>
		/// Retrieves the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>If found in the dictionary, returns the value associated with <c>key</c>.
		/// If no such value exists, creates and returns a new <c>V</c>.</returns>
		public new V this[K key] {
			get {
				V val;

				if (!this.TryGetValue(key, out val)) {
					val = new V();
					base.Add(key, val);
				}
				return val;
			}
			set {
				base[key] = value;
			}
		}

		/// <summary>
		/// Retrieves the value associated with the specified key. If <c>autoCreateItem</c>
		/// is true and no value exists, creates a new <c>V</c>, stores it in bucket <c>key</c>,
		/// and returns it. Otherwise, returns the default value for <c>V</c> (without storing
		/// it in the dictionary).
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="autoCreateItem">Specifies whether or not a new item should be created and
		/// added to the dictionary if a value does not already exist. True == if value does not exist,
		/// a new <c>V</c> will be created, added to the dictionary, and returned. False == if value
		/// does not exist, nothing will be added to the dictionary and <c>default(V)</c> will be returned.</param>
		/// <returns>The value found in bucket <c>key</c>, or if no value exists, either a new <c>V</c>
		/// or <c>default(V)</c> (based on value specified in <c>autoCreateItem</c>).</returns>
		public V this[K key, bool autoCreateItem] {
			get {
				V val = default(V);
				if (!base.TryGetValue(key, out val)) {
					if (autoCreateItem) {
						val = new V();
						base.Add(key, val);
					}
				}

				return val;
			}
			set {
				base[key] = value;
			}
		}

	}

}
