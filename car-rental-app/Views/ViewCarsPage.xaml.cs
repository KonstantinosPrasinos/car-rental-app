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
    
    public class CarViewModel
    {
        private ObservableCollection<Car> cars = new ObservableCollection<Car>();
        public ObservableCollection<Car> Cars { get { return cars; } }

        public CarViewModel()
        {
            cars.Add(new Car("Car1"));
            cars.Add(new Car("Car2"));
        }
    }
    public sealed partial class ViewCarsPage : Page
    {
        ObservableCollection<Car> cars = new ObservableCollection<Car>();

        public ViewCarsPage()
        {
            this.InitializeComponent();
            cars.Add(new Car("Car1"));
            cars.Add(new Car("Car2"));
        }

        private void ClickMoment(object sender, RoutedEventArgs e)
        {
            cars.Add(new Car("Car3"));
        }
    }
}
