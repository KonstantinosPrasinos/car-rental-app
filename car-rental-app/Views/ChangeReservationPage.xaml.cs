using car_rental_app.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace car_rental_app.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangeReservationPage : Page
    {

        private Reservation reservation;
        private string fromDateString;
        private string toDateString;
        public ChangeReservationPage()
        {
            this.InitializeComponent();

            FromCalendarDatePicker.MinDate = DateTime.Now.AddDays(1);
            ToCalendarDatePicker.MinDate = DateTime.Now.AddDays(1);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                // Split navigation parameters string to a list
                int navigationParameter = int.Parse(e.Parameter as string);

                reservation = Reservation.Instance.FirstOrDefault(tempReservation => tempReservation.Id == navigationParameter);
                fromDateString = reservation.FromDate.ToString("dd/MM/yyyy");
                toDateString = reservation.ToDate.ToString("dd/MM/yyyy");
            }
        }

        private void FromCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            ToCalendarDatePicker.IsEnabled = true;
            ToCalendarDatePicker.Date = null;
            ToCalendarDatePicker.MinDate = (DateTimeOffset)FromCalendarDatePicker.Date;
            ToCalendarDatePicker.IsCalendarOpen = true;
            ChangeDatesButton.IsEnabled = false;

            IncorrectInputTextBlock.Visibility = Visibility.Collapsed;
        }

        private void ToCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            ChangeDatesButton.IsEnabled = true;

            IncorrectInputTextBlock.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ViewCarsPage));
        }

        private async Task<Car> GetCars()
        {

            DateTimeOffset fromDate = (DateTimeOffset)FromCalendarDatePicker.Date;
            DateTimeOffset toDate = (DateTimeOffset)ToCalendarDatePicker.Date;

            TimeSpan dayDifferenceTimeSpan = toDate - fromDate;
            int differenceInDays = dayDifferenceTimeSpan.Days + 1;

            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=car_rental_app;Uid=root;Pwd=;";

                using MySqlConnection connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "SELECT Distinct Car.*FROM Car LEFT JOIN Reservation ON Car.Id = Reservation.CarId WHERE Reservation.Id = @ReservationId OR (Reservation.FromDate IS NULL OR Reservation.FromDate > @ToDate OR Reservation.ToDate< @FromDate) OR (Reservation.FromDate IS NULL OR Reservation.FromDate > @ToDate OR Reservation.ToDate IS NULL) OR (Reservation.FromDate IS NULL OR Reservation.FromDate > @ToDate OR Reservation.FromDate > @ToDate AND Reservation.ToDate< @FromDate) OR (Reservation.FromDate IS NULL OR Reservation.FromDate > @ToDate OR Reservation.FromDate > @ToDate AND Reservation.ToDate IS NULL)";
                using MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@ReservationId", reservation.Id);
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                using MySqlDataReader reader = await command.ExecuteReaderAsync();

                List<Car> cars = new List<Car>();

                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32("Id");
                    string name = reader.GetString("Name");
                    string size = reader.GetString("Size");
                    string transmissionType = reader.GetString("TransmissionType");
                    string fuelType = reader.GetString("FuelType");
                    double pricePerDay = reader.GetDouble("PricePerDay");
                    int seatNumber = reader.GetInt32("SeatNumber");

                    cars.Add(new Car(id, name, size, transmissionType, fuelType, pricePerDay, seatNumber, pricePerDay * differenceInDays));
                }

                return cars.FirstOrDefault(tempCar => tempCar.Id == reservation.CarId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        private async Task<Boolean> ChangeReservation()
        {
            DateTimeOffset fromDate = (DateTimeOffset)FromCalendarDatePicker.Date;
            DateTimeOffset toDate = (DateTimeOffset)ToCalendarDatePicker.Date;

            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=car_rental_app;Uid=root;Pwd=;";

                using MySqlConnection connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "UPDATE Reservation SET FromDate = @FromDate, ToDate = @ToDate WHERE Id = @ReservationId";
                using MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@ReservationId", reservation.Id);
                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    int reservationIndex = Reservation.Instance.IndexOf(reservation);

                    Reservation.Instance[reservationIndex].FromDate = fromDate;
                    Reservation.Instance[reservationIndex].ToDate = toDate;

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private async void ChangeDatesButton_Click(object sender, RoutedEventArgs e)
        {
            Car car = await GetCars();

            if (car != null)
            {
                // If change of dates can happen
                // Get old date difference
                TimeSpan oldDayDifferenceTimeSpan = reservation.ToDate - reservation.FromDate;
                int oldDifferenceInDays = oldDayDifferenceTimeSpan.Days + 1;

                // Get new date difference
                DateTimeOffset fromDate = (DateTimeOffset)FromCalendarDatePicker.Date;
                DateTimeOffset toDate = (DateTimeOffset)ToCalendarDatePicker.Date;

                TimeSpan dayDifferenceTimeSpan = toDate - fromDate;
                int differenceInDays = dayDifferenceTimeSpan.Days + 1;

                // Get difference in price
                double oldPrice = car.PricePerDay * oldDifferenceInDays;
                double price = car.PricePerDay * differenceInDays;

                double priceDifference = price - oldPrice;

                if (priceDifference < 0)
                {
                    // Refund -priceDifference
                    ContentDialog refundDialog = new ContentDialog
                    {
                        Title = "Confirm change of reservation",
                        Content = $"Since the amount of days selected is the lower than before, you will be refunded {-1 * priceDifference}€. Are you sure you want to change the dates of your reservation?",
                        PrimaryButtonText = "Confirm",
                        CloseButtonText = "Cancel"
                    };

                    refundDialog.XamlRoot = ChangeDatesButton.XamlRoot;

                    ContentDialogResult result = await refundDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        // Change reservation
                        if (await ChangeReservation())
                            Frame.Navigate(typeof(ViewCarsPage));
                    }
                }
                else if (priceDifference > 0)
                {
                    // Charge priceDifference
                    ContentDialog chargeDialog = new ContentDialog
                    {
                        Title = "Confirm change of reservation",
                        Content = $"Since the amount of days selected is the higher than before, you will be charged {priceDifference}€. Are you sure you want to change the dates of your reservation?",
                        PrimaryButtonText = "Confirm",
                        CloseButtonText = "Cancel"
                    };

                    chargeDialog.XamlRoot = ChangeDatesButton.XamlRoot;

                    ContentDialogResult result = await chargeDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        // Change reservation
                        if (await ChangeReservation())
                            Frame.Navigate(typeof(ViewCarsPage));
                    }
                }
                else
                {
                    // Charge priceDifference
                    ContentDialog confirmDialog = new ContentDialog
                    {
                        Title = "Confirm change of reservation",
                        Content = "Since the amount of days selected is the same as before, you will not be charged or refunded any amount of money. Are you sure you want to change the dates of your reservation?",
                        PrimaryButtonText = "Confirm",
                        CloseButtonText = "Cancel"
                    };

                    confirmDialog.XamlRoot = ChangeDatesButton.XamlRoot;

                    ContentDialogResult result = await confirmDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        // Change reservation
                        if (await ChangeReservation())
                            Frame.Navigate(typeof(ViewCarsPage));
                    }
                }
            }
            else
            {
                // If change of dates can't happen
                IncorrectInputTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
