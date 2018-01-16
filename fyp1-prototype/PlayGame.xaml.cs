using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Windows.Threading;
using System.Linq;
using System.Threading.Tasks;
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
		KinectSensor sensor;
		KinectSensorChooser kinectSensorChooser;

		private int horizontalLength = 50;	//	X axis image drop starting point
		private int verticalLength = 10;

		private int horizontalMaxLength = 750;
		private int verticalMaxLength = 1750;

		private int imageWidth = 200;
		private int imageHeight = 200;

		private DispatcherTimer timerCreateImage = new DispatcherTimer();
		private DispatcherTimer timerPushImage = new DispatcherTimer();

		private Skeleton[] allSkeletons = new Skeleton[6];
		private static int screenWidth = 2040;// (int)SystemParameters.PrimaryScreenWidth;
		private static int screenHeight = 1040;// (int)SystemParameters.PrimaryScreenHeight;

		private InteractionStream _interactionStream;
		private Skeleton[] _skeletons; //the skeletons 
		private UserInfo[] _userInfos; //the information about the interactive users

		private Dictionary<int, InteractionHandEventType> _lastLeftHandEvents = new Dictionary<int, InteractionHandEventType>();
		private Dictionary<int, InteractionHandEventType> _lastRightHandEvents = new Dictionary<int, InteractionHandEventType>();

		DroppingObjectManager _dom = new DroppingObjectManager();

		//	Constructor
		public DragDropImages()
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			InitializeComponent();

			kinectSensorChooser = new KinectSensorChooser();
			kinectSensorChooser.KinectChanged += KinectSensorChooser_KinectChanged;
			kinectSensorChooser.Start();
		}

		private void KinectSensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
		{
			bool error = false;

			if (e.OldSensor != null)
			{
				try
				{
					e.OldSensor.DepthStream.Range = DepthRange.Default;
					e.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
					e.OldSensor.DepthStream.Disable();
					e.OldSensor.SkeletonStream.Disable();
				}
				catch (InvalidOperationException)
				{
					error = true;
				}
			}

			if (e.NewSensor != null)
			{
				try
				{
					e.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
					e.NewSensor.DepthStream.Range = DepthRange.Near;
					e.NewSensor.SkeletonStream.Enable();
					e.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
					e.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
				}
				catch (InvalidOperationException)
				{
					error = true;
					throw;
				}
			}

			if (!error)
			{
				sensor = e.NewSensor;
				InitializeSkeleton();
			}
		}

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
				var result = new InteractionInfo
				{
					IsGripTarget = true,
					IsPressTarget = true,
					PressAttractionPointX = 0.5,
					PressAttractionPointY = 0.5,
					PressTargetControlId = 1
				};

				return result;
			}
		}

		private void InitializeSkeleton()
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;

			if (sensor == null)
			{
				Close();
				return;
			}

			_skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
			_userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];

			_interactionStream = new InteractionStream(sensor, new DummyInteractionClient());
			_interactionStream.InteractionFrameReady += InteractionStreamOnInteractionFrameReady;
			   
			sensor.DepthFrameReady += SensorOnDepthFrameReady;
			sensor.SkeletonFrameReady += SensorOnSkeletonFrameReady;
			
			if (!sensor.IsRunning)
				sensor.Start();

			//	Start creating images
			timerCreateImage.Tick += new EventHandler(Tick_CreateImage);
			timerCreateImage.Interval = TimeSpan.FromMilliseconds(1000);
			timerCreateImage.Start();
			// new DroppingObjectManager().Start();
		}

		private void Tick_CreateImage(object source, EventArgs e)
		{
			Image image;
			Application.Current.Dispatcher.Invoke(delegate
			{
				image = new Image
				{
					Width = imageWidth,
					Height = imageHeight,
					Source = BitmapToImageSource(Properties.Resources.pepsi330ml)
				};

				canvas.Children.Add(image);
				Canvas.SetLeft(canvas.Children[canvas.Children.Count - 1], horizontalLength);
				var p = canvas.Children[canvas.Children.Count - 1].TranslatePoint(new Point(0, 0), canvas);

				horizontalLength += 150;

				if (horizontalLength > verticalMaxLength) //1540 > width size is 200
					horizontalLength = 50;	//Reset to the most left of screen

				//	Start pushing images down
				timerPushImage.Tick += new EventHandler(Tick_PushImage);
				timerPushImage.Interval = TimeSpan.FromMilliseconds(50);
				timerPushImage.Start();
			});
		}

		private void Tick_PushImage(object source, EventArgs e)
		{
			for (int i = 6; i < canvas.Children.Count; i++)
			{
				var p = canvas.Children[i].TranslatePoint(new Point(0, 0), canvas);
				Canvas.SetTop(canvas.Children[i], p.Y + verticalLength);

				//	Define speed / difficulty
				if (canvas.Children.Count == 8)
					verticalLength = 5;

				//	If the image touched edge of window then stop it
				if (Canvas.GetTop(canvas.Children[i]) > horizontalMaxLength) //840 > height size is 200
				{
					canvas.Children.Remove(canvas.Children[i]);
				}
			}
		}

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
						{

						}
						else if (lastHandEvent == InteractionHandEventType.GripRelease)
						{

						}

						if (hand.IsPressed)
						{

						}
						//Reference: handCursor.Source = new BitmapImage(new Uri("Resources/brownbin.png", UriKind.Relative));
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

		private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
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

		private void Window_Closing(object sender, EventArgs e)
		{
			//if (sensor != null)
				//sensor.Stop();

			kinectSensorChooser.Stop();
		}
	}
}
