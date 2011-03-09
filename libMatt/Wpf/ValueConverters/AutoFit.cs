using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace libMatt.Wpf.ValueConverters {


	public class RemainingHeight : IMultiValueConverter {

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			// value needs to be a Panel, the parent container
			// parameter should be the FrameworkElement wanting height
			var parent = value as Panel;
			if (null != parent) {
				double remainingHeight = parent.ActualHeight;
				foreach (FrameworkElement child in parent.Children) {
					if (child != parameter) {
						remainingHeight -= child.ActualHeight;
					}
				}
				return remainingHeight;
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion

		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			// value needs to be a Panel, the parent container
			// parameter should be the FrameworkElement wanting height
			var parent = values[0] as Panel;
			if (null != parent) {
				double remainingHeight = parent.ActualHeight;
				foreach (FrameworkElement child in parent.Children) {
					if (child != values[1]) {
						remainingHeight -= child.ActualHeight;
					}
				}
				return remainingHeight;
			}

			return DependencyProperty.UnsetValue;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}

	/// <summary>
	/// This converter calculates the correct width of an element to consume
	/// the remaining availble space in a StackPanel. The <c>value</c> argument
	/// to <see cref="AutoFit.Convert">Convert</see> should be the parent
	/// <see>StackPanel</see>; the <c>parameter</c> argument should be the
	/// element needing a size.
	/// </summary>
	[ValueConversion(typeof(System.Windows.Controls.StackPanel), typeof(double))]
	public class AutoFit : IMultiValueConverter {

		#region IValueConverter Members

		public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			// value is parent StackPanel
			// parameter is target element (the one needing a size)
			// return is a double specifying the remaining available space in the StackPanel. Note that the StackPanel
			//		must actually have a width assigned to it in order for this to work (else it expands infinitely).
			StackPanel panel = value[0] as StackPanel;
			Control ctrl = value[1] as Control;
			FrameworkElement childCtrl = null;

			if (null == panel) {
				return ctrl.ActualWidth;
			} else {
				if (panel.Orientation == Orientation.Horizontal) {
					// calculate actualwidth
					double w = 0;

					foreach (var child in panel.Children) {
						childCtrl = child as FrameworkElement;
						if (childCtrl != null && childCtrl != ctrl) {
							w += childCtrl.ActualWidth;
						}
					}
					return panel.ActualWidth - w;
				} else {
					// calculate actualheight
					double h = 0;

					foreach (var child in panel.Children) {
						childCtrl = child as FrameworkElement;
						if (childCtrl != null && childCtrl != ctrl) {
							h += childCtrl.ActualHeight;
						}
					}
					return panel.ActualHeight - h;
				}
			}

		}

		public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotSupportedException();
		}

		#endregion
	}

}
