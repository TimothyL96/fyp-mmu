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
	/// Interaction logic for LoadGame.xaml
	/// </summary>
	public partial class LoadGame : Window
	{
		private HandPointer capturedHandPointer;
		private KinectSensorChooser kinectSensorChooser;

		public LoadGame(KinectSensorChooser kinectSensorChooser)
		{
			InitializeComponent();

			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			this.kinectSensorChooser = kinectSensorChooser;

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			//	Setup Kinect region press target and event handlers
			KinectRegion.SetIsPressTarget(buttonLoad, true);

			KinectRegion.AddHandPointerEnterHandler(buttonLoad, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(buttonLoad, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(buttonLoad, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(buttonLoad, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(buttonLoad, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(buttonLoad, HandPointerLostCaptureEvent);
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(buttonLoad) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(buttonLoad, "MouseOver", true);

			e.Handled = true;
		}

		private void HandPointerLeaveEvent(object sender, HandPointerEventArgs e)
		{
			if (!e.HandPointer.GetIsOver(buttonLoad) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(buttonLoad, "Normal", true);

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
				if (e.HandPointer.GetIsOver(buttonLoad))
				{
					e.HandPointer.Capture(buttonLoad);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
			}
		}

		private void HandPointerPressReleaseEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				if (e.HandPointer.GetIsOver(buttonLoad))
				{
					Close();
					VisualStateManager.GoToState(buttonLoad, "MouseOver", true);
				}
				else
				{
					VisualStateManager.GoToState(buttonLoad, "Normal", true);
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

		//	Disable maximizing window
		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Maximized)
			{
				WindowState = WindowState.Normal;
			}
		}

		//	Temporary click function
		private void back(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
