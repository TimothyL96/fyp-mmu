﻿using System;
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
using System.Windows.Data;
using System.Diagnostics;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for DragDropImages.xaml
	/// </summary>
	public partial class DragDropImages : Window
	{
		KinectSensor sensor;
		KinectSensorChooser kinectSensorChooser;

		private const int horizontalLengthStart = 150;
		private int horizontalLength = 150;  //	X axis image drop starting point
		private int horizontalLengthChange = 150;
		private int verticalLength = 10;

		private int horizontalMaxLength;
		private int verticalMaxLength;

		private int imageWidth = 200;
		private int imageHeight = 200;

		private Skeleton[] allSkeletons = new Skeleton[6];
		private static int screenWidth;// (int)SystemParameters.PrimaryScreenWidth;
		private static int screenHeight;// (int)SystemParameters.PrimaryScreenHeight;
		private int screenFactorX;
		private int screenFactorY;

		private InteractionStream _interactionStream;
		private Skeleton[] _skeletons; //the skeletons 
		private UserInfo[] _userInfos; //the information about the interactive users

		private Dictionary<int, InteractionHandEventType> _lastLeftHandEvents = new Dictionary<int, InteractionHandEventType>();
		private Dictionary<int, InteractionHandEventType> _lastRightHandEvents = new Dictionary<int, InteractionHandEventType>();

		private int handCursorOn = -1;
		private double handCursorOnLeftDistant;
		private double handCursorOnTopDistant;
		private int itemWidth = 30;
		private int itemHeight = 30;
		private const int itemChildrenStart = 8;

		private int currentScore;
		private int currentLives;
		public string CurrentScoreText
		{
			get { return (string)GetValue(CurrentScoreTextProperty); }
			set { SetValue(CurrentScoreTextProperty, value); }
		}
		public string CurrentLivesText
		{
			get { return (string)GetValue(CurrentLivesTextProperty); }
			set { SetValue(CurrentLivesTextProperty, value); }
		}

		Stopwatch watch = Stopwatch.StartNew();
		private string currentTime;
		private int playerID = 1;
		private int itemGame = 1;

		//	Total item counts in database
		private int totalItemCount = 0;

		//	Flag if after 3 seconds will run the game
		private int countdownFlag = 3;

		//	DipatchTimer
		DispatcherTimer timerCountdown = new DispatcherTimer();
		DispatcherTimer timerCreateImage = new DispatcherTimer();
		DispatcherTimer timerPushImage = new DispatcherTimer();

		//	ItemsRepository
		ItemsRepository itemsRepository = new ItemsRepository();

		//	Dpendency Property:
		public static readonly DependencyProperty CurrentScoreTextProperty = DependencyProperty.Register("CurrentScoreText", typeof(string), typeof(DragDropImages), new PropertyMetadata("Score: 0"));
		public static readonly DependencyProperty CurrentLivesTextProperty = DependencyProperty.Register("CurrentLivesText", typeof(string), typeof(DragDropImages), new PropertyMetadata("Lives: 0"));

		//	Constructor
		public DragDropImages()
		{
			InitializeComponent();

			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			kinectSensorChooser = new KinectSensorChooser();
			kinectSensorChooser.KinectChanged += KinectSensorChooser_KinectChanged;

			currentScore = 0;
			currentLives = 3;
			UpdateScoreLives();

			DataContext = this;

			//	Set up the font size and weight of score and lives
			//	Fontsize
			score.FontSize = 22;
			lives.FontSize = 22;

			//	Fontweight
			score.FontWeight = FontWeights.Bold;
			lives.FontWeight = FontWeights.Bold;

			//	Adjust the item width
			itemWidth = imageWidth;
			itemHeight = imageHeight;

			//	Get total item counts
			totalItemCount = itemsRepository.GetCount();
		}

		private void UpdateScoreLives()
		{
			CurrentScoreText = "Score: " + currentScore;
			CurrentLivesText = "Lives: " + currentLives;
		}

		public void GetLoadGameData(int lives, string time, int score, int itemGame)
		{
			currentLives = lives;
			currentTime = time;
			currentScore = score;
			this.itemGame = itemGame;
			UpdateScoreLives();
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
					e.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
					e.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
					e.NewSensor.SkeletonStream.Enable();
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

				if (kinectSensorChooser.Status == ChooserStatus.SensorStarted)
					InitializeSkeleton();
			}
		}
		
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			kinectSensorChooser.Start();

			//	Fix the orange bin to the middle of blue bin and brown bin
			var brownBinPoint = brownBin.TranslatePoint(new Point(0, 0), canvas);
			var blueBinPoint = blueBin.TranslatePoint(new Point(0, 0), canvas);
			Canvas.SetLeft(orangeBin, ((brownBinPoint.X - blueBinPoint.X) / 2) + blueBinPoint.X);

			screenWidth = (int)SystemParameters.FullPrimaryScreenWidth;
			screenHeight = (int)SystemParameters.FullPrimaryScreenHeight;

			screenFactorX = screenWidth / 640;
			screenFactorY = screenHeight / 480;

			//	Setup countdown text position
			Canvas.SetLeft(countdown, ((screenWidth / 2) - (countdown.ActualWidth / 2)));
			Canvas.SetTop(countdown, ((screenHeight / 2) - (countdown.ActualHeight / 2)));

			//	Set vertical and horizontal max length
			verticalMaxLength = screenHeight - itemHeight;
			horizontalMaxLength = screenWidth - itemWidth;

			//	test
			//	Delay one second
			System.Threading.Thread.Sleep(1000);

			//	Start counting down
			timerCountdown.Tick += new EventHandler(Tick_Countdown);
			timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
			countdown.Visibility = Visibility.Visible;
			timerCountdown.Start();
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

			//sensor = KinectSensor.KinectSensors.FirstOrDefault();
			if (sensor == null)
			{
				CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
				customMessageBox.ShowText("No Kinect sensor detected!");
				Close();
				return;
			}

			_skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
			_userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];

			_interactionStream = new InteractionStream(sensor, new DummyInteractionClient());
			_interactionStream.InteractionFrameReady += InteractionStreamOnInteractionFrameReady;
			   
			sensor.DepthFrameReady += SensorOnDepthFrameReady;
			sensor.SkeletonFrameReady += SensorOnSkeletonFrameReady;
			sensor.AllFramesReady += SensorAllFramesReady;
			
			if (!sensor.IsRunning)
				sensor.Start();

			//	Delay one second
			System.Threading.Thread.Sleep(1000);

			//	Start counting down
			timerCountdown.Tick += new EventHandler(Tick_Countdown);
			timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
			countdown.Visibility = Visibility.Visible;
			timerCountdown.Start();
		}
		
		private void Tick_Countdown(object source, EventArgs e)
		{
			countdownFlag--;
			countdown.Text = countdownFlag.ToString();

			if (countdownFlag == 0)
			{
				//	Start counting down
				timerCreateImage.Tick += new EventHandler(Tick_CreateImage);
				timerCreateImage.Interval = TimeSpan.FromMilliseconds(1500);
				timerCreateImage.Start();
				timerCountdown.Stop();
				countdown.Visibility = Visibility.Hidden;

				//	Start pushing images down
				timerPushImage.Tick += new EventHandler(Tick_PushImage);
				timerPushImage.Interval = TimeSpan.FromMilliseconds(50);
				timerPushImage.Start();
			}
		}

		private void Tick_CreateImage(object source, EventArgs e)
		{
			//	Create new image
			Image image;

			//	Generate random number for the random recycleable item from the database
			Random random = new Random();
			itemGame = random.Next(1, 10);

			//	Get the specific item
			ItemsRepository.ItemsDto itemsDto = itemsRepository.GetItem(itemGame)[0];

			Application.Current.Dispatcher.Invoke(delegate
			{
				image = new Image
				{
					Width = imageWidth,
					Height = imageHeight,
					Source = new BitmapImage(new Uri(itemsDto.Item_Image_Link, UriKind.Relative)),
					Tag = itemsDto.Item_Type,
				};

				canvas.Children.Add(image);
				Canvas.SetLeft(canvas.Children[canvas.Children.Count - 1], horizontalLength);
				Point p = canvas.Children[canvas.Children.Count - 1].TranslatePoint(new Point(0, 0), canvas);

				//	Randomize
				horizontalLength += horizontalLengthChange;

				if (horizontalLength >= horizontalMaxLength)
					horizontalLength = horizontalLengthStart;  //Reset to the most left of screen
			});
		}

		private void Tick_PushImage(object source, EventArgs e)
		{
			for (int i = itemChildrenStart; i < canvas.Children.Count; i++)
			{
				if (i == handCursorOn)
					continue;
				
				Point p = canvas.Children[i].TranslatePoint(new Point(0, 0), canvas);
				Canvas.SetTop(canvas.Children[i], p.Y);
				//todo

				//	Define speed / difficulty
				//verticalLength = 150;

				//	If the image touched edge of window then stop it
				if (Canvas.GetTop(canvas.Children[i]) > verticalMaxLength)
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
    
			foreach (var userInfo in _userInfos)
			{
				var userID = userInfo.SkeletonTrackingId;
				if (userID == 0)
					continue;
     
				var hands = userInfo.HandPointers;
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
					//	TODO:
					//	Lives & Score
					//	Scale to screensize
					//	Randomized the item drops
					//	items drop horizontal & vertical

					if (lastHandEvent == InteractionHandEventType.Grip)
					{
						//	If hand gripped, show gripped cursor and move the item with the cursor
						handCursor.Source = new BitmapImage(new Uri("Resources/pointerWhite.png", UriKind.Relative));

						//	Find the item number of which the hand cursor is on
						if (handCursorOn == -1)
						{
							for (int i = itemChildrenStart; i < canvas.Children.Count; i++)
							{
								var childrenPoint = canvas.Children[i].TranslatePoint(new Point(0, 0), canvas);
								var handCursorPoint = handCursor.TranslatePoint(new Point(0, 0), canvas);
								if (handCursorPoint.X >= childrenPoint.X && handCursorPoint.X <= childrenPoint.X + itemWidth && handCursorPoint.Y >= childrenPoint.Y && handCursorPoint.Y <= childrenPoint.Y + itemHeight)
								{
									handCursorOn = i;
									handCursorOnLeftDistant = handCursorPoint.X - childrenPoint.X;
									handCursorOnTopDistant = handCursorPoint.Y - childrenPoint.Y;
									break;
								}
							}
						}

						//	Move the item with the hand cursor
						if (handCursorOn != -1)
						{
							var p = handCursor.TranslatePoint(new Point(0, 0), canvas);
							Canvas.SetLeft(canvas.Children[handCursorOn], p.X + handCursorOnLeftDistant);
							Canvas.SetTop(canvas.Children[handCursorOn], p.Y + handCursorOnTopDistant);
						}
					}
					else if (lastHandEvent == InteractionHandEventType.GripRelease)
					{
						//	If hand grip released, show normal cursor
						handCursor.Source = new BitmapImage(new Uri("Resources/handWhite.png", UriKind.Relative));
						handCursorOn = -1;
					}
					else if (hand.IsPressed)
					{
						//	If hand pressed the back, go back
						var handCursorPoint = handCursor.TranslatePoint(new Point(0, 0), canvas);
						var backPoint = handCursor.TranslatePoint(new Point(0, 0), canvas);

						//	Left and Right check:
						if (back.Width + backPoint.X >= handCursorPoint.X && handCursorPoint.X >= backPoint.X)
						{
							//	Top and Bottom check:
							if (back.Height + backPoint.Y >= handCursorPoint.Y && handCursorPoint.Y >= backPoint.Y)
							{
								//	Save Game
								watch.Stop();
								GameRepository gro = new GameRepository();
								if (gro.GetGame(playerID).Count == 0)
								{
									currentTime = Convert.ToString(watch.ElapsedMilliseconds / 1000);
									gro.AddGame(currentLives, playerID, currentTime, currentScore, itemGame);
								}
								else
								{
									currentTime = Convert.ToString(Convert.ToInt64(currentTime) + watch.ElapsedMilliseconds / 1000);
									gro.ModifyGame(currentLives, playerID, currentTime, currentScore, itemGame);
								}
								watch.Reset();
								Close();
							}
						}
					}
				}
			}
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

		//	Scale the X and Y to the screen size
		private float ScaleVector(int length, float position)
		{
			float value = (((((float)length) / 1f) / 2f) * position * 5) + (length / 2);
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
			Canvas.SetLeft(element, point.X - element.Width / 2.5);
			Canvas.SetTop(element, point.Y - element.Height / 2.5);
		}

		private void Window_Closing(object sender, EventArgs e)
		{
			timerCreateImage.Stop();
			timerPushImage.Stop();

			if (sensor != null)
				sensor.Stop();

			kinectSensorChooser.Stop();
		}

		//	Temporary click function
		private void back_Click(object sender, RoutedEventArgs e)
		{
			//	Save Game
			watch.Stop();
			GameRepository gro = new GameRepository();
			if (gro.GetGame(playerID).Count == 0)
			{
				currentTime = Convert.ToString(watch.ElapsedMilliseconds / 1000);
				gro.AddGame(currentLives, playerID, currentTime, currentScore, itemGame);
			}
			else
			{
				currentTime = Convert.ToString(Convert.ToInt64(currentTime) + watch.ElapsedMilliseconds / 1000);
				gro.ModifyGame(currentLives, playerID, currentTime, currentScore, itemGame);
			}
			watch.Reset();
			Close();
		}
	}
}
