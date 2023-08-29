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
using System.Collections.ObjectModel;
using Microsoft.WindowsAppSDK.Runtime.Packages;
using MySqlConnector;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace car_rental_app.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class ViewCarsPage : Page
    {
        public ViewCarsPage()
        {
            this.InitializeComponent();

            FromCalendarPicker.MinDate = DateTime.Now.AddDays(1);
            ToCalendarPicker.MinDate = DateTime.Now.AddDays(1);

            if (Reservation.Instance.Count == 0)
            {
                GetReservations();
            }
        }

        private async void GetReservations()
        {
            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=car_rental_app;Uid=root;Pwd=;";

                using MySqlConnection connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "SELECT Car.Name, Reservation.* from Reservation LEFT JOIN car ON car.id = reservation.carid;";
                using MySqlCommand command = new MySqlCommand(query, connection);

                using MySqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32("Id");
                    int carId = reader.GetInt32("CarId");
                    DateTimeOffset fromDate = reader.GetDateTimeOffset("FromDate");
                    DateTimeOffset toDate = reader.GetDateTimeOffset("ToDate");
                    string carName = reader.GetString("Name");

                    Reservation.Instance.Add(new Reservation(id, carId, fromDate, toDate, carName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task<Boolean> GetCars()
        {
            Car.Instance.Clear();

            DateTimeOffset fromDate = (DateTimeOffset)FromCalendarPicker.Date;
            DateTimeOffset toDate = (DateTimeOffset)ToCalendarPicker.Date;

            TimeSpan dayDifferenceTimeSpan = toDate - fromDate;
            int differenceInDays = dayDifferenceTimeSpan.Days + 1;

            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=car_rental_app;Uid=root;Pwd=;";

                using MySqlConnection connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "SELECT Distinct Car.*FROM Car LEFT JOIN Reservation ON Car.Id = Reservation.CarId WHERE(Reservation.FromDate IS NULL OR Reservation.FromDate > @ToDate OR Reservation.ToDate< @FromDate) OR (Reservation.FromDate IS NULL OR Reservation.FromDate > @ToDate OR Reservation.ToDate IS NULL) OR (Reservation.FromDate IS NULL OR Reservation.FromDate > @ToDate OR Reservation.FromDate > @ToDate AND Reservation.ToDate< @FromDate) OR (Reservation.FromDate IS NULL OR Reservation.FromDate > @ToDate OR Reservation.FromDate > @ToDate AND Reservation.ToDate IS NULL)";
                using MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                using MySqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32("Id");
                    string name = reader.GetString("Name");
                    string size = reader.GetString("Size");
                    string transmissionType = reader.GetString("TransmissionType");
                    string fuelType = reader.GetString("FuelType");
                    double pricePerDay = reader.GetDouble("PricePerDay");
                    int seatNumber = reader.GetInt32("SeatNumber");

                    Car.Instance.Add(new Car(id, name, size, transmissionType, fuelType, pricePerDay, seatNumber, pricePerDay * differenceInDays));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }
        private void ReserveClick(object sender, RoutedEventArgs e)
        {
            if (sender is HyperlinkButton hyperlinkButton)
            {
                string carId = hyperlinkButton.Tag.ToString();

                DateTimeOffset fromDate = (DateTimeOffset)FromCalendarPicker.Date;
                DateTimeOffset toDate = (DateTimeOffset)ToCalendarPicker.Date;

                TimeSpan dayDifferenceTimeSpan = toDate - fromDate;
                int differenceInDays = dayDifferenceTimeSpan.Days + 1;

                double carPricePerDay = Car.Instance.First(car => car.Id == int.Parse(carId)).PricePerDay;

                string navigationParameter = carId + "-" + fromDate.ToString() + "-" + toDate.ToString();

                Frame.Navigate(typeof(ReservationPage), navigationParameter);
            }
        }

        private async void ToCalendarPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            // Make cars list visible
            NoDatesSelectedStackPanel.Visibility = Visibility.Collapsed;
            CarsListScrollViewer.Visibility = Visibility.Visible;

            await GetCars();
        }

        private void FromCalendarPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            ToCalendarPicker.IsEnabled = true;
            ToCalendarPicker.Date = null;
            ToCalendarPicker.MinDate = (DateTimeOffset)FromCalendarPicker.Date;
            ToCalendarPicker.IsCalendarOpen = true;
        }

        private void ChangeReservationClick(object sender, RoutedEventArgs e)
        {
            if (sender is HyperlinkButton button)
            {
                string reservationId = button.Tag.ToString();
                Frame.Navigate(typeof(ChangeReservationPage), reservationId);
            }
        }

        private async void CancelReservationClick(object sender, RoutedEventArgs e)
        {
            ContentDialog cancelReservationDialog = new ContentDialog
            {
                Title = "Cancel reservation?",
                Content = "If you cancel this reservation, it is not guarantee that you will be able to make it again. You will be refunded for the amount that was charged. Are you sure you want to cancel it?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            cancelReservationDialog.XamlRoot = NoDatesSelectedStackPanel.XamlRoot;

            ContentDialogResult result = await cancelReservationDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (sender is Button cancelButton)
                {
                    int id = int.Parse(cancelButton.Tag.ToString());


                    try
                    {
                        string connectionString = "Server=localhost;Port=3306;Database=car_rental_app;Uid=root;Pwd=;";

                        using MySqlConnection connection = new MySqlConnection(connectionString);
                        await connection.OpenAsync();

                        string query = "DELETE FROM reservation WHERE Id = @ReservationId";
                        using MySqlCommand command = new MySqlCommand(query, connection);

                        command.Parameters.AddWithValue("@ReservationId", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Reservation tempReservation = Reservation.Instance.FirstOrDefault(res => res.Id == id);

                            Reservation.Instance.Remove(tempReservation);
                        }
                        else
                        {
                            Console.WriteLine("No reservation found with the specified ID.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                // The user clicked the CloseButton.
            }
        }
    }
}
