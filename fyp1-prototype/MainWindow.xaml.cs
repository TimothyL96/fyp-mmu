using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private KinectSensorChooser kinectSensorChooser;
		private HandPointer capturedHandPointer;
		private int playerID = -1;
		private int playerScore = -1;

		public MainWindow()
		{
			InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            kinectSensorChooser = new KinectSensorChooser();
			kinectSensorChooser.KinectChanged += KinectSensorChooser_KinectChanged;
			kinectSensorDisplay.KinectSensorChooser = kinectSensorChooser;

			kinectSensorChooser.Start();
			
			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			#region KinectRegion
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
			#endregion
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
		{
			if (capturedHandPointer == null && e.HandPointer.IsPrimaryUser && e.HandPointer.IsPrimaryHandOfUser)
			{
				if (e.HandPointer.GetIsOver(btn_login))
				{
					e.HandPointer.Capture(btn_login);
				}
				else if (e.HandPointer.GetIsOver(btn_register))
				{
					e.HandPointer.Capture(btn_register);
				}
				else if (e.HandPointer.GetIsOver(btn_singlePlayer))
				{
					e.HandPointer.Capture(btn_singlePlayer);
				}
				else if (e.HandPointer.GetIsOver(btn_multiPlayer))
				{
					e.HandPointer.Capture(btn_multiPlayer);
				}
				else if (e.HandPointer.GetIsOver(btn_loadGame))
				{
					e.HandPointer.Capture(btn_loadGame);
				}
				else if (e.HandPointer.GetIsOver(btn_highScores))
				{
					e.HandPointer.Capture(btn_highScores);
				}
				else if (e.HandPointer.GetIsOver(btn_help))
				{
					e.HandPointer.Capture(btn_help);
				}
				else if (e.HandPointer.GetIsOver(btn_exit))
				{
					e.HandPointer.Capture(btn_exit);
				}
				e.Handled = true;
			}
		}

        //  Execute press function
		private void OnHandPointerPressRelease(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				e.HandPointer.Capture(null);
				if (e.HandPointer.GetIsOver(btn_login))
				{
					VisualStateManager.GoToState(btn_login, "MouseOver", true);

					if (playerID == -1)
					{
						//	No user logged in, proceed to show login screen
						Login login = new Login(kinectSensorChooser);
						playerID = login.CustomShowDialog();
					}
					else
					{
						//	A user is logged in, prompt current user to logout before a new login
						Logout logout = new Logout(kinectSensorChooser, playerID);

						if (logout.CustomShowDialog())
						{
							PersonalBestValue.Visibility = Visibility.Hidden;
							PersonalBestText.Visibility = Visibility.Hidden;

							//	Reset player ID
							playerID = -1;

							//	User logged out, proceed to show login screen
							Login login = new Login(kinectSensorChooser);
							playerID = login.CustomShowDialog();
						}
					}

					//	If logged in, get highest score
					if (playerID != -1)
					{
						PlayersRepository playersRepository = new PlayersRepository();
						playerScore = playersRepository.GetPlayerScore(playerID);
						PersonalBestValue.Text = playerScore.ToString();

						PersonalBestValue.Visibility = Visibility.Visible;
						PersonalBestText.Visibility = Visibility.Visible;
					}
					else
					{
						PersonalBestValue.Visibility = Visibility.Hidden;
						PersonalBestText.Visibility = Visibility.Hidden;
					}
				}
				else if (e.HandPointer.GetIsOver(btn_register))
				{
					VisualStateManager.GoToState(btn_register, "MouseOver", true);

					if (playerID == -1)
					{
						//	No user logged in, proceed to show register screen
						Register register = new Register(kinectSensorChooser);
						register.ShowDialog();
					}
					else
					{
						//	A user is logged in, prompt current user to logout before a new registration
						Logout logout = new Logout(kinectSensorChooser, playerID);

						if (logout.CustomShowDialog())
						{
							//	Reset player ID
							playerID = -1;

							PersonalBestValue.Visibility = Visibility.Hidden;
							PersonalBestText.Visibility = Visibility.Hidden;

							//	User logged out, proceed to show register screen
							Register register = new Register(kinectSensorChooser);
							register.ShowDialog();
						}
					}
				}
				else if (e.HandPointer.GetIsOver(btn_singlePlayer))
				{
					VisualStateManager.GoToState(btn_singlePlayer, "MouseOver", true);

					if (playerID == -1)
					{
						CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
						customMessageBox.ShowText("Please login to continue");
					}
					else
					{
						GameRepository gameRepository = new GameRepository();
						if (gameRepository.GetGame(playerID).Count > 0)
						{
							LoadGame loadGame = new LoadGame(kinectSensorChooser, playerID);
							loadGame.ShowDialog();
						}
						else
						{
							GameMode gameMode = new GameMode(kinectSensorChooser, playerID);
							gameMode.ShowDialog();
						}
					}					
				}
				else if (e.HandPointer.GetIsOver(btn_multiPlayer))
				{
					VisualStateManager.GoToState(btn_multiPlayer, "MouseOver", true);

					//	Multiplayer doesnt need login, player ID = -1
					GameMode gameMode = new GameMode(kinectSensorChooser, -1);
					gameMode.ShowDialog();
				}
				else if (e.HandPointer.GetIsOver(btn_loadGame))
				{
					VisualStateManager.GoToState(btn_loadGame, "MouseOver", true);

					if (playerID == -1)
					{
						CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
						customMessageBox.ShowText("Please login to continue");
					}
					else
					{
						GameRepository gameRepository = new GameRepository();
						List<GameRepository.GameDto> getGame = gameRepository.GetGame(playerID);
						if (getGame.Count == 0)
						{
							//	No game saved, display the dialog
							CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
							customMessageBox.ShowText("You have no saved game!");
						}
						else
						{
							//	Load the game
							kinectSensorChooser.Stop();
							DragDropImages dragDropImages = new DragDropImages(playerID, getGame[0].GameMode);
							dragDropImages.GetLoadGameData(getGame[0].Lives, getGame[0].Time, getGame[0].Score, getGame[0].ItemGame);
							dragDropImages.ShowDialog();

							kinectSensorChooser.Start();
						}
					}
				}
				else if (e.HandPointer.GetIsOver(btn_highScores))
				{
					VisualStateManager.GoToState(btn_highScores, "MouseOver", true);

					GameMode gameMode = new GameMode(kinectSensorChooser, playerID, 0);
					gameMode.ShowDialog();
				}
				else if (e.HandPointer.GetIsOver(btn_help))
				{
					VisualStateManager.GoToState(btn_help, "MouseOver", true);

					Help help = new Help(kinectSensorChooser);
					help.ShowDialog();
				}
				else if (e.HandPointer.GetIsOver(btn_exit))
				{
					VisualStateManager.GoToState(btn_exit, "MouseOver", true);

					Close();
				}
				else
				{
					VisualStateManager.GoToState(btn_login, "Normal", true);
					VisualStateManager.GoToState(btn_register, "Normal", true);
					VisualStateManager.GoToState(btn_singlePlayer, "Normal", true);
					VisualStateManager.GoToState(btn_multiPlayer, "Normal", true);
					VisualStateManager.GoToState(btn_loadGame, "Normal", true);
					VisualStateManager.GoToState(btn_highScores, "Normal", true);
					VisualStateManager.GoToState(btn_help, "Normal", true);
					VisualStateManager.GoToState(btn_exit, "Normal", true);
				}
				e.Handled = true;
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			kinectSensorChooser.Stop();

            Application.Current.Shutdown();
		}

        //	Below are all temporary click functions
        private void close(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void highscore(object sender, RoutedEventArgs e)
		{
			GameMode gameMode = new GameMode(kinectSensorChooser, playerID, 0);
			gameMode.ShowDialog();
		}

		private void singlePlayer(object sender, RoutedEventArgs e)
		{
			if (playerID == -1)
			{
				CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
				customMessageBox.ShowText("Please login to continue");
			}
			else
			{
				GameRepository gameRepository = new GameRepository();
				PlayersRepository playersRepository = new PlayersRepository();

				if (gameRepository.GetGame(playerID).Count > 0)
				{
					LoadGame loadGame = new LoadGame(kinectSensorChooser, playerID);
					loadGame.ShowDialog();
				}
				else
				{
					GameMode gameMode = new GameMode(kinectSensorChooser, playerID);
					gameMode.ShowDialog();
				}
				PersonalBestValue.Text = playersRepository.GetPlayerScore(playerID).ToString();
			}
		}

		private void help(object sender, RoutedEventArgs e)
		{
			Help help = new Help(kinectSensorChooser);
			help.ShowDialog();
		}

        private void login(object sender, RoutedEventArgs e)
        {
			if (playerID == -1)
			{
				//	No user logged in, proceed to show login screen
				Login login = new Login(kinectSensorChooser);
				playerID = login.CustomShowDialog();
			}
			else
			{
				//	A user is logged in, prompt current user to logout before a new login
				Logout logout = new Logout(kinectSensorChooser, playerID);

				if (logout.CustomShowDialog())
				{
					//	Reset player ID
					playerID = -1;

					//	User logged out, proceed to show login screen
					Login login = new Login(kinectSensorChooser);
					playerID = login.CustomShowDialog();

					PersonalBestValue.Visibility = Visibility.Hidden;
					PersonalBestText.Visibility = Visibility.Hidden;
				}
			}

			//	If logged in, get highest score
			if (playerID != -1)
			{
				PlayersRepository playersRepository = new PlayersRepository();
				playerScore = playersRepository.GetPlayerScore(playerID);
				PersonalBestValue.Text = playerScore.ToString();

				PersonalBestValue.Visibility = Visibility.Visible;
				PersonalBestText.Visibility = Visibility.Visible;
			}
			else
			{
				PersonalBestValue.Visibility = Visibility.Hidden;
				PersonalBestText.Visibility = Visibility.Hidden;
			}
		}

		private void register(object sender, RoutedEventArgs e)
		{
			if (playerID == -1)
			{
				//	No user logged in, proceed to show register screen
				Register register = new Register(kinectSensorChooser);
				register.ShowDialog();
			}
			else
			{
				//	A user is logged in, prompt current user to logout before a new registration
				Logout logout = new Logout(kinectSensorChooser, playerID);

				if (logout.CustomShowDialog())
				{
					//	Reset player ID
					playerID = -1;

					PersonalBestValue.Visibility = Visibility.Hidden;
					PersonalBestText.Visibility = Visibility.Hidden;

					//	User logged out, proceed to show register screen
					Register register = new Register(kinectSensorChooser);
					register.ShowDialog();
				}
			}
		}

		private void loadGame(object sender, RoutedEventArgs e)
		{
			if (playerID == -1)
			{
				CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
				customMessageBox.ShowText("Please login to continue");
			}
			else
			{
				GameRepository gameRepository = new GameRepository();
				List<GameRepository.GameDto> getGame = gameRepository.GetGame(playerID);
				if (getGame.Count == 0)
				{
					//	No game saved, display the dialog
					CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
					customMessageBox.ShowText("You have no saved game!");
				}
				else
				{
					//	Load the game
					kinectSensorChooser.Stop();
					DragDropImages dragDropImages = new DragDropImages(playerID, getGame[0].GameMode);
					dragDropImages.GetLoadGameData(getGame[0].Lives, getGame[0].Time, getGame[0].Score, getGame[0].ItemGame);
					dragDropImages.ShowDialog();

					kinectSensorChooser.Start();
				}
			}
		}

		private void multiPlayer(object sender, RoutedEventArgs e)
		{
			//	Multiplayer doesnt need login, player ID = -1
			GameMode gameMode = new GameMode(kinectSensorChooser, -1);
			gameMode.ShowDialog();
		}
	}
}