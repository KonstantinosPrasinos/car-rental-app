using Microsoft.WindowsAppSDK.Runtime.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_rental_app.Data
{
    public class User
    {
        private static User _instance;

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnUsernameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        // Event to be triggered when Username changes
        public event EventHandler OnUsernameChanged;

        private User()
        {
        }

        public static User Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new User();
                }
                return _instance;
            }
        }
    }
}
