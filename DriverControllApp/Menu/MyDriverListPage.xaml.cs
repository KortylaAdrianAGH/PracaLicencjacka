using System;
using System.Collections.Generic;

using Xamarin.Forms;
using DriverControllApp.Models;
using System.Linq;
using Xamarin.Essentials;
using DriverControllApp.Persistence;
using SQLite;
using System.Collections.ObjectModel;

namespace DriverControllApp.Menu
{
    public partial class MyDriverListPage : ContentPage
    {
        private ObservableCollection<Driver> driverDataBase;
        private DataBaseHandler dataBaseHandler;

        public MyDriverListPage(DataBaseHandler dataBaseHandler)
        {
            this.dataBaseHandler = dataBaseHandler;
            driverDataBase = dataBaseHandler.GetDriverDataBase();
            InitializeComponent();
        }

        protected override async void OnAppearing()
        { 
            base.OnAppearing();
            listView.ItemsSource = driverDataBase;
        }

        IEnumerable<Driver> GetContacts(string searchText = null)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return driverDataBase;
            }

            return driverDataBase.Where(c => c.Name.StartsWith(searchText));
        }

        void refreshPage(object sender, System.EventArgs e)
        {
            listView.ItemsSource = null;
            listView.ItemsSource = driverDataBase;
            listView.IsRefreshing = false;
        }

        void SearchBar_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            listView.ItemsSource = GetContacts(e.NewTextValue);
        }

        async void listView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var driver = e.Item as Driver;

            await Navigation.PushAsync(new DriverDetailPage(driver, dataBaseHandler));

            listView.ItemsSource = null;
            listView.ItemsSource = driverDataBase;
        }
    }
}

