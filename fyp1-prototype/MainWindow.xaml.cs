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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Timers;
//todo: we self reference to this project
//todo: image size according to margin
//todo: conclusion
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

			/*
			KinectRegion.AddHandPointerEnterHandler(wpfbtn1, OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(wpfbtn1, OnHandPointerLeave);

			KinectRegion.AddHandPointerEnterHandler(wpfbtn2, OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(wpfbtn2, OnHandPointerLeave);

			KinectRegion.AddHandPointerEnterHandler(wpfbtn3, OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(wpfbtn3, OnHandPointerLeave);


			KinectRegion.AddHandPointerGotCaptureHandler(wpfbtn1, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(wpfbtn1, OnHandPointerLostCapture);

			KinectRegion.AddHandPointerGotCaptureHandler(wpfbtn2, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(wpfbtn2, OnHandPointerLostCapture);

			KinectRegion.AddHandPointerGotCaptureHandler(wpfbtn3, OnHandPointerGotCapture);
			KinectRegion.AddHandPointerLostCaptureHandler(wpfbtn3, OnHandPointerLostCapture);


			KinectRegion.AddHandPointerPressHandler(wpfbtn1, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(wpfbtn1, OnHandPointerPressRelease);

			KinectRegion.AddHandPointerPressHandler(wpfbtn2, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(wpfbtn2, OnHandPointerPressRelease);

			KinectRegion.AddHandPointerPressHandler(wpfbtn3, OnHandPointerPress);
			KinectRegion.AddHandPointerPressReleaseHandler(wpfbtn3, OnHandPointerPressRelease);

			KinectRegion.SetIsPressTarget(wpfbtn1, true);
			KinectRegion.SetIsPressTarget(wpfbtn2, true);
			KinectRegion.SetIsPressTarget(wpfbtn3, true);	*/
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
		{/*
			if (KinectRegion.GetIsPrimaryHandPointerOver(wpfbtn1))
			{
				VisualStateManager.GoToState(wpfbtn1, "MouseOver", true);
				e.Handled = true;
			}
			else if (KinectRegion.GetIsPrimaryHandPointerOver(wpfbtn2))
			{
				VisualStateManager.GoToState(wpfbtn2, "MouseOver", true);
				e.Handled = true;
			}
			else if (KinectRegion.GetIsPrimaryHandPointerOver(wpfbtn3))
			{
				VisualStateManager.GoToState(wpfbtn3, "MouseOver", true);
				e.Handled = true;
			}*/
		}

		private void OnHandPointerLeave(object sender, HandPointerEventArgs e)
		{/*
			if (!KinectRegion.GetIsPrimaryHandPointerOver(wpfbtn1))
			{
				VisualStateManager.GoToState(wpfbtn1, "Normal", true);
			}
			if (!KinectRegion.GetIsPrimaryHandPointerOver(wpfbtn2))
			{
				VisualStateManager.GoToState(wpfbtn2, "Normal", true);
			}
			if (!KinectRegion.GetIsPrimaryHandPointerOver(wpfbtn3))
			{
				VisualStateManager.GoToState(wpfbtn3, "Normal", true);
			}

			if (capturedHandPointer == e.HandPointer)
			{
				capturedHandPointer = null;
				e.Handled = true;
			}*/
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