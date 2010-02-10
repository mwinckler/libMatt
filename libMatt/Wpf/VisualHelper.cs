using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace libMatt.Wpf {
	public class VisualHelper {

		/// <summary>
		/// Searches up the visual tree for a parent control of type T and returns the
		/// first one found, or null if none found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T VisualUpwardSearch<T>(DependencyObject source) where T : class {
			while (source != null && source.GetType() != typeof(T))
				source = VisualTreeHelper.GetParent(source);

			return source as T;
		}

		/// <summary>
		/// Returns the control of type T that is under the originating context menu item.
		/// It is safe to pass a null reference or non-menu item into this function; in
		/// such cases (or in cases where no suitable sending control&lt;T&gt; is found) it
		/// will return null.
		/// </summary>
		/// <param name="contextMenuItem"></param>
		/// <returns>The nearest control of type T that is beneath the sending menu item's context menu's originating point (PlacementTarget).</returns>
		public static T GetSendingControl<T>(MenuItem sender) where T : class {
			if (sender != null && sender.Parent as ContextMenu != null) {
				return VisualUpwardSearch<T>(((ContextMenu)sender.Parent).PlacementTarget);
			}
			return null;
		}

	}
}
