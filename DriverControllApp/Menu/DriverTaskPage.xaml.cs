using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using DriverControllApp.Models;
using DriverControllApp.Persistence;

using Position = Xamarin.Forms.Maps.Position;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DriverControllApp.Menu
{
    public partial class DriverTaskPage : ContentPage
    {
        ObservableCollection<DeliveryPlace> currentDeliveryDataBase = new ObservableCollection<DeliveryPlace>();
        ObservableCollection <DeliveryPlace> currentDriverLocations = new ObservableCollection<DeliveryPlace>();
        DataBaseHandler dataBaseHandler;
        bool IfWorking;
        Driver driver;

        public DriverTaskPage(Driver driver, DataBaseHandler dataBaseHandler)
        {
            this.dataBaseHandler = dataBaseHandler;
            this.IfWorking = false;
            currentDeliveryDataBase = dataBaseHandler.GetDeliveriesDataBase();
            InitializeComponent();
            BindingContext = driver;
            this.driver = driver;
        }

        void UpdateDriverDeliveryList()
        {
            currentDriverLocations.Clear();
            foreach (var recipe in currentDeliveryDataBase)
            {
                if (recipe.driverName == driver.Name)
                {
                    currentDriverLocations.Add(recipe);
                }
            }
            listView.ItemsSource = currentDriverLocations;
            driver.UpdateStatus(currentDeliveryDataBase);
            dataBaseHandler.UpdateDriverState(driver);
        }

        protected override async void OnAppearing()
        {
            UpdateDriverDeliveryList();
            base.OnAppearing();
        }

        void checkRouteProgress()
        {           
            int allDeliveryPlaces = 0;
            int deliveredPlaces = 0;
            foreach (var singleDelivery in currentDeliveryDataBase)
            {
                if (singleDelivery.driverName != driver.Name)
                {
                    continue;
                }
                allDeliveryPlaces++;
                deliveredPlaces += singleDelivery.deliveryStatus;

            }
            if (deliveredPlaces == allDeliveryPlaces && deliveredPlaces != 0)
            {
                showWhenCompleted();
            }
            
        }   

        async void showWhenCompleted()
        {
            await DisplayAlert("Brawo !", "Udało się dostarczyć wszystkie zamówienia, wracaj do bazy!", "Wracam");
            driver.UpdateStatus(currentDeliveryDataBase);
            RouteCompleted();

            Navigation.PopAsync();
        }


        async void listView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var deliveryPlace = e.Item as DeliveryPlace;

            if (deliveryPlace.deliveryStatus != 1)
            {
                deliveryPlace.deliveryStatus = 1;
                deliveryPlace.deliveryDoneTime = DateTime.Now;

                var doneDelivery = new DoneDeliveriesData
                {
                    deliveryId = deliveryPlace.deliveryId,
                    deliveryDoneTime = DateTime.Now,
                    deliveryAddress = deliveryPlace.deliveryAdress,
                    driverName = driver.Name

                };
                dataBaseHandler.AddNewDoneDeliveryToDataBase(doneDelivery);
                dataBaseHandler.UpdateDeliveryStatus(deliveryPlace);
                listView.ItemsSource = null;
                listView.ItemsSource = currentDriverLocations;
               
            }

            var deliveredPlaceLocation = new Position(deliveryPlace.latitude, deliveryPlace.longtitude);
            driver.SetPosition(deliveredPlaceLocation);
            driver.UpdateStatus(currentDeliveryDataBase);
            dataBaseHandler.UpdateDriverState(driver);

            checkRouteProgress();

            listView.ItemsSource = currentDriverLocations;

        }

        void RouteCompleted()
        {
            foreach(var deliveryPlace in currentDriverLocations)
            {
                dataBaseHandler.DeleteDeliveryPlaceFromDataBase(deliveryPlace);
            }
            listView.ItemsSource = null;
            listView.ItemsSource = currentDriverLocations;
        }

        void refreshPage(object sender, System.EventArgs e)
        {
            UpdateDriverDeliveryList();
            listView.IsRefreshing = false;
        }

        async void StartWorkButton(System.Object sender, System.EventArgs e)
        {
            this.IfWorking = true;
            while (IfWorking)
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromSeconds(10)));
                if (location != null)
                {
                    Position newPosition = new Position(location.Latitude, location.Longitude);
                    listView.ItemsSource = null;
                    UpdateDriverDeliveryList();
                    listView.ItemsSource = currentDriverLocations;
                    driver.SetPosition(newPosition);
                }
                await Task.Delay(3000);
                dataBaseHandler.UpdateDriverState(driver);
            }
        }

        void EndWorkButton(System.Object sender, System.EventArgs e)
        {
            this.IfWorking = false;
            Position newPosition = new Position();
            driver.SetPosition(newPosition);
            dataBaseHandler.UpdateDriverState(driver);
        }

    }
}

