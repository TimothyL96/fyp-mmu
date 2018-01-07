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
using System.Windows.Input;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

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
		private static int screenWidth = 2040;// (int)SystemParameters.PrimaryScreenWidth;
		private static int screenHeight = 1040;// (int)SystemParameters.PrimaryScreenHeight;
		KinectSensor sensor;

		private InteractionStream _interactionStream;
		private Skeleton[] _skeletons; //the skeletons 
		private UserInfo[] _userInfos; //the information about the interactive users

		public interface IInteractionClient
		{
			InteractionInfo GetInteractionInfoAtLocation(
				int skeletonTrackingId,
				InteractionHandType handType,
				double x,
				double y
				);
		}

		public class DummyInteractionClient : Microsoft.Kinect.Toolkit.Interaction.IInteractionClient
		{
			public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
			{
				var result = new InteractionInfo();
				result.IsGripTarget = true;
				result.IsPressTarget = true;
				result.PressAttractionPointX = 0.5;
				result.PressAttractionPointY = 0.5;
				result.PressTargetControlId = 1;

				return result;
			}
		}

		public DragDropImages(KinectSensorChooser kinectSensorChooser)
		{
			//Cursor cursor = new Cursor(Path.GetFullPath("Resource\\grab.cur"));
			
			if (KinectSensor.KinectSensors.Count > 0)
			{
				sensor = KinectSensor.KinectSensors[0];
				if (sensor.Status == KinectStatus.Connected)
				{
					sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
					sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensorAllFramesReady);

					sensor.SkeletonStream.Enable();

					//sensor.Start();
				}
			}		

			this.kinectSensorChooser = kinectSensorChooser;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			InitializeComponent();

			KinectRegion.AddHandPointerGripReleaseHandler(back, HandPointerPressReleaseEvent);

			//	Start creating images
			var dispatcherTimer = new System.Timers.Timer(1000);
			dispatcherTimer.Elapsed += dispatcherTimer_Tick;
			dispatcherTimer.Start();
			// new DroppingObjectManager().Start();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;

			sensor = KinectSensor.KinectSensors.FirstOrDefault();
			if (sensor == null)
			{
				Close();
				return;
			}

			_skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
			_userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];
			
	        sensor.DepthStream.Range = DepthRange.Near;
			sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
			   
			sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
			sensor.SkeletonStream.EnableTrackingInNearRange = true;
			sensor.SkeletonStream.Enable();

			_interactionStream = new InteractionStream(sensor, new DummyInteractionClient());
			_interactionStream.InteractionFrameReady += InteractionStreamOnInteractionFrameReady;
			   
			sensor.DepthFrameReady += SensorOnDepthFrameReady;
			sensor.SkeletonFrameReady += SensorOnSkeletonFrameReady;
			   
			sensor.Start();
		}

		private Dictionary<int, InteractionHandEventType> _lastLeftHandEvents = new Dictionary<int, InteractionHandEventType>();
		private Dictionary<int, InteractionHandEventType> _lastRightHandEvents = new Dictionary<int, InteractionHandEventType>();
      
		 private void InteractionStreamOnInteractionFrameReady(object sender, InteractionFrameReadyEventArgs args)
		 {
			 using (var iaf = args.OpenInteractionFrame()) //dispose as soon as possible
			 {
				 if (iaf == null)
					 return;
     
				iaf.CopyInteractionDataTo(_userInfos);
			}
     
			StringBuilder dump = new StringBuilder();
     
			var hasUser = false;
			foreach (var userInfo in _userInfos)
			{
				var userID = userInfo.SkeletonTrackingId;
				if (userID == 0)
					continue;
     
				hasUser = true;
				dump.AppendLine("User ID = " + userID);
				dump.AppendLine("  Hands: ");
				var hands = userInfo.HandPointers;
				if (hands.Count == 0)
					dump.AppendLine("    No hands");
				else
				{
					foreach (var hand in hands)
					{
						var lastHandEvents = hand.HandType == InteractionHandType.Left
													? _lastLeftHandEvents
													: _lastRightHandEvents;
						if (lastHandEvents == _lastLeftHandEvents)
							continue;

						if (hand.HandEventType != InteractionHandEventType.None)
							lastHandEvents[userID] = hand.HandEventType;
     
						var lastHandEvent = lastHandEvents.ContainsKey(userID)
												? lastHandEvents[userID]
												: InteractionHandEventType.None;
     
						dump.AppendLine();
						dump.AppendLine("    HandType: " + hand.HandType);
						dump.AppendLine("    HandEventType: " + hand.HandEventType);
						dump.AppendLine("    LastHandEventType: " + lastHandEvent);
						dump.AppendLine("    IsActive: " + hand.IsActive);
						dump.AppendLine("    IsPrimaryForUser: " + hand.IsPrimaryForUser);
						dump.AppendLine("    IsInteractive: " + hand.IsInteractive);
						dump.AppendLine("    PressExtent: " + hand.PressExtent.ToString("N3"));
						dump.AppendLine("    IsPressed: " + hand.IsPressed);
						dump.AppendLine("    IsTracked: " + hand.IsTracked);
						dump.AppendLine("    X: " + hand.X.ToString("N3"));
						dump.AppendLine("    Y: " + hand.Y.ToString("N3"));
						dump.AppendLine("    RawX: " + hand.RawX.ToString("N3"));
						dump.AppendLine("    RawY: " + hand.RawY.ToString("N3"));
						dump.AppendLine("    RawZ: " + hand.RawZ.ToString("N3"));

						if (lastHandEvent == InteractionHandEventType.Grip)
							handCursor.Source = new BitmapImage(new Uri("Resources/bluebin.png", UriKind.Relative));
						else if (lastHandEvent == InteractionHandEventType.GripRelease)
							handCursor.Source = new BitmapImage(new Uri("Resources/grab.png", UriKind.Relative));

						if (hand.IsPressed)
							handCursor.Source = new BitmapImage(new Uri("Resources/brownbin.png", UriKind.Relative));
					}
				}
     
				tb.Text = dump.ToString();
			}
     
			if (!hasUser)
				tb.Text = "No user detected.";
		}

		private void SensorOnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs skeletonFrameReadyEventArgs)
		{
			using (SkeletonFrame skeletonFrame = skeletonFrameReadyEventArgs.OpenSkeletonFrame())
			{
				if (skeletonFrame == null)
					return;
      
				try
				{
					skeletonFrame.CopySkeletonDataTo(_skeletons);
					var accelerometerReading = sensor.AccelerometerGetCurrentReading();
					_interactionStream.ProcessSkeleton(_skeletons, accelerometerReading, skeletonFrame.Timestamp);
				}
				catch (InvalidOperationException)
				{
					// SkeletonFrame functions may throw when the sensor gets
					// into a bad state.  Ignore the frame in that case.
				}
			}
		}

		private void SensorOnDepthFrameReady(object sender, DepthImageFrameReadyEventArgs depthImageFrameReadyEventArgs)
		{
			using (DepthImageFrame depthFrame = depthImageFrameReadyEventArgs.OpenDepthImageFrame())
			{
				 if (depthFrame == null)
					 return;
      
				try
				{
					_interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
				}
				catch (InvalidOperationException)
				{
					// DepthFrame functions may throw when the sensor gets
					// into a bad state.  Ignore the frame in that case.
				}
			}
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

		DroppingObjectManager _dom = new DroppingObjectManager();

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

				if (test > 1750) //1540 > width size is 200
					test = 50;

				//	Start pushing images down
				dispatcherTimer1.Tick += new EventHandler(dispatcherTimer_Tick1);
				dispatcherTimer1.Interval = TimeSpan.FromMilliseconds(50);
				dispatcherTimer1.Start();
			});

		}

		public class DroppingObjectManager
		{
			DateTime _lastCreation;

			public void Start()
			{
				Task.Run(() =>
				{
					while (true)
					{
						Tick();
						Task.Delay(10);
					}
				});
			}

			public void Tick()
			{
				DateTime now = DateTime.Now;
				if (now - _lastCreation > TimeSpan.FromSeconds(1))
				{
					Create();
					Console.WriteLine(now);
					_lastCreation = now;
				}
			}

			public void Create()
			{
			}
		}

		public class DroppingObject : Image
		{

		}

		private void dispatcherTimer_Tick1(object source, EventArgs ee)
		{
			for(int i = 6; i < canvas.Children.Count; i++)
			{
				var p = canvas.Children[i].TranslatePoint(new Point(0, 0), canvas);
				//var p = Canvas.GetTop(canvas.Children[i]);
				Canvas.SetTop(canvas.Children[i], p.Y + speed);

				//	Define speed / difficulty
				if (canvas.Children.Count == 8)
					speed = 5;

				//	If the image touched edge of window then stop it
				if (Canvas.GetTop(canvas.Children[i]) > 750) //840 > height size is 200
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
			Canvas.SetTop(element, scaledJoint.Position.Y);

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
					ColorImageFormat.RgbResolution1280x960Fps12);

				CameraPosition(handCursor, rightColorPoint);
			}
		}

		private void CameraPosition(FrameworkElement element, ColorImagePoint point)
		{
			//Canvas.SetLeft(element, point.X * 1.5 - element.Width / 2);
			//Canvas.SetTop(element, point.Y * 1.125 - element.Height / 2);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (sensor != null)
				sensor.Stop();
		}
	}
}
