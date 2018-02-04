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
using System.Security.Cryptography;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit;

namespace fyp1_prototype
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
		private HandPointer capturedHandPointer;
		private KinectSensorChooser kinectSensorChooser;
		private const int hoverSizeChange = 50;
		private char keyPressed;

		public Register(KinectSensorChooser kinectSensorChooser)
        {
            InitializeComponent();

			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			this.kinectSensorChooser = kinectSensorChooser;

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			//	Setup Kinect region press target and event handlers
			KinectRegion.SetIsPressTarget(btnCancelRegister, true);
			KinectRegion.SetIsPressTarget(btnRegister, true);

			//	btnCancelRegister
			KinectRegion.AddHandPointerEnterHandler(btnCancelRegister, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnCancelRegister, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnCancelRegister, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnCancelRegister, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnCancelRegister, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnCancelRegister, HandPointerLostCaptureEvent);

			//	btnRegister
			KinectRegion.AddHandPointerEnterHandler(btnRegister, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnRegister, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnRegister, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnRegister, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnRegister, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnRegister, HandPointerLostCaptureEvent);
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(btnCancelRegister) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnCancelRegister, "MouseOver", true);
			}				
			else if (e.HandPointer.GetIsOver(btnRegister) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnRegister, "MouseOver", true);
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
			if (!e.HandPointer.GetIsOver(btnCancelRegister) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnCancelRegister, "Normal", true);
			}
			if (!e.HandPointer.GetIsOver(btnRegister) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnRegister, "Normal", true);
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
				if (e.HandPointer.GetIsOver(btnCancelRegister))
				{
					e.HandPointer.Capture(btnCancelRegister);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
				else if (e.HandPointer.GetIsOver(btnRegister))
				{
					e.HandPointer.Capture(btnRegister);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
			}
		}

		private void HandPointerPressReleaseEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				if (e.HandPointer.GetIsOver(btnCancelRegister))
				{
					Close();
					VisualStateManager.GoToState(btnCancelRegister, "MouseOver", true);
				}
				else if (e.HandPointer.GetIsOver(btnRegister))
				{
					//	Setup custom Message box
					CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);

					//	Check if username length is less than 3 or empty
					if (textBoxUsername.Text.Length < 3)
					{
						//	Show message dialog to enter longer username
						customMessageBox.ShowText("Username must be at least 3 characters long!");
						return;
					}
					//	Check if password length is less than 3
					else if (passwordBox.Password.Length < 3)
					{
						//	Show message dialog to enter longer password
						customMessageBox.ShowText("Password must be at least 3 characters long!");

						//	Return to stop executing
						return;
					}

					//	If the fields are correct, then check the data
					//	Check if password match
					if (passwordBox.Password == passwordBox1.Password)
					{
						//	Create the player repository object
						PlayersRepository pro = new PlayersRepository();

						//	Check if username exist
						List<String> allUsername = pro.GetAllPlayersUsername();
						bool usernameDuplicate = false;

						foreach (var username in allUsername)
						{
							if (username == textBoxUsername.Text)
							{
								usernameDuplicate = true;
								break;
							}
						}

						if (usernameDuplicate == false)
						{
							//	Hash the password
							SHA256 sha256 = SHA256.Create();
							byte[] bytes = Encoding.UTF8.GetBytes(passwordBox.Password);
							byte[] hash = sha256.ComputeHash(bytes);

							StringBuilder result = new StringBuilder();
							for (int i = 0; i < hash.Length; i++)
							{
								result.Append(hash[i].ToString("X2"));
							}

							//	Register player into database
							pro.AddPlayer(textBoxUsername.Text, result.ToString());

							//	Popup successful registration dialog
							customMessageBox.ShowText("Registration succeeded!");

							//	Close after successful registration
							Close();
						}
						else
						{
							//	Feedback to user that the entered username is not available
							customMessageBox.ShowText("Username already taken! Try another");
						}
					}
					else
					{
						//	Feedback to user that the passwords do not match
						customMessageBox.ShowText("Password do not match! Try again");
					}

					VisualStateManager.GoToState(btnRegister, "MouseOver", true);
				}
				else
				{
					VisualStateManager.GoToState(btnCancelRegister, "Normal", true);
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

		//	Temporary click functions
		private void goback(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void register(object sender, RoutedEventArgs e)
		{
			//	Setup custom Message box
			CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);

			//	Check if username length is less than 3 or empty
			if (textBoxUsername.Text.Length < 3)
			{
				//	Show message dialog to enter longer username
				customMessageBox.ShowText("Username must be at least 3 characters long!");
				return;
			}
			//	Check if password length is less than 3
			else if (passwordBox.Password.Length < 3)
			{
				//	Show message dialog to enter longer password
				customMessageBox.ShowText("Password must be at least 3 characters long!");

				//	Return to stop executing
				return;
			}

			//	If the fields are correct, then check the data
			//	Check if password match
			if (passwordBox.Password == passwordBox1.Password)
			{
				//	Create the player repository object
				PlayersRepository pro = new PlayersRepository();

				//	Check if username exist
				List<String> allUsername = pro.GetAllPlayersUsername();
				bool usernameDuplicate = false;

				foreach(var username in allUsername)
				{
					if (username == textBoxUsername.Text)
					{
						usernameDuplicate = true;
						break;
					}
				}

				if (usernameDuplicate == false)
				{
					//	Hash the password
					SHA256 sha256 = SHA256.Create();
					byte[] bytes = Encoding.UTF8.GetBytes(passwordBox.Password);
					byte[] hash = sha256.ComputeHash(bytes);

					StringBuilder result = new StringBuilder();
					for (int i = 0; i < hash.Length; i++)
					{
						result.Append(hash[i].ToString("X2"));
					}

					//	Register player into database
					pro.AddPlayer(textBoxUsername.Text, result.ToString());

					//	Popup successful registration dialog
					customMessageBox.ShowText("Registration succeeded!");

					//	Close after successful registration
					Close();
				}
				else
				{
					//	Feedback to user that the entered username is not available
					customMessageBox.ShowText("Username already taken! Try another");
				}
			}
			else
			{
				//	Feedback to user that the passwords do not match
				customMessageBox.ShowText("Password do not match! Try again");
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
			else if (passwordBox1.IsFocused)
			{
				TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, passwordBox1, keyPressed.ToString()));
			}
		}
	}
}
