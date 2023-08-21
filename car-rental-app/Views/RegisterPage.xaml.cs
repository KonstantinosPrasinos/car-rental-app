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
using Windows.System;
using System.Text.RegularExpressions;

namespace car_rental_app.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        private Boolean IsLoading = false;
        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private async Task<Boolean> RegisterUser()
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

        private async void StartRegister()
        {
            if (!IsLoading)
            {
                if (PasswordTextBox.Password.Length == 0)
                {
                    IncorrectInputTextBlock.Text = "All fields are required";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                else if (EmailTextBox.Text.Length == 0)
                {
                    IncorrectInputTextBlock.Text = "All fields are required";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                if (RepeatPasswordTextBox.Password.Length == 0)
                {
                    IncorrectInputTextBlock.Text = "All fields are required";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                else if (PasswordTextBox.Password != RepeatPasswordTextBox.Password)
                {
                    IncorrectInputTextBlock.Text = "Password and repeat password must match";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                else if (!Regex.IsMatch(EmailTextBox.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    IncorrectInputTextBlock.Text = "The email field must be an email";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                RegisterTextBlock.Visibility = Visibility.Collapsed;
                LoadingRing.Visibility = Visibility.Visible;
                IsLoading = true;

                if (await RegisterUser())
                {
                    Frame.Navigate(typeof(LoginPage));
                }
                else
                {
                    // Handle fail to log in
                    IncorrectInputTextBlock.Text = "Incorrect email or password";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                }

                RegisterTextBlock.Visibility = Visibility.Visible;
                LoadingRing.Visibility = Visibility.Collapsed;
                IsLoading = false;
            }
        }

        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            StartRegister();
        }

        private void EmailTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true; // Prevent the "Enter" key from adding a new line

                PasswordTextBox.Focus(FocusState.Programmatic);
            }
            else
            {
                IncorrectInputTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true; // Prevent the "Enter" key from adding a new line

                RepeatPasswordTextBox.Focus(FocusState.Programmatic);
            }
            else
            {
                IncorrectInputTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void RepeatPasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;

                StartRegister();
            }
            else
            {
                IncorrectInputTextBlock.Visibility = Visibility.Collapsed;
            }
        }
    }
}
