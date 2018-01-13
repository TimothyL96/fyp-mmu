using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for HighScore.xaml
	/// </summary>
	public partial class HighScore : Window
	{
		private HandPointer capturedHandPointer;
		private KinectSensorChooser kinectSensorChooser;

		public HighScore(KinectSensorChooser kinectSensorChooser)
		{
			this.kinectSensorChooser = kinectSensorChooser;
			this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			InitializeComponent();

			back.FontSize = 24;

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			var textHeader = new Label
			{
				Content = "Name\t\t\t\t\t\t\t\t\t" + "Score",
				FontWeight = FontWeights.Bold,
				FontSize = 26
			};
			scrollContent.Children.Add(textHeader);

			for (int i = 35; i != 0; i--)
			{
				var textBody = new Label
				{
					FontSize = 26,
					Content = "AlliAlli\t\t\t\t\t\t\t\t\t" + i * i
				};
				scrollContent.Children.Add(textBody);
			}

			KinectRegion.SetIsPressTarget(back, true);

			KinectRegion.AddHandPointerEnterHandler(back, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(back, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(back, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(back, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(back, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(back, HandPointerLostCaptureEvent);
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(back) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(back, "MouseOver", true);

			e.Handled = true;
		}

		private void HandPointerLeaveEvent(object sender, HandPointerEventArgs e)
		{
			if (!e.HandPointer.GetIsOver(back) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(back, "Normal", true);

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
				if (e.HandPointer.GetIsOver(back))
				{
					e.HandPointer.Capture(back);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
			}
		}

		private void HandPointerPressReleaseEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				if (e.HandPointer.GetIsOver(back))
				{

					this.Close();
					VisualStateManager.GoToState(back, "MouseOver", true);
				}
				else
				{
					VisualStateManager.GoToState(back, "Normal", true);
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
			if(capturedHandPointer == e.HandPointer)
			{
				capturedHandPointer = null;
				e.Handled = true;
			}
		}

		//	Disable maximizing window
		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (this.WindowState == WindowState.Maximized)
			{
				this.WindowState = WindowState.Normal;
			}
		}
	}
}
