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
using MySqlConnector;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace car_rental_app.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddCarPage : Page
    {
        List<string> sizes = new List<string>();
        List<string> transmissionTypes = new List<string>();
        List<string> fuelTypes = new List<string>();

        Boolean isLoading = false;

        public AddCarPage()
        {
            sizes.Add("Compact");
            sizes.Add("SUV");
            sizes.Add("MidSize");
            sizes.Add("Convertible");
            sizes.Add("Sedan");

            transmissionTypes.Add("Automatic");
            transmissionTypes.Add("Manual");

            fuelTypes.Add("Gasoline");
            fuelTypes.Add("Diesel");
            fuelTypes.Add("Electric");

            this.InitializeComponent();
        }

        private async Task<Boolean> AddCar()
        {
            string name = NameTextBox.Text;
            string size = SizeComboBox.SelectedItem.ToString();
            string transmissionType = TransmissionTypeComboBox.SelectedItem.ToString();
            string fuelType = FuelTypeComboBox.SelectedItem.ToString();
            double pricePerDay = double.Parse(PriceTextBox.Text);
            int seatNumber = int.Parse(SeatNumberNumberBox.Text);

            if (name == null || size == null || transmissionType == null || fuelType == null) {
                return false;
            }

            try
            {
                string connectionString = "Server=localhost;Port=3306;Database=car_rental_app;Uid=root;Pwd=;";
                using MySqlConnection connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "INSERT INTO Car (Name, Size, TransmissionType, FuelType, PricePerDay, SeatNumber) VALUES (@Name, @Size, @TransmissionType, @FuelType, @PricePerDay, @SeatNumber)";
                using MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Size", size);
                command.Parameters.AddWithValue("@TransmissionType", transmissionType);
                command.Parameters.AddWithValue("@FuelType", fuelType);
                command.Parameters.AddWithValue("@PricePerDay", pricePerDay);
                command.Parameters.AddWithValue("@SeatNumber", seatNumber);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Car added successfully!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private async void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (isLoading) return;

            isLoading = true;

            if (await AddCar())
            {
                FailTextBlock.Visibility = Visibility.Collapsed;
                SuccessTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                SuccessTextBlock.Visibility = Visibility.Collapsed;
                FailTextBlock.Visibility = Visibility.Visible;
            }

            isLoading = false;
        }
    }
}
