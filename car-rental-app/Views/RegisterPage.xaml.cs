using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using MySqlConnector;
using BCrypt.Net;
using Microsoft.WindowsAppSDK.Runtime.Packages;
using System.Threading.Tasks;

namespace car_rental_app.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private async Task<Boolean> AddUser()
        {
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Password;

            // Hash password
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            // Hash the password using bcrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=car_rental_app;Uid=root;Pwd=;";
                using MySqlConnection connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "INSERT INTO User (email, password) VALUES (@Email, @Password)";
                using MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", hashedPassword);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("User added successfully!");
                    return true;
                }
                else
                {
                    // Failed to register
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private async void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text.Length == 0)
            {
                // Add status text
                return;
            }
            else if (PasswordTextBox.Password.Length == 0)
            {
                return;
            }
            else if (RepeatPasswordTextBox.Password.Length == 0)
            {
                return;
            }
            else if (PasswordTextBox.Password != RepeatPasswordTextBox.Password)
            {
                return;
            }

            if (await AddUser())
            {
                Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                // Register failed
            }
        }
    }
}
