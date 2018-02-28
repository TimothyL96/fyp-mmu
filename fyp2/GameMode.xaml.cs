using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Windows;
using System.Windows.Data;

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for GameMode.xaml
	/// </summary>
	public partial class GameMode : Window
	{
		private HandPointer capturedHandPointer;
		KinectSensorChooser kinectSensorChooser;
		private int playerID;
		private int windowType;
		private int player1Score = -1;
		private int player2Score = -1;

		public GameMode(KinectSensorChooser kinectSensorChooser, int playerID, int window = -1)
		{
			InitializeComponent();

			//	Set window startup location to center
			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			//	Set the class members
			this.playerID = playerID;
			this.kinectSensorChooser = kinectSensorChooser;

			// window -1 for game, 0 for high score
			windowType = window;

			// Bind Kinect Sensor to Kinect Region
			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			#region KinectRegion
			//	Setup Kinect region press target and event handlers
			KinectRegion.SetIsPressTarget(btnSurvival, true);
			KinectRegion.SetIsPressTarget(btnTimeAttack, true);

			//	btnSurvival
			KinectRegion.AddHandPointerEnterHandler(btnSurvival, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnSurvival, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnSurvival, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnSurvival, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnSurvival, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnSurvival, HandPointerLostCaptureEvent);

			//	btnTimeAttack
			KinectRegion.AddHandPointerEnterHandler(btnTimeAttack, HandPointerEnterEvent);
			KinectRegion.AddHandPointerLeaveHandler(btnTimeAttack, HandPointerLeaveEvent);

			KinectRegion.AddHandPointerPressHandler(btnTimeAttack, HandPointerPressEvent);
			KinectRegion.AddHandPointerPressReleaseHandler(btnTimeAttack, HandPointerPressReleaseEvent);

			KinectRegion.AddHandPointerGotCaptureHandler(btnTimeAttack, HandPointerCaptureEvent);
			KinectRegion.AddHandPointerLostCaptureHandler(btnTimeAttack, HandPointerLostCaptureEvent);
			#endregion
		}

		private void HandPointerEnterEvent(object sender, HandPointerEventArgs e)
		{
			if (e.HandPointer.GetIsOver(btnSurvival) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnSurvival, "MouseOver", true);
			}
			else if (e.HandPointer.GetIsOver(btnTimeAttack) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnTimeAttack, "MouseOver", true);
			}

			e.Handled = true;
		}

		private void HandPointerLeaveEvent(object sender, HandPointerEventArgs e)
		{
			if (!e.HandPointer.GetIsOver(btnSurvival) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnSurvival, "Normal", true);
			}
			if (!e.HandPointer.GetIsOver(btnTimeAttack) && e.HandPointer.IsPrimaryHandOfUser)
			{
				VisualStateManager.GoToState(btnTimeAttack, "Normal", true);
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
				if (e.HandPointer.GetIsOver(btnSurvival))
				{
					e.HandPointer.Capture(btnSurvival);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
				else if (e.HandPointer.GetIsOver(btnTimeAttack))
				{
					e.HandPointer.Capture(btnTimeAttack);
					capturedHandPointer = e.HandPointer;
					e.Handled = true;
				}
			}
		}

		//	Execute button functions
		private void HandPointerPressReleaseEvent(object sender, HandPointerEventArgs e)
		{
			if (capturedHandPointer == e.HandPointer)
			{
				if (windowType == -1)
				{
					if (playerID != -1)
					{
						//	Single player game
						//	Show game play window
						kinectSensorChooser.Stop();
						DragDropImages dragDropImages = null;

						if (e.HandPointer.GetIsOver(btnSurvival))
						{
							//	Survival
							dragDropImages = new DragDropImages(playerID, 0);

							VisualStateManager.GoToState(btnSurvival, "MouseOver", true);
						}
						else if (e.HandPointer.GetIsOver(btnTimeAttack))
						{
							//	Time Attack
							dragDropImages = new DragDropImages(playerID, 1);

							VisualStateManager.GoToState(btnTimeAttack, "MouseOver", true);
						}
						else
						{
							VisualStateManager.GoToState(btnSurvival, "Normal", true);
							VisualStateManager.GoToState(btnTimeAttack, "Normal", true);
						}

						if (dragDropImages != null)
						{
							e.HandPointer.Capture(null);
							e.Handled = true;

							Close();

							dragDropImages.ShowDialog();

							kinectSensorChooser.Start();

							return;
						}
					}
					else
					{
						//	Multiplayer game, log in not required, playerID == -1
						Multiplayer multiplayer = null;
						Multiplayer multiplayer1 = null;
						kinectSensorChooser.Stop();

						if (e.HandPointer.GetIsOver(btnSurvival))
						{
							//	Survival
							multiplayer = new Multiplayer(0);
							multiplayer1 = new Multiplayer(0);

							VisualStateManager.GoToState(btnSurvival, "MouseOver", true);
						}
						else if (e.HandPointer.GetIsOver(btnTimeAttack))
						{
							//	Time Attack
							multiplayer = new Multiplayer(1);
							multiplayer1 = new Multiplayer(1);

							VisualStateManager.GoToState(btnTimeAttack, "MouseOver", true);
						}
						else
						{
							VisualStateManager.GoToState(btnSurvival, "Normal", true);
							VisualStateManager.GoToState(btnTimeAttack, "Normal", true);
						}

						if (multiplayer != null && multiplayer1 != null)
						{
							e.HandPointer.Capture(null);
							e.Handled = true;

							Close();

							//	Start the game for player 1 then player 2
							player1Score = multiplayer.StartGame();
							player2Score = multiplayer1.StartGame();

							kinectSensorChooser.Start();

							//	Find the winner
							CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
							if (player1Score > player2Score)
							{
								customMessageBox.ShowText("Winner is Player #1\nScore: " + player1Score, "Game Ended");
							}
							else if (player2Score > player1Score)
							{
								customMessageBox.ShowText("Winner is Player #2\nScore: " + player2Score, "Game Ended");
							}
							else
							{
								//	If draw
								customMessageBox.ShowText("DRAW!\nScore: " + player1Score, "Game Ended");
							}

							return;
						}
					}					
				}
				else
				{
					//	Show high score
					if (e.HandPointer.GetIsOver(btnSurvival))
					{
						//	Survival
						HighScore highScore = new HighScore(kinectSensorChooser, 0);
						highScore.ShowDialog();

						VisualStateManager.GoToState(btnSurvival, "MouseOver", true);
					}
					else if (e.HandPointer.GetIsOver(btnTimeAttack))
					{
						//	Time Attack
						HighScore highScore = new HighScore(kinectSensorChooser, 1);
						highScore.ShowDialog();

						VisualStateManager.GoToState(btnTimeAttack, "MouseOver", true);
					}
					else
					{
						VisualStateManager.GoToState(btnSurvival, "Normal", true);
						VisualStateManager.GoToState(btnTimeAttack, "Normal", true);
					}
				}

				e.HandPointer.Capture(null);
				e.Handled = true;

				Close();
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

		//	Disable maximizing window
		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Normal)
			{
				WindowState = WindowState.Maximized;
			}
		}

		private void survival(object sender, RoutedEventArgs e)
		{
			if (windowType == -1)
			{
				if (playerID != -1)
				{
					//	Survival
					kinectSensorChooser.Stop();
					DragDropImages dragDropImages = new DragDropImages(playerID, 0);

					Close();

					dragDropImages.ShowDialog();

					kinectSensorChooser.Start();
				}
				else
				{
					//	Multiplayer
					//	Multiplayer game, log in not required, playerID == -1
					Multiplayer multiplayer = null;
					Multiplayer multiplayer1 = null;
					kinectSensorChooser.Stop();

					VisualStateManager.GoToState(btnSurvival, "MouseOver", true);

					if (true)
					{
						Close();

						//	Start the game for player 1 then player 2
						//	Survival
						multiplayer = new Multiplayer(0);
						player1Score = multiplayer.StartGame();
						multiplayer1 = new Multiplayer(0);
						player2Score = multiplayer1.StartGame();

						kinectSensorChooser.Start();

						//	Find the winner
						CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
						if (player1Score > player2Score)
						{
							customMessageBox.ShowText("Winner is Player #1\nScore: " + player1Score, "Game Ended");
						}
						else if (player2Score > player1Score)
						{
							customMessageBox.ShowText("Winner is Player #2\nScore: " + player2Score, "Game Ended");
						}
						else
						{
							//	If draw
							customMessageBox.ShowText("DRAW!\nScore: " + player1Score, "Game Ended");
						}
					}
				}
				
			}
			else
			{
				//	Survival
				HighScore highScore = new HighScore(kinectSensorChooser, 0);
				highScore.Show();

				Close();
			}
		}

		private void timeAttack(object sender, RoutedEventArgs e)
		{
			if (windowType == -1)
			{
				if (playerID != -1)
				{
					//	Time Attack
					kinectSensorChooser.Stop();
					DragDropImages dragDropImages = new DragDropImages(playerID, 1);

					Close();

					dragDropImages.ShowDialog();

					kinectSensorChooser.Start();
				}
				else
				{
					//	Multiplayer
					//	Multiplayer game, log in not required, playerID == -1
					Multiplayer multiplayer = null;
					kinectSensorChooser.Stop();

					//	Time Attack
					multiplayer = new Multiplayer(1);

					VisualStateManager.GoToState(btnTimeAttack, "MouseOver", true);

					if (multiplayer != null)
					{
						Close();

						//	Start the game for player 1 then player 2
						player1Score = multiplayer.StartGame();
						multiplayer = new Multiplayer(1);
						player2Score = multiplayer.StartGame();

						kinectSensorChooser.Start();

						//	Find the winner
						CustomMessageBox customMessageBox = new CustomMessageBox(kinectSensorChooser);
						if (player1Score > player2Score)
						{
							customMessageBox.ShowText("Winner is Player #1\nScore: " + player1Score, "Game Ended");
						}
						else if (player2Score > player1Score)
						{
							customMessageBox.ShowText("Winner is Player #2\nScore: " + player2Score, "Game Ended");
						}
						else
						{
							//	If draw
							customMessageBox.ShowText("DRAW!\nScore: " + player1Score, "Game Ended");
						}

						return;
					}
				}
			}
			else
			{
				//	Time attack
				HighScore highScore = new HighScore(kinectSensorChooser, 1);
				highScore.Show();

				Close();
			}
		}
	}
}
