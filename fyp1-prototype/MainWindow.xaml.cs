using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Timers;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private KinectSensorChooser kinectSensorChooser;
		private HandPointer capturedHandPointer;

		public MainWindow()
		{
			InitializeComponent();

			kinectSensorChooser = new KinectSensorChooser();
			kinectSensorChooser.KinectChanged += KinectSensorChooser_KinectChanged;
			kinectSensorDisplay.KinectSensorChooser = kinectSensorChooser;

			kinectSensorChooser.Start();

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			//	Hand pointer enter and leave event handler
			KinectRegion.AddHandPointerEnterHandler(btn_login, OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(btn_login, OnHandPointerLeave);

			KinectRegion.AddHandPointerEnterHandler(btn_register, OnHandPointerEnter);
			KinectRegion.AddHandPointerEnterHandler(btn_register, OnHandPointerEnter);

			KinectRegion.AddHandPointerLeaveHandler(btn_singlePlayer, OnHandPointerLeave);
			KinectRegion.AddHandPointerEnterHandler(btn_singlePlayer, OnHandPointerEnter);

			KinectRegion.AddHandPointerEnterHandler(btn_multiPlayer, OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(btn_multiPlayer, OnHandPointerLeave);

			KinectRegion.AddHandPointerEnterHandler(btn_loadGame, OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(btn_loadGame, OnHandPointerLeave);

			KinectRegion.AddHandPointerEnterHandler(btn_highScores, OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(btn_highScores, OnHandPointerLeave);

			KinectRegion.AddHandPointerEnterHandler(btn_help, OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(btn_help, OnHandPointerLeave);

			KinectRegion.AddHandPointerEnterHandler(btn_exit, OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(btn_exit, OnHandPointerLeave);

			//	Hand pointer got capture and lost capture event handler
			KinectRegion.AddHandPointerGotCaptureHandler(btn_login, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(btn_login, OnHandPointerLostCapture);

			KinectRegion.AddHandPointerGotCaptureHandler(btn_register, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(btn_register, OnHandPointerLostCapture);

			KinectRegion.AddHandPointerGotCaptureHandler(btn_singlePlayer, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(btn_singlePlayer, OnHandPointerLostCapture);

			KinectRegion.AddHandPointerGotCaptureHandler(btn_multiPlayer, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(btn_multiPlayer, OnHandPointerLostCapture);

			KinectRegion.AddHandPointerGotCaptureHandler(btn_loadGame, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(btn_loadGame, OnHandPointerLostCapture);

			KinectRegion.AddHandPointerGotCaptureHandler(btn_highScores, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(btn_highScores, OnHandPointerLostCapture);

			KinectRegion.AddHandPointerGotCaptureHandler(btn_help, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(btn_help, OnHandPointerLostCapture);

			KinectRegion.AddHandPointerGotCaptureHandler(btn_exit, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(btn_exit, OnHandPointerLostCapture);

			//	Hand Pointer press and press release handler
			KinectRegion.AddHandPointerPressHandler(btn_login, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(btn_login, OnHandPointerPressRelease);

			KinectRegion.AddHandPointerPressHandler(btn_register, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(btn_register, OnHandPointerPressRelease);

			KinectRegion.AddHandPointerPressHandler(btn_singlePlayer, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(btn_singlePlayer, OnHandPointerPressRelease);

			KinectRegion.AddHandPointerPressHandler(btn_multiPlayer, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(btn_multiPlayer, OnHandPointerPressRelease);

			KinectRegion.AddHandPointerPressHandler(btn_loadGame, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(btn_loadGame, OnHandPointerPressRelease);

			KinectRegion.AddHandPointerPressHandler(btn_highScores, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(btn_highScores, OnHandPointerPressRelease);

			KinectRegion.AddHandPointerPressHandler(btn_help, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(btn_help, OnHandPointerPressRelease);

			KinectRegion.AddHandPointerPressHandler(btn_exit, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(btn_exit, OnHandPointerPressRelease);

			KinectRegion.SetIsPressTarget(btn_login, true);
			KinectRegion.SetIsPressTarget(btn_register, true);
			KinectRegion.SetIsPressTarget(btn_singlePlayer, true);
			KinectRegion.SetIsPressTarget(btn_multiPlayer, true);
			KinectRegion.SetIsPressTarget(btn_loadGame, true);
			KinectRegion.SetIsPressTarget(btn_highScores, true);
			KinectRegion.SetIsPressTarget(btn_help, true);
			KinectRegion.SetIsPressTarget(btn_exit, true);
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
				kinectKinectRegion.KinectSensor = e.NewSensor;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			kinectSensorChooser.Stop();
		}

		private void OnHandPointerEnter(object sender, HandPointerEventArgs e)
		{
			if (KinectRegion.GetIsPrimaryHandPointerOver(btn_login))
			{
				VisualStateManager.GoToState(btn_login, "MouseOver", true);
				e.Handled = true;
			}
			else if (KinectRegion.GetIsPrimaryHandPointerOver(btn_register))
			{
				VisualStateManager.GoToState(btn_register, "MouseOver", true);
				e.Handled = true;
			}
			else if (KinectRegion.GetIsPrimaryHandPointerOver(btn_singlePlayer))
			{
				VisualStateManager.GoToState(btn_singlePlayer, "MouseOver", true);
				e.Handled = true;
			}
			else if (KinectRegion.GetIsPrimaryHandPointerOver(btn_multiPlayer))
			{
				VisualStateManager.GoToState(btn_multiPlayer, "MouseOver", true);
				e.Handled = true;
			}
			else if (KinectRegion.GetIsPrimaryHandPointerOver(btn_loadGame))
			{
				VisualStateManager.GoToState(btn_loadGame, "MouseOver", true);
				e.Handled = true;
			}
			else if (KinectRegion.GetIsPrimaryHandPointerOver(btn_highScores))
			{
				VisualStateManager.GoToState(btn_highScores, "MouseOver", true);
				e.Handled = true;
			}
			else if (KinectRegion.GetIsPrimaryHandPointerOver(btn_help))
			{
				VisualStateManager.GoToState(btn_help, "MouseOver", true);
				e.Handled = true;
			}
			else if (KinectRegion.GetIsPrimaryHandPointerOver(btn_exit))
			{
				VisualStateManager.GoToState(btn_exit, "MouseOver", true);
				e.Handled = true;
			}
		}

		private void OnHandPointerLeave(object sender, HandPointerEventArgs e)
		{
			if (!KinectRegion.GetIsPrimaryHandPointerOver(btn_login))
			{
				VisualStateManager.GoToState(btn_login, "Normal", true);
			}
			if (!KinectRegion.GetIsPrimaryHandPointerOver(btn_register))
			{
				VisualStateManager.GoToState(btn_register, "Normal", true);
			}
			if (!KinectRegion.GetIsPrimaryHandPointerOver(btn_singlePlayer))
			{
				VisualStateManager.GoToState(btn_singlePlayer, "Normal", true);
			}
			if (!KinectRegion.GetIsPrimaryHandPointerOver(btn_multiPlayer))
			{
				VisualStateManager.GoToState(btn_multiPlayer, "Normal", true);
			}
			if (!KinectRegion.GetIsPrimaryHandPointerOver(btn_loadGame))
			{
				VisualStateManager.GoToState(btn_loadGame, "Normal", true);
			}
			if (!KinectRegion.GetIsPrimaryHandPointerOver(btn_highScores))
			{
				VisualStateManager.GoToState(btn_highScores, "Normal", true);
			}
			if (!KinectRegion.GetIsPrimaryHandPointerOver(btn_help))
			{
				VisualStateManager.GoToState(btn_help, "Normal", true);
			}
			if (!KinectRegion.GetIsPrimaryHandPointerOver(btn_exit))
			{
				VisualStateManager.GoToState(btn_exit, "Normal", true);
			}

			if (capturedHandPointer == e.HandPointer)
			{
				capturedHandPointer = null;
				e.Handled = true;
			}
		}

		private void OnHandPointerGotCapture(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == null)
			{
				capturedHandPointer = e.HandPointer;
				e.Handled = true;
			}
		}

		private void OnHandPointerLostCapture(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				capturedHandPointer = null;
				e.Handled = true;
			}
		}

		private void OnHandPointerPress(object sender, HandPointerEventArgs e)
		{/*
			if (capturedHandPointer == null && e.HandPointer.IsPrimaryUser && e.HandPointer.IsPrimaryHandOfUser)
			{
				if (e.HandPointer.GetIsOver(wpfbtn1))
				{
					statusHolder.Content = "WPF BUTTON One Press";
					e.HandPointer.Capture(wpfbtn1);
				}
				else if (e.HandPointer.GetIsOver(wpfbtn2))
				{
					statusHolder.Content = "WPF BUTTON Two Press";
					e.HandPointer.Capture(wpfbtn2);
				}
				else if (e.HandPointer.GetIsOver(wpfbtn3))
				{
					statusHolder.Content = "WPF BUTTON Three Press";
					e.HandPointer.Capture(wpfbtn3);
				}
				e.Handled = true;
			}*/
		}

		private void OnHandPointerPressRelease(object sender, HandPointerEventArgs e)
		{/*
			if (capturedHandPointer == e.HandPointer)
			{
				e.HandPointer.Capture(null);
				if (e.HandPointer.GetIsOver(wpfbtn1))
				{
					statusHolder.Content = "WPF BUTTON One Press Release";
					VisualStateManager.GoToState(wpfbtn1, "MouseOver", true);
				}
				else if (e.HandPointer.GetIsOver(wpfbtn2))
				{
					OnPressWpfBtn2();
					statusHolder.Content = "WPF BUTTON Two Press Release";
					VisualStateManager.GoToState(wpfbtn2, "MouseOver", true);
				}
				else if (e.HandPointer.GetIsOver(wpfbtn3))
				{
					OnPressWpfBtn3();
					statusHolder.Content = "WPF BUTTON Three Press Release";
					VisualStateManager.GoToState(wpfbtn3, "MouseOver", true);
				}
				else
				{
					VisualStateManager.GoToState(wpfbtn1, "Normal", true);
					VisualStateManager.GoToState(wpfbtn2, "Normal", true);
					VisualStateManager.GoToState(wpfbtn3, "Normal", true);
				}
				e.Handled = true;
			}*/
		}

		private void OnPressWpfBtn2()
		{
			DragDropImages two = new DragDropImages(kinectSensorChooser);
			two.ShowDialog();
		}

		private void OnPressWpfBtn3()
		{
			HighScore one = new HighScore(kinectSensorChooser);
			one.ShowDialog();
		}

		private void wpfbtn2_Click(object sender, RoutedEventArgs e)
		{
			Image image = new Image();
			image.Width = 150;
			image.Height = 150;
			//image.Source = BitmapToImageSource(Properties.Resources.test);
			//canvas.Children.Add(image);
		}

		private void wpfbtn3_Click(object sender, RoutedEventArgs e)
		{
			
			//Canvas.SetLeft(canvas.Children[0], p.X + 10);

			var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
			//dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
			dispatcherTimer.Start();
		}

		private void dispatcherTimer_Tick(object source, EventArgs e)
		{
			//var p = canvas.Children[0].TranslatePoint(new Point(0, 0), canvas);
			//Canvas.SetTop(canvas.Children[0], p.Y + 10);
		}

		private void close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void highscore(object sender, RoutedEventArgs e)
		{
			HighScore highScore = new HighScore(kinectSensorChooser);
			highScore.Show();
		}

		private void singlePlayer(object sender, RoutedEventArgs e)
		{
			DragDropImages dragDropImages = new DragDropImages(kinectSensorChooser);
			dragDropImages.Owner = Application.Current.MainWindow;
			dragDropImages.Show();
		}
	}
}