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
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace libMatt.Wpf {
	/// <summary>
	/// Interaction logic for MessageBar.xaml
	/// </summary>
	public partial class MessageBar : UserControl, INotifyPropertyChanged {

		public MessageBar() {
			InitializeComponent();
			this.DataContext = this;
			this.Loaded += (sndr, ev) => {
				this.Visibility = Visibility.Collapsed;
			};

		}

		private MessageType _messageType;
		public MessageType MessageType {
			get {
				return _messageType;
			}
			set {
				if (value != _messageType) {
					_messageType = value;
					this.OnPropertyChanged("MessageType");
				}
			}
		}

		private DoubleAnimation _opacityAnim = new DoubleAnimation(1.0, 0.0, TimeSpan.FromSeconds(2), FillBehavior.Stop) {
			BeginTime = TimeSpan.FromSeconds(3)
		};

		private void FadeOutCompleted(object sender, EventArgs e) {
			this.Visibility = Visibility.Collapsed;
		}

		public void Show(string msg, MessageType messageType, bool persist) {
			Action showMsg = () => {
				this.message.Text = msg;
				this.MessageType = messageType;

				_opacityAnim.Completed -= FadeOutCompleted;
				msgBorder.BeginAnimation(Border.OpacityProperty, null);
				this.msgBorder.Opacity = 1;


				if (!persist) {
					// Start the countdown/animation.
					_opacityAnim.Completed += FadeOutCompleted;
					msgBorder.BeginAnimation(Border.OpacityProperty, _opacityAnim);
				}
				this.Visibility = Visibility.Visible;
			};
			if (!this.Dispatcher.CheckAccess()) {
				this.Dispatcher.BeginInvoke(showMsg, null);
			} else {
				showMsg();
			}
		}



		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) {
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			// Hide.
			this.Visibility = Visibility.Collapsed;
			msgBorder.BeginAnimation(Border.OpacityProperty, null);
		}

	}


	public enum MessageType {
		success,
		error,
		warning,
		info
	}
}
