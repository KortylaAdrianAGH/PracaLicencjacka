using System;
using System.Collections.Generic;

using Xamarin.Forms;
using DriverControllApp.Models;
using SQLite;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using System.Linq;
using System.Diagnostics.Contracts;
using DriverControllApp.Persistence;
using System.Threading.Tasks;

namespace DriverControllApp.Menu
{
    public partial class DriverDetailPage : ContentPage
    {
        ObservableCollection<DeliveryPlace> currentDeliveryDataBase = new ObservableCollection<DeliveryPlace>();
        private readonly Geocoder geocoder = new Geocoder();
        Driver driver;
        bool IfAlreadyShown = false;
        private DataBaseHandler dataBaseHandler;


        public DriverDetailPage(Driver driver, DataBaseHandler dataBaseHandler)
        {
            this.dataBaseHandler = dataBaseHandler;
            currentDeliveryDataBase = dataBaseHandler.GetDeliveriesDataBase();
            this.driver = driver;
            BindingContext = this.driver;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (currentDeliveryDataBase.Count > 0 && IfAlreadyShown == false)
            {
                createRouteProgress();
            }
            IfAlreadyShown = true;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DriverMapViewPage(driver, dataBaseHandler));
        }

        void createRouteProgress()
        {
            foreach (var item in currentDeliveryDataBase)
            {
                if (item.driverName != driver.Name)
                {
                    continue;
                }
                string state = item.deliveryStatus > 0 ? "⎷" : "✕";
                Label label = new Label
                {
                    Text = state + "\t\t" + item.deliveryAdress,
                    Padding = 5.0,
                };
                stackLayout.Children.Add(label);
            }
            Content = stackLayout;
            driver.UpdateStatus(currentDeliveryDataBase);
            dataBaseHandler.UpdateDriverState(driver);
        }

        async void NewLocationButton(System.Object sender, System.EventArgs e)
        {
            if(newAdressEntry.Text != null)
            {
                var positions = await geocoder.GetPositionsForAddressAsync(newAdressEntry.Text);

                if (IfIllegalDelivery(positions))
                {
                    var newDelivery = new DeliveryPlace
                    {
                        deliveryId = findProperIndex(),
                        latitude = positions.First().Latitude,
                        longtitude = positions.First().Longitude,
                        driverName = driver.Name,
                        deliveryStatus = 0,
                        deliveryAdress = AddressCompression(newAdressEntry.Text)
                    };
                    dataBaseHandler.AddNewDeliveryPlaceToDataBase(newDelivery);
                    Label label = new Label
                    {
                        Text = "✕" + "\t\t" + AddressCompression(newAdressEntry.Text),
                        Padding = 5.0,
                    };
                    stackLayout.Children.Add(label);
                    Content = stackLayout;

                    driver.UpdateStatus(currentDeliveryDataBase);
                    dataBaseHandler.UpdateDriverState(driver);
                    await DisplayAlert("Udało się", "Dodano adres dowozowy do zadań", "OK");

                    newAdressEntry.Text = String.Empty;
                }
                
            }
            else
            {
                await DisplayAlert("Błąd wprowadzania", "Nie wprowadzono żadnych danych !", "OK");
            }
        }

        string AddressCompression(string address)
        {
            address = address.Replace(System.Environment.NewLine, ", ");
            address = address.Replace(", Polska", "");
            return address;
        }

        int findProperIndex()
        {
            int index = 0;

            foreach (var currentDelivery in currentDeliveryDataBase)
            {
                if (currentDelivery.deliveryId >= index)
                {
                    index = currentDelivery.deliveryId + 1;
                }
            }

            foreach (var doneDelivery in dataBaseHandler.GetDoneDeliveriesDataBase())
            {
                if (doneDelivery.deliveryId > index)
                {
                    index = doneDelivery.deliveryId + 1;
                }
            }
            return index;
        }

        bool IfIllegalDelivery(IEnumerable<Position> positions)
        {

            foreach (var deliveryPlace in currentDeliveryDataBase)
            {
                if ((deliveryPlace.latitude == positions.First().Latitude &&
                    deliveryPlace.longtitude == positions.First().Longitude) &&
                    deliveryPlace.driverName == driver.Name)
                {
                    DisplayAlert("Błąd", "Adres został już przypisany do kierowcy", "OK");
                    return false;
                }
            }

            if (positions.First() == null)
            {
                 DisplayAlert("Błąd", "Nie znaleziono takiego adresu\n" +
                     "Upewnij się, że jest poprawny i wpisz go ponownie", "OK");

                return false;
            }

            return true;
        }
    }
}

