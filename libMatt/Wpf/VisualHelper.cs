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


		// FindChild method courtesy of CrimsonX:
		// http://stackoverflow.com/a/1759923/81554
		/// <summary>
		/// Finds a Child of a given item in the visual tree. 
		/// </summary>
		/// <param name="parent">A direct parent of the queried item.</param>
		/// <typeparam name="T">The type of the queried item.</typeparam>
		/// <param name="childName">x:Name or Name of child. </param>
		/// <returns>The first parent item that matches the submitted type parameter. 
		/// If not matching item can be found, 
		/// a null parent is being returned.</returns>
		public static T FindChild<T>(DependencyObject parent, string childName)
		   where T : DependencyObject {
			// Confirm parent and childName are valid. 
			if (parent == null)
				return null;

			T foundChild = null;

			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++) {
				var child = VisualTreeHelper.GetChild(parent, i);
				// If the child is not of the request child type child
				T childType = child as T;
				if (childType == null) {
					// recursively drill down the tree
					foundChild = FindChild<T>(child, childName);

					// If the child is found, break so we do not overwrite the found child. 
					if (foundChild != null)
						break;
				} else if (!string.IsNullOrEmpty(childName)) {
					var frameworkElement = child as FrameworkElement;
					// If the child's name is set for search
					if (frameworkElement != null && frameworkElement.Name == childName) {
						// if the child's name is of the request name
						foundChild = (T)child;
						break;
					}
				} else {
					// child element found.
					foundChild = (T)child;
					break;
				}
			}

			return foundChild;
		}


	}
}
