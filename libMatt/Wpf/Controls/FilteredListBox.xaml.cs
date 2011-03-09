using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using libMatt.Wpf.Interfaces;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Controls.Primitives;

namespace libMatt.Wpf {
	/// <summary>
	/// Interaction logic for FilteredListBox.xaml
	/// </summary>
	public partial class FilteredListBox : ListBox {
		private class Checkable : ICheckable {

			public Checkable(object sourceObject) {
				SourceObject = sourceObject;
			}

			public object SourceObject { get; set; }

			public override string ToString() {
				return SourceObject.ToString();
			}

			#region ICheckable Members

			private bool _is_checked;
			public bool IsChecked {
				get { return _is_checked; }
				set {
					if (value != _is_checked) {
						_is_checked = value;
						this.OnPropertyChanged("IsChecked");
						if (null != this.CheckedChanged)
							this.CheckedChanged(this, EventArgs.Empty);
					}
				}
			}

			public event EventHandler CheckedChanged;

			#endregion

			#region INotifyPropertyChanged Members

			public event PropertyChangedEventHandler PropertyChanged;

			private void OnPropertyChanged(string propertyName) {
				if (null != this.PropertyChanged)
					this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}

			#endregion
		}

		public enum FilteredListBoxMode {
			RadioButton,
			CheckBox,
			None
		}

		private bool _is_select_all_visible;
		private bool _is_filter_visible = true;
		private FilteredListBoxMode _mode = FilteredListBoxMode.None;

		public FilteredListBox() {
			InitializeComponent();

		}


		#region Item Management

		private ObservableCollection<ICheckable> _checkable_collection;
		private ObservableCollection<ICheckable> CheckableCollection {
			get {
				if (null == _checkable_collection)
					_checkable_collection = new ObservableCollection<ICheckable>();

				return _checkable_collection;
			}
		}

		private IEnumerable _original_collection;

		public new IEnumerable ItemsSource {
			get {
				return _original_collection;
			}
			set {

				// Important note: This "_checkable_collection" business will be
				// problematic if:
				//		1. _original_collection is observable, and
				//		2. _original_collection's contents change, and
				//		3. The user expects this listbox to reflect those changes.
				//
				// TODO: Observe _original_collection (if observable) and update 
				//		_checkable_collection upon change.

				_checkable_collection = new ObservableCollection<ICheckable>();
				ICheckable chk;
				foreach (object obj in value) {
					chk = obj as ICheckable;
					if (null == chk) {
						chk = new Checkable(obj);
					}
					_checkable_collection.Add(chk);
				}

				_original_collection = value;

				base.ItemsSource = _checkable_collection;
				IsSelectAllEnabled = IsSelectAllVisible = (Mode == FilteredListBoxMode.CheckBox);
			}
		}

		public new IEnumerable<ICheckable> ItemsSourceCheckable {
			get { return _checkable_collection; }
		}


		#endregion

		//------------ Properties ------------------//
		#region Behavior/Widget Management

		public bool IsFilterVisible {
			get { return _is_filter_visible; }
			set {
				if (value != _is_filter_visible) {
					_is_filter_visible = value;
					this.OnPropertyChanged("IsFilterVisible");
				}
			}
		}

		public bool IsSelectAllVisible {
			get {
				return _is_select_all_visible;
			}
			set {
				if (value != _is_select_all_visible) {
					_is_select_all_visible = value;
					this.OnPropertyChanged("IsSelectAllVisible");
				}
			}
		}

		private bool _is_select_all_enabled = true;
		public bool IsSelectAllEnabled {
			get {
				return _is_select_all_enabled;
			}
			set {
				if (value != _is_select_all_enabled) {
					_is_select_all_enabled = value;
					this.OnPropertyChanged("IsSelectAllEnabled");
				}
			}
		}

		/// <summary>
		/// This property cannot be set at runtime. Switching the template results 
		/// in odd behavior (the ToggleButtons behave as whichever Mode was selected
		/// at design-time, regardless of the run-time setting, even though the UI
		/// widgets get rendered correctly.)
		/// </summary>
		public FilteredListBoxMode Mode {
			get { return _mode; }
			set {
				if (value != _mode) {
					_mode = value;
					// Set the control template based on mode.
					switch (_mode) {
						case FilteredListBoxMode.CheckBox:
							this.ItemTemplate = ((DataTemplate)this.Resources["checkBoxItem"]);
							IsSelectAllEnabled = true;
							break;
						case FilteredListBoxMode.RadioButton:
							/* MAW.20090707: All is vanity. Forget switching at runtime.
							IsSelectAllEnabled = false;
							// There might be multiple selected at this point, but that's not
							// allowed with radiobuttons. Deselect all but the first.
							if (null != _checkable_collection) {
								bool first = true;
								foreach (var item in _checkable_collection) {
									if (first)
										first = false;
									else
										item.IsChecked = false;
								}
							}
							 */

							this.ItemTemplate = ((DataTemplate)this.Resources["radioButtonItem"]);

							break;
						default:
							this.ItemTemplate = null;
							break;
					}
				}
			}
		}

		#endregion


		/// <summary>
		/// Returns all items in the ItemsSource that implement ICheckable.
		/// </summary>
		protected IEnumerable<ICheckable> GetCheckables() {
			// This method used to do more to get the checkables, 
			// but now that logic is encapsulated in the CheckableCollection.
			return CheckableCollection;
		}


		public IEnumerable<object> CheckedItems() {
			List<int> throwaway;
			return CheckedItems(out throwaway);
		}

