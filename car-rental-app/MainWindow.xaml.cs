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

namespace car_rental_app
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        int totalButtons = 0;
        public MainWindow()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(MainPage));
        }

        public void AddCar(object sender, RoutedEventArgs e)
        {
            Button newButton = new Button();
            HyperlinkButton newHyperLinkButton = new HyperlinkButton();

            newButton.Name = $"Button #{totalButtons}";
            newButton.Content = $"Button #{totalButtons}";
            newButton.Click += ChangeReservation;

            newHyperLinkButton.Name = $"ChangeButton{totalButtons}";
            newHyperLinkButton.Content = "Change";
            newHyperLinkButton.Click += ChangeReservation;

            RentedCarsStackPanel.Children.Add(newButton);

            totalButtons++;
        }

        private void RemoveCar(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.Content = "hello";
        }

        private void ChangeReservation(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Page2));
        }
    }
}
