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

        public ViewCarsPage()
        {
            this.InitializeComponent();
            cars.Add(new Car("0", "Toyota Yaris", "Small", "Automatic", "Petrol", 70.0, 5));
            cars.Add(new Car("1", "Toyota Aygo", "Small", "Automatic", "Petrol", 60.0, 4));
            cars.Add(new Car("2", "Toyota Aygo", "Small", "Automatic", "Petrol", 60.0, 4));
            cars.Add(new Car("3", "Toyota Aygo", "Small", "Automatic", "Petrol", 60.0, 4));
            cars.Add(new Car("4", "Toyota Aygo", "Small", "Automatic", "Petrol", 60.0, 4));
            cars.Add(new Car("5", "Toyota Aygo", "Small", "Automatic", "Petrol", 60.0, 4));
        }
        private void ReserveClick(object sender, RoutedEventArgs e)
        {
            if (sender is HyperlinkButton hyperlinkButton)
            {
                string carId = hyperlinkButton.Tag as string;

                Frame.Navigate(typeof(ReservationPage), carId);
            }
        }

        private void ToCalendarPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (FromCalendarPicker.Date != null)
            {
                SearchButton.IsEnabled = true;
            }
        }

        private void FromCalendarPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (ToCalendarPicker.Date != null)
            {
                SearchButton.IsEnabled = true;
            }
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            if (ToCalendarPicker.Date != null && FromCalendarPicker.Date != null)
            {
                NoDatesSelectedStackPanel.Visibility = Visibility.Collapsed;
                CarsListScrollViewer.Visibility = Visibility.Visible;
            }
        }
    }
}
