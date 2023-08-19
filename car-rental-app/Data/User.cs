using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_rental_app.Data
{
    public class User
    {
        // Private static instance of the User class
        private static User _instance;

        // Public property to access the shared instance
        public static User Instance
        {
            get
            {
                // If the instance doesn't exist, create it
                if (_instance == null)
                {
                    _instance = new User();
                }
                return _instance;
            }
        }

        // Add your user-related properties here
        public string Email { get; set; }
        public Boolean IsAdmin { get; set; }
        public int Id { get; set; }

        // Private constructor to prevent external instantiation
        private User()
        {
            // Initialize properties or perform other setup here
        }
    }
}
