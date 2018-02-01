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

		private void login(object sender, RoutedEventArgs e)
		{
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
					MessageBox.Show("Log in succeeded");

					//	Close after succesfully logged in
					Close();
				}
				else
				{
					//	Show dialog that the login is not successful
					MessageBox.Show("Password is incorrect!");

					//	Clear the incorrect password
					passwordBox.Password = "";
				}
			}
			else
			{
				//	Show dialog that the login is not successful
				MessageBox.Show("Player doesn't exist!");
			}
		}
	}
}
