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
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
			
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

		private void goback()
		{

		}

		//	Temporary click function
		private void goback(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
