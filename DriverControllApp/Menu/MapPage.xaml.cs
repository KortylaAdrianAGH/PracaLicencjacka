using System;
using System.Collections.Generic;
using System.Linq;
using CrossPlatformLibrary.Geolocation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;
using Position = Xamarin.Forms.Maps.Position;
using DriverControllApp.Models;
using SQLite;
using System.Collections.ObjectModel;
using DriverControllApp.Persistence;
using System.Threading.Tasks;

namespace DriverControllApp.Menu
{
    public partial class MapPage : ContentPage
    {
        private readonly Geocoder geocoder = new Geocoder();
        private ObservableCollection<Driver> driverDataBase;
        DataBaseHandler dataBaseHandler;

        private Pin searchedPlacePin = new Pin
        {
            Label = "Szukana lokalizacja"
        };
        

        public MapPage(DataBaseHandler dataBaseHandler)
        {
            this.dataBaseHandler = dataBaseHandler;
            driverDataBase = dataBaseHandler.GetDriverDataBase();
            InitializeComponent();
            setCentralPointMapView();
            UpdateLocationOfDrivers();
        }

        async void setCentralPointMapView()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if(location != null)
            {
                Position position = new Position(location.Latitude, location.Longitude);
                myMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(0.5)));
            }
            else
            {
                await DisplayAlert("Błąd", "Nie możemy zczytać twojej lokalizacji", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            ShowBaseLocation();
            base.OnAppearing();

            UpdateLocationOfDrivers();
        }

        async void Map_MapClicked(System.Object sender, Xamarin.Forms.Maps.MapClickedEventArgs e)
        {
            
            var address = await geocoder.GetAddressesForPositionAsync(e.Position);
            await DisplayAlert("Address", address.FirstOrDefault()?.ToString(), "OK");
        }

        async void locationSearchBar_Completed(System.Object sender, System.EventArgs e)
        {
            var positions = await geocoder.GetPositionsForAddressAsync(locationSearchBar.Text);

            if (positions.First() != null)
            {
                myMap.MoveToRegion(MapSpan.FromCenterAndRadius(positions.First(), Distance.FromKilometers(0.5)));
                searchedPlacePin.Position = positions.First();
                searchedPlacePin.Label = locationSearchBar.Text;
                myMap.Pins.Add(searchedPlacePin);
            }
            else
            {
                await DisplayAlert("Błąd", "Nie znaleziono takiego adresu\n" +
                    "Upewnij się, że jest poprawny i wpisz go ponownie", "OK");
            }
        }

        async void UpdateLocationOfDrivers()
        {
            while (true)
            {
                myMap.Pins.Clear();
                ShowBaseLocation();
                foreach (var driver in driverDataBase)
                {
                    Pin driverPin = new Pin
                    {
                        Label = driver.Name + "\n" + driver.Status,
                        Position = driver.GetPosition()
                    };
                    myMap.Pins.Add(driverPin);
                }
                await Task.Delay(3000);
            }
        }

        void ShowBaseLocation()
        {
            Pin baseLocation = new Pin
            {
                Label = "Baza",
                Position = new Base().GetBaseLocation()
            };

            myMap.Pins.Add(baseLocation);
        }
    }
}
