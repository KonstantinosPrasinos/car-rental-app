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
using System.Text.RegularExpressions;
using System.Data;

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

                string query = "SELECT Id, Password, IsAdmin, CreditCardNumber, CreditCardExpirationDate, CreditCardCVV FROM User WHERE Email = @Email";
                using MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Email", email);

                using MySqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32("Id");
                    string hashedPassword = reader.GetString("Password");
                    Boolean isAdmin = reader.GetBoolean("IsAdmin");

                    string cardNumber = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("CreditCardNumber")))
                    {
                        cardNumber = reader.GetString("CreditCardNumber");
                    }

                    string cardExpirationDate = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("CreditCardExpirationDate")))
                    {
                        cardExpirationDate = reader.GetString("CreditCardExpirationDate");
                    }

                    string cardCVV = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("CreditCardCVV")))
                    {
                        cardCVV = reader.GetString("CreditCardCVV");
                    }

                    // Compare user password with hashed password
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

                    if (isPasswordValid)
                    {
                        // Store user
                        Data.User user = Data.User.Instance;
                        user.Email = email;
                        user.Id = id;
                        user.IsAdmin = isAdmin;
                        
                        if (cardNumber != "" && cardExpirationDate != "" && cardCVV != "" )
                        {
                            user.CardNumber = cardNumber;
                            user.CardExpirationDate = cardExpirationDate;
                            user.CardCVV = cardCVV;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private async void StartLogin()
        {
            if (!IsLoading)
            {
                if (PasswordTextBox.Password.Length == 0 && EmailTextBox.Text.Length == 0)
                {
                    IncorrectInputTextBlock.Text = "The email and password fields are required";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                else if (PasswordTextBox.Password.Length == 0)
                {
                    IncorrectInputTextBlock.Text = "The password field is required";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                else if (EmailTextBox.Text.Length == 0)
                {
                    IncorrectInputTextBlock.Text = "The email field is required";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                else if (!Regex.IsMatch(EmailTextBox.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    IncorrectInputTextBlock.Text = "The email field must be an email";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                LoginTextBlock.Visibility = Visibility.Collapsed;
                LoadingRing.Visibility = Visibility.Visible;
                IsLoading = true;

                if (await LoginUser())
                {
                    Frame.Navigate(typeof(ViewCarsPage));
                }
                else
                {
                    // Handle fail to log in
                    IncorrectInputTextBlock.Text = "Incorrect email or password";
                    IncorrectInputTextBlock.Visibility = Visibility.Visible;
                }

                LoginTextBlock.Visibility = Visibility.Visible;
                LoadingRing.Visibility = Visibility.Collapsed;
                IsLoading = false;
            }
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            StartLogin();
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

                StartLogin();
            }
            else
            {
                IncorrectInputTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }
    }
}
