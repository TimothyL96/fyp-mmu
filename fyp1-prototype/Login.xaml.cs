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
		private const int hoverSizeChange = 50;
		private char keyPressed;

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

			KinectRegion.SetIsPressTarget(btnA, true);
			KinectRegion.SetIsPressTarget(btnB, true);
			KinectRegion.SetIsPressTarget(btnC, true);
			KinectRegion.SetIsPressTarget(btnD, true);
			KinectRegion.SetIsPressTarget(btnE, true);
			KinectRegion.SetIsPressTarget(btnF, true);
			KinectRegion.SetIsPressTarget(btnG, true);
			KinectRegion.SetIsPressTarget(btnH, true);
			KinectRegion.SetIsPressTarget(btnI, true);
			KinectRegion.SetIsPressTarget(btnJ, true);
			KinectRegion.SetIsPressTarget(btnK, true);
			KinectRegion.SetIsPressTarget(btnL, true);
			KinectRegion.SetIsPressTarget(btnM, true);
			KinectRegion.SetIsPressTarget(btnN, true);
			KinectRegion.SetIsPressTarget(btnO, true);
			KinectRegion.SetIsPressTarget(btnP, true);
			KinectRegion.SetIsPressTarget(btnQ, true);
			KinectRegion.SetIsPressTarget(btnR, true);
			KinectRegion.SetIsPressTarget(btnS, true);
			KinectRegion.SetIsPressTarget(btnT, true);
			KinectRegion.SetIsPressTarget(btnU, true);
			KinectRegion.SetIsPressTarget(btnV, true);
			KinectRegion.SetIsPressTarget(btnW, true);
			KinectRegion.SetIsPressTarget(btnX, true);
			KinectRegion.SetIsPressTarget(btnY, true);
			KinectRegion.SetIsPressTarget(btnZ, true);

			KinectRegion.SetIsPressTarget(textBoxUsername, true);
			KinectRegion.SetIsPressTarget(passwordBox, true);

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

			//	textBoxUsername
			KinectRegion.AddHandPointerEnterHandler(textBoxUsername, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(textBoxUsername, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(textBoxUsername, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(textBoxUsername, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(textBoxUsername, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(textBoxUsername, HandPointerLostCaptureEvent);

			//	passwordBox
			KinectRegion.AddHandPointerEnterHandler(passwordBox, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(passwordBox, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(passwordBox, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(passwordBox, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(passwordBox, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(passwordBox, HandPointerLostCaptureEvent);

			//	Keyboard: Register event handlers
			//	1 - AddHandPointerEnterHandler
			KinectRegion.AddHandPointerEnterHandler(btnA, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnB, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnC, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnD, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnE, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnF, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnG, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnH, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnI, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnJ, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnK, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnL, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnM, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnN, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnO, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnP, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnQ, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnR, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnS, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnT, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnU, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnV, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnW, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnX, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnY, HandPointerEnterEvent);
			KinectRegion.AddHandPointerEnterHandler(btnZ, HandPointerEnterEvent);

			//	2 - HandPointerEnterHandler
			KinectRegion.AddHandPointerLeaveHandler(btnA, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnB, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnC, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnD, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnE, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnF, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnG, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnH, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnI, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnJ, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnK, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnL, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnM, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnN, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnO, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnP, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnQ, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnR, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnS, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnT, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnU, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnV, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnW, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnX, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnY, HandPointerLeaveEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnZ, HandPointerLeaveEvent);

			//	3 - AddHandPointerPressHandler
			KinectRegion.AddHandPointerPressHandler(btnA, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnB, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnC, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnD, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnE, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnF, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnG, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnH, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnI, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnJ, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnK, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnL, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnM, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnN, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnO, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnP, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnQ, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnR, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnS, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnT, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnU, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnV, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnW, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnX, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnY, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressHandler(btnZ, HandPointerPressEvent);

			//	4 - AddHandPointerPressReleaseHandler
			KinectRegion.AddHandPointerPressReleaseHandler(btnA, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnB, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnC, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnD, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnE, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnF, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnG, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnH, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnI, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnJ, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnK, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnL, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnM, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnN, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnO, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnP, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnQ, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnR, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnS, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnT, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnU, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnV, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnW, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnX, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnY, HandPointerPressReleaseEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnZ, HandPointerPressReleaseEvent);

			//	5 - AddHandPointerGotCaptureHandler
			KinectRegion.AddHandPointerGotCaptureHandler(btnA, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnB, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnC, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnD, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnE, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnF, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnG, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnH, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnI, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnJ, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnK, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnL, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnM, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnN, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnO, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnP, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnQ, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnR, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnS, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnT, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnU, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnV, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnW, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnX, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnY, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerGotCaptureHandler(btnZ, HandPointerCaptureEvent);

			//	6 - AddHandPointerLostCaptureHandler
			KinectRegion.AddHandPointerLostCaptureHandler(btnA, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnB, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnC, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnD, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnE, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnF, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnG, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnH, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnI, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnJ, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnK, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnL, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnM, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnN, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnO, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnP, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnQ, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnR, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnS, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnT, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnU, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnV, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnW, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnX, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnY, HandPointerLostCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnZ, HandPointerLostCaptureEvent);
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(btnCancelLogin) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnCancelLogin, "MouseOver", true);
			}
			else if (e.HandPointer.GetIsOver(btnLogin) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnLogin, "MouseOver", true);
			}
			else if (e.HandPointer.GetIsOver(textBoxUsername) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(textBoxUsername, "MouseOver", true);
			}
			else if (e.HandPointer.GetIsOver(passwordBox) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(passwordBox, "MouseOver", true);
			}
			else if (e.HandPointer.GetIsOver((Button)sender) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState((Button)sender, "MouseOver", true);

				Button button = (Button)sender;
				button.RenderTransformOrigin = new Point(0.5, 0.5);
				ScaleTransform scaleTransform = new ScaleTransform
				{
					ScaleX = 1.8,
					ScaleY = 1.8
				};
				TransformGroup transformGroup = new TransformGroup();
				transformGroup.Children.Add(scaleTransform);
				button.RenderTransform = transformGroup;
			}				

			e.Handled = true;
		}

		private void HandPointerLeaveEvent(object sender, HandPointerEventArgs e)
		{
			if (!e.HandPointer.GetIsOver(btnCancelLogin) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnCancelLogin, "Normal", true);
			}
			if (!e.HandPointer.GetIsOver(btnLogin) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnLogin, "Normal", true);
			}
			if (!e.HandPointer.GetIsOver(textBoxUsername) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(textBoxUsername, "Normal", true);
			}
			if (!e.HandPointer.GetIsOver(passwordBox) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(passwordBox, "Normal", true);
			}
			if (!e.HandPointer.GetIsOver((Button)sender) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState((Button)sender, "Normal", true);

				Button button = (Button)sender;
				button.RenderTransformOrigin = new Point(0.5, 0.5);
				ScaleTransform scaleTransform = new ScaleTransform
				{
					ScaleX = 1,
					ScaleY = 1
				};
				TransformGroup transformGroup = new TransformGroup();
				transformGroup.Children.Add(scaleTransform);
				button.RenderTransform = transformGroup;
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
				else if (e.HandPointer.GetIsOver(textBoxUsername))
				{
					e.HandPointer.Capture(textBoxUsername);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
				else if (e.HandPointer.GetIsOver(passwordBox))
				{
					e.HandPointer.Capture(passwordBox);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
				else if (e.HandPointer.GetIsOver((Button)sender))
				{
					e.HandPointer.Capture((Button)sender);
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

					VisualStateManager.GoToState(btnLogin, "MouseOver", true);
				}
				else if (e.HandPointer.GetIsOver(textBoxUsername))
				{
					textBoxUsername.Focus();
					VisualStateManager.GoToState(textBoxUsername, "MouseOver", true);
				}
				else if (e.HandPointer.GetIsOver(passwordBox))
				{
					passwordBox.Focus();
					VisualStateManager.GoToState(passwordBox, "MouseOver", true);
				}
				else if (e.HandPointer.GetIsOver((Button)sender))
				{
					Button button = (Button)sender;
					if (button.Equals(btnA))
					{
						keyPressed = 'a';
					}
					else if (button.Equals(btnB))
					{
						keyPressed = 'b';
					}
					else if (button.Equals(btnC))
					{
						keyPressed = 'c';
					}
					else if (button.Equals(btnD))
					{
						keyPressed = 'd';
					}
					else if (button.Equals(btnE))
					{
						keyPressed = 'e';
					}
					else if (button.Equals(btnF))
					{
						keyPressed = 'f';
					}
					else if (button.Equals(btnG))
					{
						keyPressed = 'g';
					}
					else if (button.Equals(btnH))
					{
						keyPressed = 'h';
					}
					else if (button.Equals(btnI))
					{
						keyPressed = 'i';
					}
					else if (button.Equals(btnJ))
					{
						keyPressed = 'j';
					}
					else if (button.Equals(btnK))
					{
						keyPressed = 'k';
					}
					else if (button.Equals(btnL))
					{
						keyPressed = 'l';
					}
					else if (button.Equals(btnM))
					{
						keyPressed = 'm';
					}
					else if (button.Equals(btnN))
					{
						keyPressed = 'n';
					}
					else if (button.Equals(btnO))
					{
						keyPressed = 'o';
					}
					else if (button.Equals(btnP))
					{
						keyPressed = 'p';
					}
					else if (button.Equals(btnQ))
					{
						keyPressed = 'q';
					}
					else if (button.Equals(btnR))
					{
						keyPressed = 'r';
					}
					else if (button.Equals(btnS))
					{
						keyPressed = 's';
					}
					else if (button.Equals(btnT))
					{
						keyPressed = 't';
					}
					else if (button.Equals(btnU))
					{
						keyPressed = 'u';
					}
					else if (button.Equals(btnV))
					{
						keyPressed = 'v';
					}
					else if (button.Equals(btnW))
					{
						keyPressed = 'w';
					}
					else if (button.Equals(btnX))
					{
						keyPressed = 'x';
					}
					else if (button.Equals(btnY))
					{
						keyPressed = 'y';
					}
					else if (button.Equals(btnZ))
					{
						keyPressed = 'z';
					}

					if (textBoxUsername.IsFocused)
					{
						int usernameCaretIndex = textBoxUsername.CaretIndex;
						textBoxUsername.Text = textBoxUsername.Text.Insert(usernameCaretIndex, keyPressed.ToString());
						textBoxUsername.CaretIndex = usernameCaretIndex + 1;
					}
					else if (passwordBox.IsFocused)
					{
						//	Due to security reason, caret index of passwordbox is not retrievable
						//	Use TextCompositionManager to input at current caret
						TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, passwordBox, keyPressed.ToString()));
					}
				}
				else
				{
					VisualStateManager.GoToState((Button)sender, "Normal", true);
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

		private void AMouseEnter(object sender, MouseEventArgs e)
		{
			Button button = (Button)sender;
			button.RenderTransformOrigin = new Point(0.5, .5);
			ScaleTransform scaleTransform = new ScaleTransform
			{
				ScaleX = 1.8,
				ScaleY = 1.8
			};
			TransformGroup transformGroup = new TransformGroup();
			transformGroup.Children.Add(scaleTransform);
			button.RenderTransform = transformGroup;
		}

		private void AMouseLeave(object sender, MouseEventArgs e)
		{
			Button button = (Button)sender;
			button.RenderTransformOrigin = new Point(0.5, .5);
			ScaleTransform scaleTransform = new ScaleTransform
			{
				ScaleX = 1,
				ScaleY = 1
			};
			TransformGroup transformGroup = new TransformGroup();
			transformGroup.Children.Add(scaleTransform);
			button.RenderTransform = transformGroup;
		}

		private void keyClick(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			if (button.Equals(btnA))
			{
				keyPressed = 'a';
			}
			else if (button.Equals(btnB))
			{
				keyPressed = 'b';
			}
			else if (button.Equals(btnC))
			{
				keyPressed = 'c';
			}
			else if (button.Equals(btnD))
			{
				keyPressed = 'd';
			}
			else if (button.Equals(btnE))
			{
				keyPressed = 'e';
			}
			else if (button.Equals(btnF))
			{
				keyPressed = 'f';
			}
			else if (button.Equals(btnG))
			{
				keyPressed = 'g';
			}
			else if (button.Equals(btnH))
			{
				keyPressed = 'h';
			}
			else if (button.Equals(btnI))
			{
				keyPressed = 'i';
			}
			else if (button.Equals(btnJ))
			{
				keyPressed = 'j';
			}
			else if (button.Equals(btnK))
			{
				keyPressed = 'k';
			}
			else if (button.Equals(btnL))
			{
				keyPressed = 'l';
			}
			else if (button.Equals(btnM))
			{
				keyPressed = 'm';
			}
			else if (button.Equals(btnN))
			{
				keyPressed = 'n';
			}
			else if (button.Equals(btnO))
			{
				keyPressed = 'o';
			}
			else if (button.Equals(btnP))
			{
				keyPressed = 'p';
			}
			else if (button.Equals(btnQ))
			{
				keyPressed = 'q';
			}
			else if (button.Equals(btnR))
			{
				keyPressed = 'r';
			}
			else if (button.Equals(btnS))
			{
				keyPressed = 's';
			}
			else if (button.Equals(btnT))
			{
				keyPressed = 't';
			}
			else if (button.Equals(btnU))
			{
				keyPressed = 'u';
			}
			else if (button.Equals(btnV))
			{
				keyPressed = 'v';
			}
			else if (button.Equals(btnW))
			{
				keyPressed = 'w';
			}
			else if (button.Equals(btnX))
			{
				keyPressed = 'x';
			}
			else if (button.Equals(btnY))
			{
				keyPressed = 'y';
			}
			else if (button.Equals(btnZ))
			{
				keyPressed = 'z';
			}

			if (textBoxUsername.IsFocused)
			{
				int usernameCaretIndex = textBoxUsername.CaretIndex;
				textBoxUsername.Text = textBoxUsername.Text.Insert(usernameCaretIndex, keyPressed.ToString());
				textBoxUsername.CaretIndex = usernameCaretIndex + 1;
			}
			else if (passwordBox.IsFocused)
			{
				//	Due to security reason, caret index of passwordbox is not retrievable
				//	Use TextCompositionManager to input at current caret
				TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, passwordBox, keyPressed.ToString()));
			}
		}
	}
}
