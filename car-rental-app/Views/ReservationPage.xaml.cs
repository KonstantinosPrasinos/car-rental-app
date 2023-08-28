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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace car_rental_app.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReservationPage : Page
    {
        private DateTimeOffset fromDate;
        private DateTimeOffset toDate;
        private int carId;
        private int differenceInDays;
        private double pricePerDay;
        private string fromDateString;
        private string toDateString;
        private Car car;
        private string CardExpirationMonth;
        private string CardExpirationYear;
        public ReservationPage()
        {
            this.InitializeComponent();

            if (User.Instance.CardExpirationDate != null)
            {
                string[] values = User.Instance.CardExpirationDate.Split("/");
                CardExpirationMonth = values[0];
                CardExpirationYear = values[1];
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                // Split navigation parameters string to a list
                string navigationParameter = e.Parameter as string;

                string[] values = navigationParameter.Split('-');

                // Parse all the required fields from the list
                int.TryParse(values[0], out carId);
                DateTimeOffset.TryParse(values[1], out fromDate);
                DateTimeOffset.TryParse(values[2], out toDate);

                // Process the required 
                fromDateString = fromDate.ToString("dd/MM/yyyy");
                toDateString = toDate.ToString("dd/MM/yyyy");

                car = Car.Instance.First(car => car.Id == carId);

                TimeSpan dayDifferenceTimeSpan = toDate - fromDate;
                differenceInDays = dayDifferenceTimeSpan.Days + 1;
            }
        }

        private void Finallize_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ViewCarsPage));
        }
    }

}
