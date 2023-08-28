using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_rental_app.Data
{
    public class Car
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string TransmissionType { get; set; }
        public string FuelType { get; set; }
        public double PricePerDay { get; set; }
        public int SeatNumber { get; set; }

        public double TotalPrice { get; set; }

        // Private static instance of the User class
        private static ObservableCollection<Car> _instance;

        // Public property to access the shared instance
        public static ObservableCollection<Car> Instance
        {
            get
            {
                // If the instance doesn't exist, create it
                if (_instance == null)
                {
                    _instance = new ObservableCollection<Car>();
                }
                return _instance;
            }
        }

        public Car(int id, string name, string size, string transmissionType, string fuelType, double pricePerDay, int seatNumber, double totalPrice)
        {
            Id = id;
            Name = name;
            Size = size;
            TransmissionType = transmissionType;
            FuelType = fuelType;
            PricePerDay = pricePerDay;
            SeatNumber = seatNumber;
            TotalPrice = totalPrice;
        }
    }
}
