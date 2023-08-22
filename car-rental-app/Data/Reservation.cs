using System;
using System.Collections.Generic;
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
