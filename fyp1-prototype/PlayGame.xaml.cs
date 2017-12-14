using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Windows.Threading;
using System.Linq;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for DragDropImages.xaml
	/// </summary>
	public partial class DragDropImages : Window
	{
		private KinectSensorChooser kinectSensorChooser;
		private HandPointer capturedHandPointer;
		private bool isGripinInteraction = false;
		private int test = 50;	//	X axis image drop starting point
		private DispatcherTimer dispatcherTimer1 = new DispatcherTimer();
		private int speed = 10;

		private Skeleton[] allSkeletons = new Skeleton[6];
		private static int screenWidth = 1900;
		private static int screenHeight = 1000;
		KinectSensor sensor;

		public DragDropImages(KinectSensorChooser kinectSensorChooser)
		{
			if (KinectSensor.KinectSensors.Count > 0)
			{
				sensor = KinectSensor.KinectSensors[0];
				if (sensor.Status == KinectStatus.Connected)
				{
					sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
					sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensorAllFramesReady);

					sensor.SkeletonStream.Enable();

					sensor.Start();
				}
			}		

			this.kinectSensorChooser = kinectSensorChooser;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			InitializeComponent();

			//var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			//BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			//Press
			KinectRegion.SetIsPressTarget(back, true);

			KinectRegion.AddHandPointerEnterHandler(back, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(back, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(back, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(back, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(back, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(back, HandPointerLostCaptureEvent);

			//Grip
			KinectRegion.SetIsGripTarget(back, true);

			KinectRegion.AddHandPointerGripHandler(back, HandPointerPressEvent);
			KinectRegion.AddHandPointerGripReleaseHandler(back, HandPointerPressReleaseEvent);

			/*KinectRegion.AddHandPointerEnterHandler(Img, HandPointerGripEnterEvent);

			KinectRegion.AddHandPointerGripHandler(back, HandPointerGripEvent);
			KinectRegion.AddHandPointerGripReleaseHandler(back, HandPointerGripReleaseEvent);
			
			KinectRegion.AddHandPointerGotCaptureHandler(Img, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(Img, HandPointerLostCaptureEvent);

			KinectRegion.AddQueryInteractionStatusHandler(Img, QueryInteractionStatusEvent);

			KinectRegion.AddHandPointerMoveHandler(Img, HandPointerMoveEvent);*/

			//	Start creating images
			var dispatcherTimer = new System.Timers.Timer(1000);
			dispatcherTimer.Elapsed += dispatcherTimer_Tick;
			dispatcherTimer.Start();
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
				isGripinInteraction = false;
				e.Handled = true;
			}
		}

		private void HandPointerGripEnterEvent(object sender, HandPointerEventArgs e)
		{
			//if (e.HandPointer.GetIsOver(Img) && e.HandPointer.IsPrimaryHandOfUser)
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
						//e.HandPointer.Capture(Img);
					}
					else
					{
						// Some other control has capture, ignore grip
						return;
					}
					capturedHandPointer = e.HandPointer;
				}

				isGripinInteraction = true;
				e.Handled = true;
			}
		}

		private void HandPointerGripReleaseEvent(object sender, HandPointerEventArgs e)
		{
			//if (Img.Equals(e.HandPointer.Captured))
			{
				isGripinInteraction = false;
				e.HandPointer.Capture(null);
				e.Handled = true;
			}
		}

		private void QueryInteractionStatusEvent(object sender, QueryInteractionStatusEventArgs e)
		{
			/*if (Img.Equals(e.HandPointer.Captured))
			{ 
				e.IsInGripInteraction = lastGripState == GripState.Gripped;
				e.Handled = true;
			}*/

			//	If a grip is detected then change the cursor image to grip
			if (e.HandPointer.HandEventType == HandEventType.Grip)
			{
				isGripinInteraction = true;
			}

			//	If grip release is detected then change the cursor image to open
			else if (e.HandPointer.HandEventType == HandEventType.GripRelease)
			{
				isGripinInteraction = false;
			}

			// If no change in state then do not change the cursor
			else if (e.HandPointer.HandEventType == HandEventType.None)
			{
				e.IsInGripInteraction = isGripinInteraction;
			}
		}

		//When Hand Pointer Moves
		private void HandPointerMoveEvent(object sender, HandPointerEventArgs e)
		{
			if (this.Equals(e.HandPointer.Captured))
			{
				e.Handled = true;

				//var currentPosition = e.HandPointer.GetPosition(Img);

				if (isGripinInteraction == false || !e.HandPointer.IsInteractive)
					return;
				
				Image image = e.Source as Image;
				DataObject data = new DataObject(typeof(ImageSource), image.Source);
				DragDrop.DoDragDrop(image, data, DragDropEffects.Copy);
			}
		}

		private void DropImage(object sender, DragEventArgs e)
		{

		}

		private void dispatcherTimer_Tick(object source, EventArgs e)
		{
			Image image;
			Application.Current.Dispatcher.Invoke((Action)delegate {
				image = new Image();
				image.Width = 200;
				image.Height = 200;
				image.Source = BitmapToImageSource(Properties.Resources.test);

				
				
				canvas.Children.Add(image);
				Canvas.SetLeft(canvas.Children[canvas.Children.Count - 1], test);
				var p = canvas.Children[canvas.Children.Count - 1].TranslatePoint(new Point(0, 0), canvas);

				test += 150;

				if (test > 1300) //1540 > width size is 200
					test = 50;

				//	Start pushing images down
				dispatcherTimer1.Tick += new EventHandler(dispatcherTimer_Tick1);
				dispatcherTimer1.Interval = TimeSpan.FromMilliseconds(300);
				dispatcherTimer1.Start();
			});
			

		}

		private void dispatcherTimer_Tick1(object source, EventArgs ee)
		{
			for(int i = 5; i < canvas.Children.Count; i++)
			{
				var p = canvas.Children[i].TranslatePoint(new Point(0, 0), canvas);
				//var p = Canvas.GetTop(canvas.Children[i]);
				Canvas.SetTop(canvas.Children[i], p.Y + speed);

				//	Define speed / difficulty
				if (canvas.Children.Count == 8)
					speed = 70;

				//	If the image touched edge of window then stop it
				if (Canvas.GetTop(canvas.Children[i]) > 700) //840 > height size is 200
				{
					canvas.Children.Remove(canvas.Children[i]);
				}
			}
		}

		//	Disable maximizing window
		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (this.WindowState == WindowState.Normal)
			{
				this.WindowState = WindowState.Maximized;
			}
		}

		//	Convert bitmap image to image source
		BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
		{
			using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
			{
				bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
				memory.Position = 0;
				BitmapImage bitmapimage = new BitmapImage();
				bitmapimage.BeginInit();
				bitmapimage.StreamSource = memory;
				bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapimage.EndInit();

				return bitmapimage;
			}
		}



		private void sensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
		{
			Skeleton first = GetFirstSkeleton(e);

			if (first == null)
			{
				return;
			}

			ScalePosition(handCursor, first.Joints[JointType.HandRight]);

			ProcessGesture(first.Joints[JointType.HandRight]);

			GetCameraPoint(first, e);

			throw new NotImplementedException();
		}

		Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
		{
			using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
			{
				if (skeletonFrameData == null)
				{
					return null;
				}

				skeletonFrameData.CopySkeletonDataTo(allSkeletons);

				Skeleton first = (from s in allSkeletons
								  where s.TrackingState == SkeletonTrackingState.Tracked
								  select s).FirstOrDefault();

				return first;
			}
		}

		private void ScalePosition(FrameworkElement element, Joint joint)
		{
			//convert the value to X/Y
			//Joint scaledJoint = joint.ScaleTo(1024, 768); 
			SkeletonPoint point = new SkeletonPoint();
			point.X = ScaleVector(screenWidth, joint.Position.X);
			point.Y = ScaleVector(screenHeight, -joint.Position.Y);
			point.Z = joint.Position.Z;

			Joint scaledJoint = joint;
			//Joint scaledJoint = joint.ScaleTo(1920, 1080);

			scaledJoint.TrackingState = JointTrackingState.Tracked;
			scaledJoint.Position = point;

			Canvas.SetLeft(element, scaledJoint.Position.X);
			//Canvas.SetTop(element, scaledJoint.Position.Y);

		}

		private float ScaleVector(int length, float position)
		{
			float value = (((((float)length) / 1f) / 2f) * position) + (length / 2);
			if (value > length)
			{
				return (float)length;
			}
			if (value < 0f)
			{
				return 0f;
			}
			return value;
		}

		private void ProcessGesture(Joint rightHand)
		{
			SkeletonPoint point = new SkeletonPoint();

			point.X = ScaleVector(screenWidth, rightHand.Position.X);
			point.Y = ScaleVector(screenHeight, -rightHand.Position.Y);
			point.Z = rightHand.Position.Z;

			rightHand.Position = point;
		}

		void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
		{
			using (DepthImageFrame depth = e.OpenDepthImageFrame())
			{
				if (depth == null)
				{
					return;
				}

				DepthImagePoint rightDepthPoint =
					depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);

				ColorImagePoint rightColorPoint =
					depth.MapToColorImagePoint(rightDepthPoint.X, rightDepthPoint.Y,
					ColorImageFormat.RgbResolution640x480Fps30);

				CameraPosition(handCursor, rightColorPoint);
			}
		}

		private void CameraPosition(FrameworkElement element, ColorImagePoint point)
		{
			Canvas.SetLeft(element, point.X - element.Width / 2);
			Canvas.SetTop(element, point.Y - element.Height / 2);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (sensor != null)
				sensor.Stop();
		}
	}
}
