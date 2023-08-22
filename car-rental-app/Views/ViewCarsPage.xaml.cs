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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace car_rental_app.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class ViewCarsPage : Page
    {
        ObservableCollection<Car> cars = new ObservableCollection<Car>();
        ObservableCollection<Reservation> reservations = new ObservableCollection<Reservation>();

        public ViewCarsPage()
        {
            this.InitializeComponent();

            FromCalendarPicker.MinDate = DateTime.Now.AddDays(1);
            ToCalendarPicker.MinDate = DateTime.Now.AddDays(1);
        }

        private async Task<Boolean> GetCars()
        {
            cars.Clear();

            DateTimeOffset fromDate = (DateTimeOffset)FromCalendarPicker.Date;
            DateTimeOffset toDate = (DateTimeOffset)ToCalendarPicker.Date;

            TimeSpan dayDifferenceTimeSpan = toDate - fromDate;
            int differenceInDays = dayDifferenceTimeSpan.Days;

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

                    cars.Add(new Car(id, name, size, transmissionType, fuelType, pricePerDay, seatNumber, pricePerDay * differenceInDays));
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
                string carId = hyperlinkButton.Tag as string;

                Frame.Navigate(typeof(ReservationPage), carId);
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
    }
}
