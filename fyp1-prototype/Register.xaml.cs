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

namespace fyp1_prototype
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();

			WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}

		//	Temporary click functions
		private void goback(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void register(object sender, RoutedEventArgs e)
		{
			if (passwordBox.Password == passwordBox1.Password)
			{
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
					MessageBox.Show("Registration succeeded!");

					//	Close after successful registration
					Close();
				}
			}
		}
	}
}
