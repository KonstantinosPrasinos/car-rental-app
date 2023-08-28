using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_rental_app.Data
{
    class Reservation
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public DateTimeOffset FromDate { get; set; }
        public DateTimeOffset ToDate { get; set; }
        public string CarName { get; set; }

        private static ObservableCollection<Reservation> _instance;

        // Public property to access the shared instance
        public static ObservableCollection<Reservation> Instance
        {
            get
            {
                // If the instance doesn't exist, create it
                if (_instance == null)
                {
                    _instance = new ObservableCollection<Reservation>();
                }
                return _instance;
            }
        }

        public Reservation(int id, int carId, DateTimeOffset fromDate, DateTimeOffset toDate, string carName)
        {
            Id = id;
            CarId = carId;
            FromDate = fromDate;
            ToDate = toDate;
            CarName = carName;
        }
    }
}
