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
    }
}
