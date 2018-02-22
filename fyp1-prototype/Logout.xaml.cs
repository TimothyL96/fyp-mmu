using Microsoft.Kinect.Toolkit;
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

namespace fyp1_prototype
{
	/// <summary>
	/// Interaction logic for Logout.xaml
	/// </summary>
	public partial class Logout : Window
	{
		private int playerID;
		KinectSensorChooser kinectSensorChooser;
		private bool loggedOut = false;

		public Logout(KinectSensorChooser kinectSensorChooser, int playerID)
		{
			InitializeComponent();

			this.playerID = playerID;
			this.kinectSensorChooser = kinectSensorChooser;
		}

		public bool CustomShowDialog()
		{
			ShowDialog();
			return loggedOut;
		}

		private void back(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void logout(object sender, RoutedEventArgs e)
		{
			loggedOut = true;
		}
	}
}
