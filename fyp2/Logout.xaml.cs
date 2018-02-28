using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for Logout.xaml
	/// </summary>
	public partial class Logout : Window
	{
		KinectSensorChooser kinectSensorChooser;
		private HandPointer capturedHandPointer;

		private int playerID;
		private bool loggedOut = false;

		public Logout(KinectSensorChooser kinectSensorChooser, int playerID)
		{
			InitializeComponent();

			//	Set window to center of screen
			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			//	Set the values
			this.playerID = playerID;
			this.kinectSensorChooser = kinectSensorChooser;

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			#region KinectRegion
			//	Setup Kinect region press target and event handlers
			KinectRegion.SetIsPressTarget(btnBack, true);
			KinectRegion.SetIsPressTarget(btnLogout, true);

			//	btnBack
			KinectRegion.AddHandPointerEnterHandler(btnBack, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnBack, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnBack, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnBack, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnBack, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnBack, HandPointerLostCaptureEvent);

			//	btnLogout
			KinectRegion.AddHandPointerEnterHandler(btnLogout, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnLogout, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnLogout, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnLogout, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnLogout, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnLogout, HandPointerLostCaptureEvent);

			#endregion
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(btnBack) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnBack, "MouseOver", true);
			}
			else if (e.HandPointer.GetIsOver(btnLogout) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnLogout, "MouseOver", true);
			}

			e.Handled = true;
		}

		private void HandPointerLeaveEvent(object sender, HandPointerEventArgs e)
		{
			if (!e.HandPointer.GetIsOver(btnBack) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnBack, "Normal", true);
			}
			if (!e.HandPointer.GetIsOver(btnLogout) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnLogout, "Normal", true);
			}

			if (capturedHandPointer == e.HandPointer)
			{
				capturedHandPointer = null;
				e.Handled = true;
			}
		}

		private void HandPointerPressEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == null && e.HandPointer.IsPrimaryHandOfUser && e.HandPointer.IsPrimaryHandOfUser)
			{
				if (e.HandPointer.GetIsOver(btnBack))
				{
					e.HandPointer.Capture(btnBack);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
				else if (e.HandPointer.GetIsOver(btnLogout))
				{
					e.HandPointer.Capture(btnLogout);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
			}
		}

		//	Execute button functions
		private void HandPointerPressReleaseEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				if (e.HandPointer.GetIsOver(btnBack))
				{
					//	Close the window
					Close();

					VisualStateManager.GoToState(btnBack, "MouseOver", true);
				}
				else if (e.HandPointer.GetIsOver(btnLogout))
				{
					//	User decided to log out
					loggedOut = true;
					Close();

					VisualStateManager.GoToState(btnLogout, "MouseOver", true);
				}
				else
				{
					VisualStateManager.GoToState(btnBack, "Normal", true);
					VisualStateManager.GoToState(btnLogout, "Normal", true);
				}

				e.HandPointer.Capture(null);
				e.Handled = true;

				Close();
			}
		}

		private void HandPointerCaptureEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == null)
			{
				capturedHandPointer = e.HandPointer;
				e.Handled = true;
			}
		}

		private void HandPointerLostCaptureEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				capturedHandPointer = null;
				e.Handled = true;
			}
		}

		//	Function that show Window as dialog and return logged out status
		public bool CustomShowDialog()
		{
			ShowDialog();
			return loggedOut;
		}

		//	Temporary clicks function
		private void back(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void logout(object sender, RoutedEventArgs e)
		{
			loggedOut = true;
			Close();
		}
	}
}
