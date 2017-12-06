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
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for DragDropImages.xaml
	/// </summary>
	public partial class DragDropImages : Window
	{
		private KinectSensorChooser kinectSensorChooser;
		private HandPointer capturedHandPointer;
		private GripState lastGripState = GripState.Released;

		private enum GripState
		{
			Released,
			Gripped
		}

		public DragDropImages(Microsoft.Kinect.Toolkit.KinectSensorChooser kinectSensorChooser)
		{
			this.kinectSensorChooser = kinectSensorChooser;
			this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			InitializeComponent();

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			//Press
			KinectRegion.SetIsPressTarget(back, true);

			KinectRegion.AddHandPointerEnterHandler(back, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(back, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(back, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(back, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(back, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(back, HandPointerLostCaptureEvent);

			//Grip
			KinectRegion.SetIsGripTarget(Img, true);

			KinectRegion.AddHandPointerEnterHandler(Img, HandPointerGripEnterEvent);

			KinectRegion.AddHandPointerGripHandler(Img, HandPointerGripEvent);
			KinectRegion.AddHandPointerGripReleaseHandler(Img, HandPointerGripReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(Img, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(Img, HandPointerLostCaptureEvent);

			KinectRegion.AddQueryInteractionStatusHandler(Img, QueryInteractionStatusEvent);

			KinectRegion.AddHandPointerMoveHandler(Img, HandPointerMoveEvent);
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
			if (capturedHandPointer != null)
			{
				capturedHandPointer.Capture(null);
			}
			capturedHandPointer = e.HandPointer;
			e.Handled = true;
		}

		private void HandPointerLostCaptureEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				capturedHandPointer = null;
				lastGripState = GripState.Released;
				e.Handled = true;
			}
		}

		private void HandPointerGripEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(Img) && e.HandPointer.IsPrimaryHandOfUser)
				e.Handled = true;
		}

		private void HandPointerGripEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.IsPrimaryUser && e.HandPointer.IsPrimaryHandOfUser && e.HandPointer.IsInteractive)
			{
				if (e.HandPointer == null)
					return;

				if (capturedHandPointer != e.HandPointer)
				{
					if (e.HandPointer.Captured == null)
					{
						e.HandPointer.Capture(Img);
					}
					else
					{
						// Some other control has capture, ignore grip
						return;
					}
					capturedHandPointer = e.HandPointer;
				}

				lastGripState = GripState.Gripped;
				e.Handled = true;
			}
		}

		private void HandPointerGripReleaseEvent(object sender, HandPointerEventArgs e)
		{
			if (Img.Equals(e.HandPointer.Captured))
			{
				lastGripState = GripState.Released;
				e.HandPointer.Capture(null);
				e.Handled = true;
			}
		}

		private void QueryInteractionStatusEvent(object sender, QueryInteractionStatusEventArgs e)
		{
			if (Img.Equals(e.HandPointer.Captured))
			{ 
				e.IsInGripInteraction = lastGripState == GripState.Gripped;
				e.Handled = true;
			}
		}

		//When Hand Pointer Moves
		private void HandPointerMoveEvent(object sender, HandPointerEventArgs e)
		{
			if (this.Equals(e.HandPointer.Captured))
			{
				e.Handled = true;

				var currentPosition = e.HandPointer.GetPosition(Img);

				if (lastGripState == GripState.Released || !e.HandPointer.IsInteractive)
					return;
				
				Image image = e.Source as Image;
				DataObject data = new DataObject(typeof(ImageSource), image.Source);
				DragDrop.DoDragDrop(image, data, DragDropEffects.Copy);
			}
		}

		private void DropImage(object sender, DragEventArgs e)
		{


		}
	}
}
