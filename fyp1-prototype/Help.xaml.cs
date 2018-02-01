using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
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
	/// Interaction logic for Help.xaml
	/// </summary>
	public partial class Help : Window
	{
		private KinectSensorChooser kinectSensorChooser;

		public Help(KinectSensorChooser kinectSensorChooser)
		{
			InitializeComponent();

			this.kinectSensorChooser = kinectSensorChooser;

			var kinectRegionandSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser };
			BindingOperations.SetBinding(kinectKinectRegion, KinectRegion.KinectSensorProperty, kinectRegionandSensorBinding);

			var textHeader = new Label
			{
				Content = "Help",
				FontWeight = FontWeights.Bold,
				FontSize = 68
			};
			scrollContent.Children.Add(textHeader);

			var textContent = new Label
			{
				Content = "This section will give you a guide to this game.\nTo play this game, you need to have Kinect for Windows.\nFind an area where the Kinect sensor can detect you well and clear.\n",
				FontSize = 22
			};
			scrollContent.Children.Add(textContent);

			var textCredits = new Label
			{
				Content = "Credits:\nfreepik",
				FontSize = 22
			};
			scrollContent.Children.Add(textCredits);
		}
	}
}
