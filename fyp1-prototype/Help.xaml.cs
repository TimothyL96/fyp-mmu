using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for Help.xaml
	/// </summary>
	public partial class Help : Window
	{
		private HandPointer capturedHandPointer;
		private KinectSensorChooser kinectSensorChooser;

		public Help(KinectSensorChooser kinectSensorChooser)
		{
			InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.kinectSensorChooser = kinectSensorChooser;

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			back.FontSize = 24;

			var textHeader = new Label
			{
				Content = "Help",
				FontWeight = FontWeights.Bold,
				FontSize = 68
			};
			scrollContent.Children.Add(textHeader);

			string text = "This section will give you a guide to this game.\nTo play this game, you need to have Kinect for Windows sensor.\nFind an area where the Kinect sensor can detect you well and clear.\n";
			text += "\nPress a button by performing a push and pull gesture with your hand.\n";
			text += "Drag and drop items by gripping your hand and releasing the grip.\n";
			text += "You need an account and be logged in to start playing the game.\n";
			text += "\nYour goal for the game would be about dragging the spawned item to the correct recycle bin.\n";
			text += "\nA correct drag drop would give you a score and a wrong drag drop would takes a live away from you.\n";
			text += "\nCurrently, a drop to the floor would not have your live deducted.\n";
			text += "\nSURVIVAL MODE:\nSurviving the game limited lives. Your game would not end if you still have lives left!.\n";
			text += "\nTIME ATTACK MODE:\nIn 60 seconds, get as much score as you can!.\n";
			var textContent = new Label
			{
				Content = text,
				FontSize = 22
			};
			scrollContent.Children.Add(textContent);

			var creditHeader = new Label
			{
				Content = "Credits:",
				FontWeight = FontWeights.Bold,
				FontSize = 68
			};
			scrollContent.Children.Add(creditHeader);
			var creditContent = new Label
			{
				Content = "flaticon.com\nSome images are provided free by this website.\nIcon made by Vaadin from www.flaticon.com ",
				FontSize = 22
			};
			scrollContent.Children.Add(creditContent);

			#region KinectRegion
			//	Setup Kinect region press target and event handlers
			KinectRegion.SetIsPressTarget(back, true);

			KinectRegion.AddHandPointerEnterHandler(back, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(back, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(back, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(back, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(back, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(back, HandPointerLostCaptureEvent);
			#endregion`
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(back) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(back, "MouseOver", true);
			}

			e.Handled = true;
		}

		private void HandPointerLeaveEvent(object sender, HandPointerEventArgs e)
		{
			if (!e.HandPointer.GetIsOver(back) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(back, "Normal", true);
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
				if (e.HandPointer.GetIsOver(back))
				{
					e.HandPointer.Capture(back);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
			}
		}

		//	Execute press functions
		private void HandPointerPressReleaseEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				if (e.HandPointer.GetIsOver(back))
				{
					Close();

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
	}
}
