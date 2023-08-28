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
using Windows.System;
using Microsoft.WindowsAppSDK.Runtime.Packages;
using MySqlConnector;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace car_rental_app.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReservationPage : Page
    {
        private DateTimeOffset fromDate;
        private DateTimeOffset toDate;
        private int carId;
        private int differenceInDays;
        private double pricePerDay;
        private string fromDateString;
        private string toDateString;
        private Car car;
        private string CardExpirationMonth;
        private string CardExpirationYear;
        public ReservationPage()
        {
            this.InitializeComponent();

            if (Data.User.Instance.CardExpirationDate != null)
            {
                string[] values = Data.User.Instance.CardExpirationDate.Split("/");
                CardExpirationMonth = values[0];
                CardExpirationYear = values[1];
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                // Split navigation parameters string to a list
                string navigationParameter = e.Parameter as string;

                string[] values = navigationParameter.Split('-');

                // Parse all the required fields from the list
                int.TryParse(values[0], out carId);
                DateTimeOffset.TryParse(values[1], out fromDate);
                DateTimeOffset.TryParse(values[2], out toDate);

                // Process the required 
                fromDateString = fromDate.ToString("dd/MM/yyyy");
                toDateString = toDate.ToString("dd/MM/yyyy");

                car = Car.Instance.First(car => car.Id == carId);

                TimeSpan dayDifferenceTimeSpan = toDate - fromDate;
                differenceInDays = dayDifferenceTimeSpan.Days + 1;
            }
        }

        private async void Finallize_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CardNumberNumberBox.Text.Length == 12 && (CardExpirationMonthTextBox.Text.Length == 2 || CardExpirationMonthTextBox.Text.Length == 1) && CardExpirationYearTextBox.Text.Length == 2 && CardSecurityCodeTextBox.Text.Length == 3)
            {
                try
                {
                    string connectionString = "Server=localhost;Port=3306;Database=car_rental_app;Uid=root;Pwd=;";
                    using MySqlConnection connection = new MySqlConnection(connectionString);
                    await connection.OpenAsync();

                    string query = "INSERT INTO Reservation (UserId, CarId, FromDate, ToDate) VALUES (@UserId, @CarId, @FromDate, @ToDate)";
                    using MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@UserId", 2);
                    command.Parameters.AddWithValue("@CarId", carId);
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        string selectQuery = "SELECT Id FROM Reservation WHERE FromDate = @FromDate AND ToDate = @ToDate AND CarId = @CarId";
                        using MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);

                        selectCommand.Parameters.AddWithValue("@FromDate", fromDate);
                        selectCommand.Parameters.AddWithValue("@ToDate", toDate);
                        selectCommand.Parameters.AddWithValue("@CarId", carId);

                        using MySqlDataReader reader = await selectCommand.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32("Id");
                            Reservation.Instance.Add(new Reservation(id, carId, fromDate, toDate, car.Name));
                        }
                        Frame.Navigate(typeof(ViewCarsPage));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ViewCarsPage));
        }

        private void CardNumberNumberBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            foreach (char c in args.NewText)
            {
                if (!char.IsDigit(c))
                {
                    args.Cancel = true;
                    return;
                }
            }

            if (args.NewText.Length > 12)
            {
                CardExpirationMonthTextBox.Focus(FocusState.Programmatic);
                args.Cancel = true; // Cancel the text change if it exceeds the maximum length
                return;
            }
        }

        private void CardExpirationMonthTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            foreach (char c in args.NewText)
            {
                if (!char.IsDigit(c))
                {
                    args.Cancel = true;
                    return;
                }
            }

            if (args.NewText.Length > 2)
            {
                CardExpirationYearTextBox.Focus(FocusState.Programmatic);
                args.Cancel = true; // Cancel the text change if it exceeds the maximum length
                return;
            }

            if (int.Parse(args.NewText) < 0 || int.Parse(args.NewText) > 12)
            {
                args.Cancel = true;
                return;
            }
        }

        private void CardExpirationYearTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            foreach (char c in args.NewText)
            {
                if (!char.IsDigit(c))
                {
                    args.Cancel = true;
                    return;
                }
            }

            if (args.NewText.Length > 2)
            {
                CardSecurityCodeTextBox.Focus(FocusState.Programmatic);
                args.Cancel = true; // Cancel the text change if it exceeds the maximum length
                return;
            }
        }

        private void CardSecurityCodeTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            foreach (char c in args.NewText)
            {
                if (!char.IsDigit(c))
                {
                    args.Cancel = true;
                    return;
                }
            }

            if (args.NewText.Length > 3)
            {
                args.Cancel = true; // Cancel the text change if it exceeds the maximum length
                return;
            }
        }
    }

}
