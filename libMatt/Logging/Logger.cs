using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace libMatt.Logging {

	/// <summary>
	/// This class contains methods useful for logging stuff: events, exceptions, or otherwise.
	/// </summary>
	public static class Logging {

		private static MethodBase GetCaller() {
			// Assume one caller from this class and another from elsewhere
			return GetCaller(2);
		}

		private static MethodBase GetCaller(int offset) {
			offset += 2;    // account for this method and the one calling from this class
			// Probably dangerous code; not checking to see that ndx actually exists on the stacktrace.
			return new System.Diagnostics.StackTrace().GetFrame(offset).GetMethod();
		}

		/// <summary>
		/// MethodIdentifier uses reflection to obtain the name of the calling function
		/// and returns that name in brackets, suitable for prefixing an error statement.
		/// </summary>
		/// <overloads>This method has two overloads.</overloads>
		/// <returns>The calling function's name, in square brackets, all suffixed with a single space.</returns>
		/// <example>
		/// <code>
		///		public void MyFunction() {
		///			try {
		///				var i = 1 / 0;
		///			} catch Exception ex {
		///				Console.WriteLine(MethodIdentifier() + ex.Message);  // Writes: "[MyFunction] exception message..."
		///			}
		///		}
		/// </code>
		/// </example>
		public static string MethodIdentifier() {
			return MethodIdentifier(0);
		}

		/// <summary>
		/// Obtains the name of a function in the call stack and returns that name 
		/// surrounded by square brackets and suffixed with a single space.
		/// </summary>
		/// <param name="offset">The number of steps to traverse the call stack--the higher the number, 
		/// the further back to go--where <c>0</c> represents the function calling <c>MethodIdentifier</c>. </param>
		/// <returns>The name of a function in the call stack, in square brackets, suffixed with a single space.</returns>
		public static string MethodIdentifier(int offset) {
			return "[" + GetCaller(offset).Name + "] ";
		}

		/// <summary>
		/// Obtains an array of the calling function's parameters using reflection.
		/// </summary>
		/// <returns>An array of <c>ParameterInfo</c> objects for the calling function.</returns>
		public static ParameterInfo[] MethodArguments() {
			var method = GetCaller();
			return method.GetParameters();
		}

	}

}
