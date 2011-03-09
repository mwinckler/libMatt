using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace libMatt.Wpf.ValueConverters {

	[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
	public class ColorToSolidBrush: IValueConverter {

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value == null)
				return DependencyProperty.UnsetValue;

			return new SolidColorBrush((Color)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value == null)
				return DependencyProperty.UnsetValue;

			return ((SolidColorBrush)value).Color;
		}

		#endregion
	}
}
