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
	/// Interaction logic for CustomMessageBox.xaml
	/// </summary>
	public partial class CustomMessageBox : Window
	{
		private HandPointer capturedHandPointer;
		private KinectSensorChooser kinectSensorChooser;
		
		public CustomMessageBox(KinectSensorChooser kinectSensorChooser)
		{
			InitializeComponent();

			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			this.kinectSensorChooser = kinectSensorChooser;

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			//	Setup Kinect region press target and event handlers
			KinectRegion.SetIsPressTarget(btnOK, true);

			//	btnOK
			KinectRegion.AddHandPointerEnterHandler(btnOK, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnOK, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnOK, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnOK, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnOK, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnOK, HandPointerLostCaptureEvent);
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(btnOK) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(btnOK, "MouseOver", true);

			e.Handled = true;
		}

		private void HandPointerLeaveEvent(object sender, HandPointerEventArgs e)
		{
			if (!e.HandPointer.GetIsOver(btnOK) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(btnOK, "Normal", true);

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
				if (e.HandPointer.GetIsOver(btnOK))
				{
					e.HandPointer.Capture(btnOK);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
			}
		}

		private void HandPointerPressReleaseEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				if (e.HandPointer.GetIsOver(btnOK))
				{
					Close();
					VisualStateManager.GoToState(btnOK, "MouseOver", true);
				}
				else
				{
					VisualStateManager.GoToState(btnOK, "Normal", true);
				}
				e.HandPointer.Capture(null);
				e.Handled = true;
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

		// Show message
		public void ShowText(string text, string title = "Error")
		{
			textBlock.Text = text;
			Title = title;
			ShowDialog();
		}

		//	Temporary click function
		private void back(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
