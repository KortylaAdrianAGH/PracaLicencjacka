using System;
using System.Collections.Generic;
using SQLite;
using System.Collections.ObjectModel;


using Xamarin.Forms;
using DriverControllApp.Persistence;
using DriverControllApp.Models;

namespace DriverControllApp.Menu
{
    public partial class doneDeliveriesPage : ContentPage
    {
        private ObservableCollection<DoneDeliveriesData> doneDeliveriesDataBase;
        DataBaseHandler dataBaseHandler;

        public doneDeliveriesPage(DataBaseHandler dataBaseHandler)
        {
            this.dataBaseHandler = dataBaseHandler;
            doneDeliveriesDataBase = dataBaseHandler.GetDoneDeliveriesDataBase();

            InitializeComponent();
            doneDeliveriesListView.ItemsSource = doneDeliveriesDataBase;

        }

        void refreshPage(object sender, System.EventArgs e)
        {
            doneDeliveriesDataBase = dataBaseHandler.GetDoneDeliveriesDataBase();
            doneDeliveriesListView.ItemsSource = doneDeliveriesDataBase;
            doneDeliveriesListView.IsRefreshing = false;
        }
    }
}