		/// <summary>
		/// Gets an IEnumerable of the currently-checked items in the listbox
		/// (including ones not currently visible due to the filter). Also returns
		/// the corresponding ListBox indices in the out parameter.
		/// </summary>
		/// <param name="indices"></param>
		/// <returns></returns>
		public IEnumerable<object> CheckedItems(out List<int> indices) {
			List<object> ret = new List<object>();
			indices = new List<int>();

			//ListBoxItem listItem = null;
			for (int i = 0; i < this.Items.Count; i++) {
				if (((ICheckable)this.Items[i]).IsChecked) {
					ret.Add(this.Items[i]);
					indices.Add(i);
				}
			}

			return ret;
		}


		#region ListBoxItem / Template helpers

		protected bool IsChecked(ListBoxItem item) {
			// MAW.200900707: Obsoleted code now that all child items are ICheckables.
			//					Keeping it here for now in case it becomes necessary again.
			// Check to see if the datatemplate is applied, and if so, find a togglebutton
			/*
			var cp = VisualTreeHelper.GetChild(item, 0) as ContentPresenter;
			object chk = item.ContentTemplate.FindName("chk", cp);

			if (chk is ToggleButton) {
			 */
			return (((ICheckable)item.Content)).IsChecked;
			/*
			ToggleButton chk = this.GetToggleButton(item);
			if (chk != null)
				return chk.IsChecked.GetValueOrDefault();

			return false;
			 */
		}

		/// <summary>
		/// Sets all items' IsChecked property to false.
		/// </summary>
		public void ClearChecks() {
			for (int i = 0; i < this.Items.Count; i++) {
				((ICheckable)this.Items[i]).IsChecked = false;
			}
		}


		protected ListBoxItem GetListBoxItem(int itemIndex) {
			if (itemIndex > -1 && itemIndex < this.Items.Count)
				return (ListBoxItem)this.ItemContainerGenerator.ContainerFromIndex(itemIndex);

			return null;
		}

		protected ToggleButton GetToggleButton(ListBoxItem item) {
			if (null != item) {
				var cp = VisualTreeHelper.GetChild(item, 0) as ContentPresenter;
				object chk = item.ContentTemplate.FindName("chk", cp);

				return chk as ToggleButton;
			}
			return null;
		}

		#endregion




		#region Custom Events

		public event EventHandler<CheckedItemsChangedArgs> CheckedItemsChange;
		public class CheckedItemsChangedArgs : EventArgs {
			/// <summary>
			/// Returns a list of all databound objects that are checked.
			/// </summary>
			public IEnumerable<object> CheckedItems { get; set; }
			public System.Collections.IList SelectedItems { get; set; }
		}

		protected void OnCheckedItemsChange() {
			if (null != CheckedItemsChange)
				CheckedItemsChange(this, new CheckedItemsChangedArgs() {
					CheckedItems = CheckedItems(),
					SelectedItems = SelectedItems
				});
		}

		#endregion



		#region Event Handlers

		private void filterText_TextChanged(object sender, TextChangedEventArgs e) {
			// Filter listbox visible items based on contents of textbox.
			//lbxData.Items.Refresh();
			var box = sender as TextBox;
			if (null != box) {
				this.Items.Filter = delegate(object obj) {
					string str = obj.ToString();
					if (String.IsNullOrEmpty(box.Text))
						return true;
					if (String.IsNullOrEmpty(str))
						return false;
					return (str.ToLower().IndexOf(box.Text.ToLower(), 0) > -1);
				};
			}
		}

		private void ListBox_MouseDown(object sender, MouseButtonEventArgs e) {
			// Deselect any selections...they do not play a role in the elitist FilteredListBox.
			((ListBox)sender).SelectedItem = null;
		}

		private void chk_Checked(object sender, RoutedEventArgs e) {
			var chk = sender as ToggleButton;

			// MAW.20090707: As of today, this conditional should *always* be true.
			//				TODO: Still need to figure out what to do with Mode == None.
			if (chk.Content is ICheckable) {
				((ICheckable)chk.Content).IsChecked =
					(Mode == FilteredListBoxMode.RadioButton ||
					((ICheckable)chk.Content).IsChecked);
				OnCheckedItemsChange();
			} else {
				if (Mode != FilteredListBoxMode.None) {
					OnCheckedItemsChange();
				}
			}
		}

		void FilteredListBox_Selected(object sender, RoutedEventArgs e) {
			((ListBoxItem)sender).IsSelected = false;
			e.Handled = true;
		}

		#endregion

		private void chkSelectAll_Checked(object sender, RoutedEventArgs e) {
			// Check or uncheck all ICheckables in the list.
			var chkSelectAll = sender as CheckBox;
			if (null != chkSelectAll) {
				GetCheckables().ToList().ForEach(
					delegate(ICheckable chk) {
						chk.IsChecked = chkSelectAll.IsChecked.GetValueOrDefault();
					}
				);
			}
		}




		#region ValueConverters

		/// <summary>
		/// A hack to get the textbox to scale/expand while also respecting max width.
		/// </summary>
		[ValueConversion(typeof(double), typeof(double))]
		protected class TextBoxWidthConverter : IValueConverter {

			#region IValueConverter Members

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				// value == stackpanel width
				// parameter == filter label width
				// returns desired width of textbox
				return (double)value - (double)parameter;
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				throw new NotSupportedException("This IValueConverter does not support conversion back.");
			}

			#endregion
		}


		#endregion


		#region Interface Implementations

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName) {
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#endregion

	}
}
