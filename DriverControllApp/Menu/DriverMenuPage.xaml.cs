using System;
using System.Collections.Generic;
using Xamarin.Forms;
using DriverControllApp.Models;
using DriverControllApp.Persistence;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace DriverControllApp.Menu
{
    public partial class DriverMenuPage : ContentPage
    {
        Driver loggedDriver;
        private DataBaseHandler dataBaseHandler;
        

        public DriverMenuPage(Driver driver, DataBaseHandler dataBaseHandler)
        {
            InitializeComponent();
            loggedDriver = driver;
            this.dataBaseHandler = dataBaseHandler;
        }

        async void DeliveryButtonClicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DriverTaskPage(loggedDriver, dataBaseHandler));
        }

        async void LogOutButtonClicked(System.Object sender, System.EventArgs e)
        {
            await DisplayAlert("Wylogowanie", "Wylogowanie udało się", "OK");
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}

