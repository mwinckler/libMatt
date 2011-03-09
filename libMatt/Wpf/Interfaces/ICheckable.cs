using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libMatt.Wpf.Interfaces {
	public interface ICheckable : System.ComponentModel.INotifyPropertyChanged {
		bool IsChecked { get; set; }
		event EventHandler CheckedChanged;
	}
}
