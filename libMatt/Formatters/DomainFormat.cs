using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace libMatt.Formatters {
	public static class DomainFormat {

		private static readonly Regex rePattern = new Regex(@"(\{+)([^\}]+)(\}+)", RegexOptions.Compiled);
		/// <summary>
		/// Formats a string using named parameters with anonymous or domain types as the argument.
		/// </summary>
		/// <example>
		/// <code>Format("You are {age} years old and your last name is {name} ", new {age = 18, name = "Foo"});</code>
		/// </example>
		/// <remarks>
		/// Courtesy of Marc Gravell: 
		/// http://stackoverflow.com/questions/1322037/how-can-i-create-a-more-user-friendly-string-format-syntax/1322103#1322103
		/// </remarks>
		/// <param name="pattern"></param>
		/// <param name="template"></param>
		/// <returns></returns>
		public static string Format(this string pattern, object template) {
			if (template == null)
				throw new ArgumentNullException();
			Type type = template.GetType();
			var cache = new Dictionary<string, string>();
			return rePattern.Replace(pattern, match => {
				int lCount = match.Groups[1].Value.Length,
					rCount = match.Groups[3].Value.Length;
				if ((lCount % 2) != (rCount % 2))
					throw new InvalidOperationException("Unbalanced braces");
				string lBrace = lCount == 1 ? "" : new string('{', lCount / 2),
					rBrace = rCount == 1 ? "" : new string('}', rCount / 2);

				string key = match.Groups[2].Value, value;
				if (lCount % 2 == 0) {
					value = key;
				} else {
					if (!cache.TryGetValue(key, out value)) {
						var prop = type.GetProperty(key);
						if (prop == null) {
							throw new ArgumentException("Not found: " + key, "pattern");
						}
						value = Convert.ToString(prop.GetValue(template, null));
						cache.Add(key, value);
					}
				}
				return lBrace + value + rBrace;
			});
		}

	}
}
