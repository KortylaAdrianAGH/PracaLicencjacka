using System;
using System.Collections.Generic;

using Xamarin.Forms;
using DriverControllApp.Models;
using DriverControllApp.Persistence;
using System.Threading.Tasks;

namespace DriverControllApp.Menu
{
    public partial class MenuPage : ContentPage
    {
        private DataBaseHandler dataBaseHandler;

        public MenuPage(DataBaseHandler dataBaseHandler)
        {
            this.dataBaseHandler = dataBaseHandler;
            InitializeComponent();
        }

        void MapButtonClicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MapPage(dataBaseHandler));
        }

        void DriversButtonClicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MyDriverListPage(dataBaseHandler));
        }

        void DeliveryHistoryButtonClicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new doneDeliveriesPage(dataBaseHandler));
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

