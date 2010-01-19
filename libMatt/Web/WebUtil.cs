using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;



namespace libMatt.Web {
	public static class WebUtil {

		/// <summary>
		/// Returns the server variable REMOTE_ADDR if context and request are present. Else returns null.
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentIP() {
			if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.ServerVariables != null) {
				return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
			} else {
				return null;
			}
		}

		/// <summary>
		/// Checks the file for existence and if found, appends a question mark and unix timestamp to the filename based on last modified date.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private static string GetVersionedFilename(string filename) {
			if (File.Exists(filename)) {
				return filename + "?" + ((int)((File.GetLastWriteTime(filename) - new DateTime(1970, 1, 1)).TotalSeconds)).ToString();
			}
			return filename;
		}

		/// <summary>
		/// Returns an autoversioned reference to the specified resource, appending a querystring consisting of the
		/// Unix timestamp of the resource's last modified date. For example, passing "js/jquery-1.3.2.js" to this routine
		/// might return "js/jquery-1.3.2.js?1263925780"; the query string would change each time jquery-1.3.2.js is modified.
		/// </summary>
		/// <param name="filename">The resource to reference, as a path relative to the application root. Example: "js/jquery-1.3.2.js"</param>
		/// <returns></returns>
		public static string Autoversion(string filename) {
			if (HttpContext.Current == null || HttpContext.Current.Request == null) {
				return filename;
			} else {
				string absFilename = HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"];
				string dir = Path.GetDirectoryName(filename);

				filename = dir + "/" + Path.GetFileName(GetVersionedFilename(Path.Combine(absFilename, filename)));
				return VirtualPathUtility.ToAbsolute("~/") + filename.Replace(Path.DirectorySeparatorChar, '/');
			}
		}

	}
}
