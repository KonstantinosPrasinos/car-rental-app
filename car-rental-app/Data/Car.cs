using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_rental_app.Data
{
    public class Car
    {
        public string Title { get; set; }

        public Car(string title)
        {
            Title = title;
        }
    }
}
