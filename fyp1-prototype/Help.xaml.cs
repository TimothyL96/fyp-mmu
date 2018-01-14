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
				Content = "This is a freakin guide\nThis is a line space\nThis is the complete guide!!!",
				FontWeight = FontWeights.Bold,
				FontSize = 26
			};
			scrollContent.Children.Add(textHeader);
		}
	}
}
