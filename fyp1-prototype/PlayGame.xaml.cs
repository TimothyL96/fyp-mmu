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
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for DragDropImages.xaml
	/// </summary>
	public partial class DragDropImages : Window
	{
		#region Class members
		//	Sensor storage
		KinectSensor sensor;
		KinectSensorChooser kinectSensorChooser;

		//	Set items spawning and speed
		private const int horizontalLengthStart = 150;
		private int horizontalLength = 150;  //	X axis image drop starting point
		private int horizontalLengthChange = 150;
		private int verticalLength = 10;

		//	Items max spawing and pushing area
		private int horizontalMaxLength;
		private int verticalMaxLength;

		//	Default item width and height
		private int imageWidth = 200;
		private int imageHeight = 200;

		private int itemWidth = 30;
		private int itemHeight = 30;

		//	ALl skeleton and Interaction Streams storage
		private Skeleton[] allSkeletons = new Skeleton[6];
		private InteractionStream _interactionStream;

		//	The skeletons
		private Skeleton[] _skeletons;

		//	The information about the interactive users
		private UserInfo[] _userInfos;

		//	Full primary screen width and height
		private static int screenWidth;
		private static int screenHeight;

		//	Screen factor of horizontal and vertical by dividing screenWidth and screenHeight by Kinect's 640x480 for scaling
		private float screenFactorX;
		private float screenFactorY;

		//	Dictionary for left and right hand
		private Dictionary<int, InteractionHandEventType> _lastLeftHandEvents = new Dictionary<int, InteractionHandEventType>();
		private Dictionary<int, InteractionHandEventType> _lastRightHandEvents = new Dictionary<int, InteractionHandEventType>();

		//	This indicates the start index of canvas that is an item
		private const int itemChildrenStart = 9;

		//	Record the current score, lives and time
		private int currentScore;
		private int currentLives;
		private long currentTime;

		//	Timer to record the time
		Stopwatch watch = Stopwatch.StartNew();

		//	Previously elapsed time in saved game
		private long previousTime = 0;

		//	String to display current score, lives and time on the screen
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
		public string CurrentTimeText
		{
			get { return (string)GetValue(CurrentTimeTextProperty); }
			set { SetValue(CurrentTimeTextProperty, value); }
		}

		//	Get the logged in player ID
		public int playerID;

		//	Current Item index from the item table
		private int itemGame = 1;

		//	Total item counts in database
		private int totalItemCount = 0;

		//	Flag for counting down. After 3 seconds will run the game
		private double countdownFlag = 4;

		//	Has game ended flag
		private bool hasGameEnded = false;

		//	Item Gripped object
		ItemGripped itemGripped;

		//	Random variables
		Random random;
		Random randomHorizontal;
		Random randomVertical;

		//	DipatchTimer
		DispatcherTimer timerCountdown = new DispatcherTimer();
		DispatcherTimer timerCreateImage = new DispatcherTimer();
		DispatcherTimer timerUpdateScore = new DispatcherTimer();
		DispatcherTimer timerGameEnd = new DispatcherTimer();
		DispatcherTimer timerGameBack = new DispatcherTimer();

		//	ItemsRepository
		ItemsRepository itemsRepository = new ItemsRepository();

		/// <summary>
		/// Game mode
		///	0 - Survival
		///	1 - Time Attack
		/// </summary>
		public int gameMode;

		//	Dpendency Property:
		public static readonly DependencyProperty CurrentScoreTextProperty =
			DependencyProperty.Register("CurrentScoreText", typeof(string), typeof(DragDropImages), new PropertyMetadata("Score: 0"));
		public static readonly DependencyProperty CurrentLivesTextProperty =
			DependencyProperty.Register("CurrentLivesText", typeof(string), typeof(DragDropImages), new PropertyMetadata("Lives: 0"));
		public static readonly DependencyProperty CurrentTimeTextProperty =
			DependencyProperty.Register("CurrentTimeText", typeof(string), typeof(DragDropImages), new PropertyMetadata("Time: 0"));
		#endregion

		//	TODO:
		//	Scale to screensize - next to do
		//	fix kinect start and stop app crash
		//	multiplayer
		//	add more items
		//	personal best at homescreen
		//	login & register - backspace
		//	login before proceeding playing game n loading game
		//	logout button

		//	Constructor
		public DragDropImages(int playerID, int gameMode)
		{
			InitializeComponent();

			//	Set screen to the center
			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			//	Kinect changed event listener
			kinectSensorChooser = new KinectSensorChooser();
			kinectSensorChooser.KinectChanged += KinectSensorChooser_KinectChanged;

			//	Set the player ID
			this.playerID = playerID;

			//	Set the game mode
			this.gameMode = gameMode;

			//	Reset the stopwatch
			watch.Reset();

			//	Default value for the score, lives and time
			currentScore = 0;
			currentLives = 3;
			currentTime = 0;

			//	Update the score, lives and time on screen
			UpdateScoreLivesTime();

			//	Data binding
			DataContext = this;

			//	Set up the font size and weight of score and lives
			//	Fontsize
			score.FontSize = 22;
			lives.FontSize = 22;
			time.FontSize = 22;

			//	Fontweight
			score.FontWeight = FontWeights.Bold;
			lives.FontWeight = FontWeights.Bold;
			time.FontWeight = FontWeights.Bold;

			//	Adjust the item width
			itemWidth = imageWidth;
			itemHeight = imageHeight;

			//	Get total item counts
			totalItemCount = itemsRepository.GetCount();

			itemGripped = new ItemGripped
			{
				GameMode = gameMode,
				CurrentScore = currentScore,
				CurrentLives = currentLives,
				HandCursorOn = -1,
				HandCursorOnSet = false,
			};
		}

		//	When window is loaded
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//	Start the Kinect sensor
			kinectSensorChooser.Start();

			//	Fix the orange bin to the middle of blue bin and brown bin by dividing the distance betweenthe blue bin and brown bin
			var brownBinPoint = brownBin.TranslatePoint(new Point(0, 0), canvas);
			var blueBinPoint = blueBin.TranslatePoint(new Point(0, 0), canvas);
			Canvas.SetLeft(orangeBin, ((brownBinPoint.X - blueBinPoint.X) / 2) + blueBinPoint.X);

			//	Set the screenWidth and screenHeight to the width and height of the full primary screen
			screenWidth = (int)SystemParameters.FullPrimaryScreenWidth;
			screenHeight = (int)SystemParameters.FullPrimaryScreenHeight;

			//	Calculate the screen factor
			screenFactorX = screenWidth / 640;
			screenFactorY = screenHeight / 480;

			//	Setup countdown text position
			Canvas.SetLeft(countdown, ((screenWidth / 2) - (countdown.ActualWidth / 2)));
			Canvas.SetTop(countdown, ((screenHeight / 2) - (countdown.ActualHeight / 2)));

			//	Set vertical and horizontal max length
			//	Vertical max length is screen height minus recycle bin height minus 25% of a item height
			verticalMaxLength = screenHeight - (int)blueBin.ActualHeight - (int)(itemHeight * 0.75);
			horizontalMaxLength = screenWidth - itemWidth;
		}

		//	When Kinect is started or stopped
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

		//	Update score, lives and time on the screen
		private void UpdateScoreLivesTime()
		{
			CurrentScoreText = "Score: " + currentScore;

			//	Update lives text according to game mode
			if (gameMode == 0)
			{
				CurrentLivesText = "Lives: " + currentLives;
			}
			else if (gameMode == 1)
			{
				CurrentLivesText = "Lives: -";
			}

			//	Get the playing time
			if (watch.IsRunning)
			{
				currentTime = previousTime + watch.ElapsedMilliseconds / 1000;
			}
			else
			{
				currentTime = previousTime;
			}

			//	Update time text according to game mode
			if (gameMode == 0)
			{
				CurrentTimeText = "Time: " + currentTime;
			}
			else if (gameMode == 1)
			{
				CurrentTimeText = "Time: " + Convert.ToString((60 - Convert.ToInt32(currentTime)));
			}

			CheckGameEnd();
		}

		//	Set up data for loading game
		public void GetLoadGameData(int lives, int time, int score, int itemGame)
		{
			currentLives = lives;
			previousTime = time;
			currentScore = score;
			this.itemGame = itemGame;

			//	Update the screen's score, livse and time
			UpdateScoreLivesTime();
		}

		//	Function that check if the game ends according to the selected game mode
		private void CheckGameEnd()
		{
			//	Check if game ends
			if ((gameMode == 0 && currentLives == 0) || (gameMode == 1 && currentTime >= 60) && hasGameEnded == false)
			{
				//	Game ends
				hasGameEnded = true;

				//	Stop the timers
				timerCreateImage.Stop();
				timerUpdateScore.Stop();
				watch.Stop();

				//	Display text
				countdown.Visibility = Visibility.Visible;
				countdown.Text = "END";

				//	Delay three second and display score
				timerGameEnd.Tick += new EventHandler(Tick_GameEnd);
				timerGameEnd.Interval = TimeSpan.FromMilliseconds(3000);
				timerGameEnd.Start();
			}
		}

		//	IInteractionClient for DummyInteractionClient
		public interface IInteractionClient
		{
			InteractionInfo GetInteractionInfoAtLocation(
				int skeletonTrackingId,
				InteractionHandType handType,
				double x,
				double y
				);
		}

		//	Dummy Interaction Client with Interaction Info
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
			//System.Threading.Thread.Sleep(1000);

			//	Start counting down
			timerCountdown.Tick += new EventHandler(Tick_Countdown);
			timerCountdown.Interval = TimeSpan.FromMilliseconds(100);
			timerCountdown.Start();

			//	Show the text box
			countdown.Visibility = Visibility.Visible;
			countdown.Width = 1000;
		}

		private void Tick_Countdown(object source, EventArgs e)
		{
			if (countdownFlag == 4)
			{
				countdown.FontSize = 396;
				countdown.Text = (countdownFlag - 1).ToString();
				countdownFlag--;
			}
			else
			{
				countdownFlag -= 0.1;
				countdown.Text = ((int)countdownFlag + 1).ToString();
			}

			Canvas.SetLeft(countdown, screenWidth / 2 - countdown.ActualWidth / 2);
			Canvas.SetTop(countdown, screenHeight / 2 - countdown.ActualHeight / 2);

			if (countdownFlag <= 0)
			{
				//	Start the timer
				watch.Start();

				//	Start create images
				timerCreateImage.Tick += new EventHandler(Tick_CreateImage);
				timerCreateImage.Interval = TimeSpan.FromMilliseconds(1500);
				timerCreateImage.Start();
				timerCountdown.Stop();
				countdown.Visibility = Visibility.Hidden;

				//	Start updating score
				timerUpdateScore.Tick += new EventHandler(Tick_UpdateScore);
				timerUpdateScore.Interval = TimeSpan.FromMilliseconds(50);
				timerUpdateScore.Start();
			}
		}

		private void Tick_CreateImage(object source, EventArgs e)
		{
			//	Create new image
			Image image;

			//	Generate random number for the random recycleable item from the database
			random = new Random();
			itemGame = random.Next(1, 10);

			//	Get the specific item
			ItemsRepository.ItemsDto itemsDto = itemsRepository.GetItem(itemGame)[0];

			#region Randomize specific item drop down speed
			//	Randomize speed / difficulty
			randomVertical = new Random();

			//	If game mode is survival mode
			if (gameMode == 0)
			{
				if (previousTime + watch.ElapsedMilliseconds / 1000 <= 30)
				{
					//	If less than or equal to 30 seconds
					verticalLength = randomVertical.Next(1, 9); // 1 to 8
				}
				else if (previousTime + watch.ElapsedMilliseconds / 1000 <= 60)
				{
					//	If game time passes 30 seconds and before 1 minute
					verticalLength = randomVertical.Next(6, 13); //	6 to 12
				}
				else
				{
					//	If game time passes 1 minute
					verticalLength = randomVertical.Next(10, 22); // 10 to 21
				}
			}
			//	If game mode is time attack mode
			else if (gameMode == 1)
			{
				verticalLength = randomVertical.Next(3, 19); // 3 to 18
			}
			#endregion

			Application.Current.Dispatcher.Invoke(delegate
			{
				image = new Image
				{
					Width = imageWidth,
					Height = imageHeight,
					Source = new BitmapImage(new Uri(itemsDto.Item_Image_Link, UriKind.Relative)),
				};

				//	Randomize the X-axis position
				randomHorizontal = new Random();
				horizontalLength = randomHorizontal.Next(0, horizontalMaxLength);

				//	Add the new image to the canvas
				canvas.Children.Add(image);

				//	Set the X-axis position of the image
				Canvas.SetLeft(canvas.Children[canvas.Children.Count - 1], horizontalLength);

				//	Create new item drop instant for the newly create item/image
				ItemDrop itemDrop = new ItemDrop
				{
					ItemType = itemsDto.Item_Type,
					DropSpeed = verticalLength,
					ItemWidth = imageWidth,
					ItemHeight = imageHeight,
					VerticalMaxLength = verticalMaxLength,
					ImageSource = new BitmapImage(new Uri(itemsDto.Item_Image_Link, UriKind.Relative)),
					ItemImage = image,
					BlueBin = blueBin,
					OrangeBin = orangeBin,
					BrownBin = brownBin,
					CanvasGame = canvas,
					ItemGrippedObject = itemGripped,
				};
			});
		}

		private void Tick_UpdateScore(object source, EventArgs e)
		{
			//	Get the latest data
			currentScore = itemGripped.CurrentScore;
			currentLives = itemGripped.CurrentLives;

			//	Update the score
			UpdateScoreLivesTime();
		}

		//	Display final score after game end
		private void Tick_GameEnd(object source, EventArgs e)
		{
			//	Stop the one time timer
			timerGameEnd.Stop();

			//	Update the score
			ScoreRepository scoreRepository = new ScoreRepository();
			scoreRepository.AddScore(currentScore, playerID, gameMode);

			//	Delete previous saved game
			GameRepository gameRepository = new GameRepository();
			if (gameRepository.GetGame(playerID).Count > 0)
			{
				gameRepository.DeleteGame(playerID);
			}

			//	Display the score
			countdown.Text = "FINAL SCORE:\n" + currentScore;
			countdown.FontSize = 200;
			countdown.Width = screenWidth;
			Canvas.SetLeft(countdown, 0);
			Canvas.SetTop(countdown, screenHeight / 2 - countdown.ActualHeight / 2);

			//	Auto back after 5 seconds
			timerGameBack.Tick += new EventHandler(Tick_GameBack);
			timerGameBack.Interval = TimeSpan.FromMilliseconds(3000);
			timerGameBack.Start();
		}

		//	Close the game 3 seconds after game end
		private void Tick_GameBack(object source, EventArgs e)
		{
			//	Stop the one time timer
			timerGameBack.Stop();

			//	Close the window
			Close();
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
					
					if (lastHandEvent == InteractionHandEventType.Grip)
					{
						//	If hand gripped, show gripped cursor and move the item with the cursor
						handCursor.Source = new BitmapImage(new Uri("Resources/pointerWhite.png", UriKind.Relative));

						//	Find the item number of which the hand cursor is on
						if (itemGripped.HandCursorOn == -1 && itemGripped.HandCursorOnSet == false)
						{
							for (int i = itemChildrenStart; i < canvas.Children.Count; i++)
							{
								var childrenPoint = canvas.Children[i].TranslatePoint(new Point(0, 0), canvas);
								var handCursorPoint = handCursor.TranslatePoint(new Point(0, 0), canvas);
								if (handCursorPoint.X > childrenPoint.X && handCursorPoint.X < childrenPoint.X +
									itemWidth && handCursorPoint.Y > childrenPoint.Y && handCursorPoint.Y < childrenPoint.Y + itemHeight)
								{
									itemGripped.HandCursorOn = i;
									itemGripped.HandCursorOnLeftDistant = handCursorPoint.X - childrenPoint.X;
									itemGripped.HandCursorOnTopDistant = handCursorPoint.Y - childrenPoint.Y;
									itemGripped.HandCursorOnSet = true;
									break;
								}
							}
						}

						//	Move the item with the hand cursor
						if (itemGripped.HandCursorOn != -1)
						{
							var p = handCursor.TranslatePoint(new Point(0, 0), canvas);

							try
							{
								Canvas.SetLeft(canvas.Children[itemGripped.HandCursorOn], p.X - itemGripped.HandCursorOnLeftDistant);
								Canvas.SetTop(canvas.Children[itemGripped.HandCursorOn], p.Y - itemGripped.HandCursorOnTopDistant);
							}
							catch (Exception)
							{

							}
						}
					}
					else if (lastHandEvent == InteractionHandEventType.GripRelease)
					{
						//	If hand grip released, show normal cursor
						handCursor.Source = new BitmapImage(new Uri("Resources/handWhite.png", UriKind.Relative));
						itemGripped.HandCursorOn = -1;
						itemGripped.HandCursorOnSet = false;
					}
					else if (hand.IsPressed)
					{
						//	If hand pressed the back, go back:

						//	Get points of hand cursor and back button
						var handCursorPoint = handCursor.TranslatePoint(new Point(0, 0), canvas);
						var backPoint = handCursor.TranslatePoint(new Point(0, 0), canvas);

						//	Left and Right check:
						if (back.Width + backPoint.X >= handCursorPoint.X && handCursorPoint.X >= backPoint.X)
						{
							//	Top and Bottom check:
							if (back.Height + backPoint.Y >= handCursorPoint.Y && handCursorPoint.Y >= backPoint.Y)
							{
								//	If 'watch' timer is running, then save the game first
								//	If not, just close the window
								if (watch.IsRunning)
								{
									//	Stop the timer
									watch.Stop();

									//	Create new GameRepository object
									GameRepository gro = new GameRepository();
									//	Get the playing time

									currentTime = previousTime + watch.ElapsedMilliseconds / 1000;

									//	If the player ID exists in the database (Playing loaded game), update the row to the current game, else insert a new row
									if (gro.GetGame(playerID).Count == 0)
									{
										//	Add a new game to the database
										gro.AddGame(currentLives, playerID, currentTime, currentScore, itemGame, gameMode);
									}
									else
									{
										//	Modify the game in the database
										gro.ModifyGame(currentLives, playerID, currentTime, currentScore, itemGame, gameMode);
									}

									//	Reset the timer
									watch.Reset();
								}
								//	Close the game window
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

		//	Disable maximizing window
		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Normal)
			{
				WindowState = WindowState.Maximized;
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
			SkeletonPoint point = new SkeletonPoint
			{
				X = ScaleVector(screenWidth, joint.Position.X),
				Y = ScaleVector(screenHeight, -joint.Position.Y),
				Z = joint.Position.Z
			};

			Joint scaledJoint = joint;
			//Joint scaledJoint = joint.ScaleTo(1920, 1080);

			scaledJoint.TrackingState = JointTrackingState.Tracked;
			scaledJoint.Position = point;

			Canvas.SetLeft(element, scaledJoint.Position.X);
			Canvas.SetTop(element, scaledJoint.Position.Y);
		}

		//	Scale the X and Y to the screen size: Kinect 640x480 30 fps
		private float ScaleVector(int length, float position)
		{
			float value = (((length / 1f) / 2f) * position) + (length / 2);
			if (value > length)
			{
				return length;
			}
			if (value < 0f)
			{
				return 0f;
			}
			return value;
		}

		private float ScaleVectorX(int length, float position)
		{
			float value = position * screenFactorX;

			if (value > screenWidth)
			{
				return screenWidth;
			}
			
			return value;
		}

		private float ScaleVectorY(int length, float position)
		{
			float value = position * screenFactorY - length / 2 ;

			if (value > screenHeight)
			{
				return screenHeight;
			}

			return value;
		}

		private void ProcessGesture(Joint rightHand)
		{
			SkeletonPoint point = new SkeletonPoint
			{
				X = ScaleVector(screenWidth, rightHand.Position.X),
				Y = ScaleVector(screenHeight, -rightHand.Position.Y),
				Z = rightHand.Position.Z
			};

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

				try
				{
					//DepthImagePoint rightDepthPoint =
				 //	depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);

					DepthImagePoint rightDepthPoint =
					kinectSensorChooser.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(first.Joints[JointType.HandRight].Position, DepthImageFormat.Resolution640x480Fps30);

					//ColorImagePoint rightColorPoint =
					//	depth.MapToColorImagePoint(rightDepthPoint.X, rightDepthPoint.Y,
					//	ColorImageFormat.RgbResolution640x480Fps30);

					ColorImagePoint rightColorPoint =
						kinectSensorChooser.Kinect.CoordinateMapper.MapDepthPointToColorPoint(DepthImageFormat.Resolution640x480Fps30,
						rightDepthPoint, ColorImageFormat.RgbResolution640x480Fps30);

					CameraPosition(handCursor, rightColorPoint);
				}
				catch (Exception)
				{

				}
			}
		}

		private void CameraPosition(FrameworkElement element, ColorImagePoint point)
		{
			// 640 x 480
			Canvas.SetLeft(element, point.X * (screenFactorX + 0.5) - element.Width / 2);
			Canvas.SetTop(element, point.Y * (screenFactorY + 0.5) - element.Height / 2);
		}

		private void Window_Closing(object sender, EventArgs e)
		{
			timerCreateImage.Stop();
			timerUpdateScore.Stop();

			/*if (sensor != null)
				sensor.Stop();

			kinectSensorChooser.Stop();*/
		}

		//	Temporary click function
		private void back_Click(object sender, RoutedEventArgs e)
		{
			//	Save Game
			if (watch.IsRunning)
			{
				watch.Stop();
				GameRepository gro = new GameRepository();
				currentTime = previousTime + watch.ElapsedMilliseconds / 1000;
				if (gro.GetGame(playerID).Count == 0)
				{
					gro.AddGame(currentLives, playerID, currentTime, currentScore, itemGame, gameMode);
				}
				else
				{
					gro.ModifyGame(currentLives, playerID, currentTime, currentScore, itemGame, gameMode);
				}
				watch.Reset();
			}

			//	Close the game window
			Close();
		}
	}
}
