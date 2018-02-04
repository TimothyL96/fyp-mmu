using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace fyp1_prototype
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class Login : Window
    {
		private HandPointer capturedHandPointer;
		private KinectSensorChooser kinectSensorChooser;

		public Login(KinectSensorChooser kinectSensorChooser)
        {
            InitializeComponent();
			
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

			this.kinectSensorChooser = kinectSensorChooser;

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			//	Setup Kinect region press target and event handlers
			KinectRegion.SetIsPressTarget(btnCancelLogin, true);
			KinectRegion.SetIsPressTarget(btnLogin, true);

			//	btnCancelLogin
			KinectRegion.AddHandPointerEnterHandler(btnCancelLogin, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnCancelLogin, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnCancelLogin, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnCancelLogin, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnCancelLogin, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnCancelLogin, HandPointerLostCaptureEvent);

			//	btnLogin
			KinectRegion.AddHandPointerEnterHandler(btnLogin, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnLogin, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnLogin, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnLogin, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnLogin, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnLogin, HandPointerLostCaptureEvent);
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(btnCancelLogin) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(btnCancelLogin, "MouseOver", true);
			else if (e.HandPointer.GetIsOver(btnLogin) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(btnLogin, "MouseOver", true);

			e.Handled = true;
		}

		private void HandPointerLeaveEvent(object sender, HandPointerEventArgs e)
		{
			if (!e.HandPointer.GetIsOver(btnCancelLogin) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(btnCancelLogin, "Normal", true);
			if (!e.HandPointer.GetIsOver(btnLogin) && e.HandPointer.IsPrimaryHandOfUser)
				VisualStateManager.GoToState(btnLogin, "Normal", true);

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
				if (e.HandPointer.GetIsOver(btnCancelLogin))
				{
					e.HandPointer.Capture(btnCancelLogin);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
				else if (e.HandPointer.GetIsOver(btnLogin))
				{
					e.HandPointer.Capture(btnLogin);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
			}
		}

		private void HandPointerPressReleaseEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				if (e.HandPointer.GetIsOver(btnCancelLogin))
				{
					Close();
					VisualStateManager.GoToState(btnCancelLogin, "MouseOver", true);
				}
				else if (e.HandPointer.GetIsOver(btnLogin))
				{
					VisualStateManager.GoToState(btnLogin, "MouseOver", true);
				}
				else
				{
					VisualStateManager.GoToState(btnCancelLogin, "Normal", true);
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

		//	Temporary click function
		private void goback(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void login(object sender, RoutedEventArgs e)
		{
			//	Setup custom message box
			CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);

			//	Check if username entered is at least 3 characters long
			if (textBoxUsername.Text.Length < 3)
			{
				//	Show error message dialog
				customMessageBox.ShowText("Username is at least 3 characters long!");

				//	Return to stop executing the remaining codes
				return;
			}
			//	Check password length is at least 3 characters long
			else if (passwordBox.Password.Length < 3)
			{
				//	Show error message dialog
				customMessageBox.ShowText("Password is at least 3 characters long!");

				//	Return to stop executing the remaining codes
				return;
			}

			//	Create the player repository object
			PlayersRepository pro = new PlayersRepository();

			//	Hash the password
			SHA256 sha256 = SHA256.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(passwordBox.Password);
			byte[] hash = sha256.ComputeHash(bytes);

			StringBuilder result = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				result.Append(hash[i].ToString("X2"));
			}

			//	Search database for this username
			List<PlayersRepository.PlayerDto> player = pro.GetPlayerWithUsername(textBoxUsername.Text);

			//	If player exist
			if (player.Count == 1)
			{
				//	If password match then log them in
				if (result.ToString().Contains(player[0].Password))
				{
					//	Show dialog that the login is successful
					customMessageBox.ShowText("Log in succeeded");

					//	Close after succesfully logged in
					Close();
				}
				else
				{
					//	Show dialog that the login is not successful
					customMessageBox.ShowText("Password is incorrect!");

					//	Clear the incorrect password
					passwordBox.Password = "";
				}
			}
			else
			{
				//	Show dialog that the login is not successful
				customMessageBox.ShowText("Player doesn't exist!");
			}
		}
	}
}
