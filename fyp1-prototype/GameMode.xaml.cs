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
	/// Interaction logic for GameMode.xaml
	/// </summary>
	public partial class GameMode : Window
	{
		private HandPointer capturedHandPointer;
		KinectSensorChooser kinectSensorChooser;
		private int playerID;

		public GameMode(KinectSensorChooser kinectSensorChooser, int playerID)
		{
			InitializeComponent();

			//	Set window startup location to center
			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			//	Set the class members
			this.playerID = playerID;
			this.kinectSensorChooser = kinectSensorChooser;

			// Bind Kinect Sensor to Kinect Region
			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			#region KinectRegion
			//	Setup Kinect region press target and event handlers
			KinectRegion.SetIsPressTarget(btnTimelessClassic, true);
			KinectRegion.SetIsPressTarget(btnTimeAttack, true);

			//	btnTimelessClassic
			KinectRegion.AddHandPointerEnterHandler(btnTimelessClassic, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnTimelessClassic, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnTimelessClassic, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnTimelessClassic, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnTimelessClassic, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnTimelessClassic, HandPointerLostCaptureEvent);

			//	btnTimeAttack
			KinectRegion.AddHandPointerEnterHandler(btnTimeAttack, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnTimeAttack, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnTimeAttack, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnTimeAttack, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnTimeAttack, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnTimeAttack, HandPointerLostCaptureEvent);
			#endregion
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(btnTimelessClassic) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnTimelessClassic, "MouseOver", true);
			}
			else if (e.HandPointer.GetIsOver(btnTimeAttack) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnTimeAttack, "MouseOver", true);
			}

			e.Handled = true;
		}

		private void HandPointerLeaveEvent(object sender, HandPointerEventArgs e)
		{
			if (!e.HandPointer.GetIsOver(btnTimelessClassic) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnTimelessClassic, "Normal", true);
			}
			if (!e.HandPointer.GetIsOver(btnTimeAttack) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnTimeAttack, "Normal", true);
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
				if (e.HandPointer.GetIsOver(btnTimelessClassic))
				{
					e.HandPointer.Capture(btnTimelessClassic);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
				else if (e.HandPointer.GetIsOver(btnTimeAttack))
				{
					e.HandPointer.Capture(btnTimeAttack);
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
				kinectSensorChooser.Stop();
				DragDropImages dragDropImages = null;

				if (e.HandPointer.GetIsOver(btnTimelessClassic))
				{
					//	Timeless Classic
					dragDropImages  = new DragDropImages(playerID, 0);

					VisualStateManager.GoToState(btnTimelessClassic, "MouseOver", true);
				}
				else if (e.HandPointer.GetIsOver(btnTimeAttack))
				{
					//	Time Attack
					dragDropImages = new DragDropImages(playerID, 1);

					VisualStateManager.GoToState(btnTimeAttack, "MouseOver", true);
				}
				else
				{
					VisualStateManager.GoToState(btnTimelessClassic, "Normal", true);
					VisualStateManager.GoToState(btnTimeAttack, "Normal", true);
				}

				if (dragDropImages != null)
					dragDropImages.Show();

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

		private void timelessClassic(object sender, RoutedEventArgs e)
		{
			//	Timeless Classic
			kinectSensorChooser.Stop();
			DragDropImages dragDropImages = new DragDropImages(playerID, 0);
		}

		private void timeAttack(object sender, RoutedEventArgs e)
		{
			//	Time Attack
			kinectSensorChooser.Stop();
			DragDropImages dragDropImages = new DragDropImages(playerID, 1);
		}

		//	Disable maximizing window
		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Normal)
			{
				WindowState = WindowState.Maximized;
			}
		}
	}
}
