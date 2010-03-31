using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.ComponentModel;

namespace libMatt.Formatters {
	public static class Language {

		#region Strings and words

		/// <summary>
		/// Returns the contents of the IEnumerable formatted as a grammatically-correct
		/// English list, such as "itemA, itemB, and itemN" or "itemA and itemB".
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static string ToSentence(this IEnumerable<string> list) {
			if (null == list)
				return "";
			int count = list.Count();
			switch (count) {
				case 0:
					return "";
				case 1:
					return list.First();
				case 2:
					return String.Join(" and ", list.ToArray());
				default:
					return String.Join(", ", list.Take(count - 1).ToArray())
						+ " and " + list.ElementAt(count - 1);
			}
		}

		/// <summary>
		/// Returns the specified string with the first letter of every word capitalized.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string TitleCase(string str) {
			return new CultureInfo("en").TextInfo.ToTitleCase(str.ToLower());
		}

		public static string Pluralize(int count, string singular) {
			return count.ToString() + " " + GetPlural(count, singular);
		}

		public static string Pluralize(int count, string singular, string plural) {
			return count.ToString() + " " + GetPlural(count, singular, plural);
		}

		public static string GetPlural(int count, string singular) {
			return GetPlural(count, singular, GetPlural(singular));
		}

		public static string GetPlural(int count, string singular, string plural) {
			return (count > 1 || count == 0 ? plural : singular);
		}

		/// <summary>
		/// Finds the plural of the singular word, or if none has
		/// been stored, just returns singular + 's'.
		/// </summary>
		/// <remarks>
		/// TODO: return word in proper case (as passed in).
		/// </remarks>
		/// <param name="singular"></param>
		/// <returns></returns>
		private static string GetPlural(string singular) {
			string p;
			if (_pluralLookup.TryGetValue(singular.ToLower(), out p)) {
				return p;
			} else {
				return singular + "s";
			}
		}

		private static Dictionary<string, string> _pluralLookup = new Dictionary<string,string>();

		/// <summary>
		/// Specify any exceptions to the general "word + 's'" rule in a dictionary, where Key is the singular and Value is the correct plural form.
		/// </summary>
		/// <param name="exceptions"></param>
		public static void InitializePluralityExceptions(Dictionary<string, string> exceptions) {
			if (exceptions == null)
				throw new ArgumentNullException("Argument 'exceptions' cannot be null.");

			_pluralLookup = exceptions;
		}

		#endregion

		#region Dates


		/// <summary>
		/// Converts the timespan to a "conversational" date: "X days, Y hours, Z minutes."
		/// Omits any part with a zero value.
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static string GetTimeConversational(TimeSpan time) {
			List<string> parts = new List<string>();
			Action<int, string> addPart = delegate(int qty, string part) {
				if (qty > 0)
					parts.Add(Pluralize(qty, part));
			};
			// If the time is less than a minute, quote seconds.
			if (time.TotalSeconds < 60) {
				addPart(time.Seconds, "second");
			} else {
				// Else, quote the rest.
				addPart(time.Days, "day");
				addPart(time.Hours, "hour");
				addPart(time.Minutes, "minute");
			}
			return String.Join(", ", parts.ToArray());
		}

		#endregion


		#region Attribute readers

		/// <summary>
		/// Searches for DescriptionAttribute attached to the specified enum value. If none found, returns the value.ToString().
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string GetDescription(this Enum obj) {
			if (null == obj)
				return "";

			DescriptionAttribute da =
				Attribute.GetCustomAttribute(
					obj.GetType().GetField(obj.ToString()),
					typeof(DescriptionAttribute))
				as DescriptionAttribute;
			if (da != null) {
				return da.Description;
			} else {
				return obj.ToString();
			}

		}

		#endregion

	}

}
