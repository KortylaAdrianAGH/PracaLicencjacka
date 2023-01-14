using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DriverControllApp
{
    public partial class App : Application
    {
        public App ()
        {
            InitializeComponent();

            MainPage = new NavigationPage( new LoggingSystem.StartingPage());
            //MainPage = new NavigationPage(new Persistence.DataBaseTestPage());
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

