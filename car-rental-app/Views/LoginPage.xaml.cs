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
using car_rental_app.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
using MySqlConnector;
using Windows.UI.Core;
using Windows.System;
using System.Threading.Tasks;

namespace car_rental_app.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private Boolean IsLoading = false;
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async Task<Boolean> LoginUser()
        {
            // Add user validation
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Password;

            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=car_rental_app;Uid=root;Pwd=;";

                using MySqlConnection connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "SELECT Id, Password, IsAdmin FROM User WHERE Email = @Email";
                using MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Email", email);

                using MySqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32("Id");
                    string hashedPassword = reader.GetString("Password");
                    Boolean isAdmin = reader.GetBoolean("IsAdmin");

                    // Compare user password with hashed password
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

                    if (isPasswordValid)
                    {
                        // Store user
                        Data.User user = Data.User.Instance;
                        user.Email = email;
                        user.Id = id;
                        user.IsAdmin = isAdmin;

                        // Navigate to ViewCarsPage
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                tempTextBlock.Text = ex.ToString();
            }

            return false;
        }

        private async void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            ProgressRing progressRing = new ProgressRing();
            progressRing.IsIndeterminate = true;
            progressRing.IsActive = true;
            progressRing.Height = 20;
            progressRing.Width = 20;

            LoginButton.Content = progressRing;
            IsLoading = true;

            if (await LoginUser())
            {
                Frame.Navigate(typeof(ViewCarsPage));
            }
            else
            {
                // Handle fail to log in
            }

            LoginButton.Content = "Log in";
            IsLoading = false;
        }

        private async void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true; // Prevent the "Enter" key from adding a new line

                if (await LoginUser())
                {
                    Frame.Navigate(typeof(ViewCarsPage));
                }
                else
                {
                    // Handle fail to log in
                }
            }
        }

        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }
    }
}
