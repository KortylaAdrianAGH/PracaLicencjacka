using System;
using System.Collections.Generic;
using System.Linq;
using CrossPlatformLibrary.Geolocation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;
using Position = Xamarin.Forms.Maps.Position;
using Contact = DriverControllApp.Models.Driver;
using DriverControllApp.Models;
using System.Threading.Tasks;
using DriverControllApp.Persistence;

namespace DriverControllApp.Menu
{
    public partial class DriverMapViewPage : ContentPage
    {
        private readonly Geocoder geocoder = new Geocoder();
        private Driver driver;
        DataBaseHandler dataBaseHandler;

        public DriverMapViewPage(Driver driver, DataBaseHandler dataBaseHandler)
        {
            this.dataBaseHandler = dataBaseHandler;
            this.driver = driver;
            InitializeComponent();
            UpdateLocationOfDriver();
        }

        async void Map_MapClicked(System.Object sender, Xamarin.Forms.Maps.MapClickedEventArgs e)
        {

            var address = await geocoder.GetAddressesForPositionAsync(e.Position);
            await DisplayAlert("Address", address.FirstOrDefault()?.ToString(), "OK");
        }

        async void UpdateLocationOfDriver()
        {
            while (true)
            {
                myMap.Pins.Clear();
                Position newDriverPosition = dataBaseHandler.GetGivenDriverPositon(driver.Name);
                Pin driverPin = new Pin
                {
                    Label = driver.Name + "\n" + driver.Status,
                    Position = newDriverPosition
                };
                myMap.MoveToRegion(MapSpan.FromCenterAndRadius(newDriverPosition, Distance.FromKilometers(0.5)));
                myMap.Pins.Add(driverPin);
                
                await Task.Delay(3000);
            }
        }
    }
}
