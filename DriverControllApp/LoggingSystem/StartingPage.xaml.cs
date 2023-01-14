using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DriverControllApp;
using DriverControllApp.Models;
using SQLite;
using Xamarin.Forms;
using DriverControllApp.Persistence;

namespace DriverControllApp.LoggingSystem
{
    public partial class StartingPage : ContentPage
    {
        private DataBaseHandler dataBaseHandler = new DataBaseHandler();
        private ObservableCollection<RegisteredUsers> registeredUserDataBase = new ObservableCollection<RegisteredUsers>();
        private ObservableCollection<Driver> driverDataBase = new ObservableCollection<Driver>();

        public StartingPage()
        {
            InitializeComponent();
        }

        async void LogInButtonClicked(System.Object sender, System.EventArgs e)
        {
            registeredUserDataBase = dataBaseHandler.GetRegisteredUsersDataBase();
            driverDataBase = dataBaseHandler.GetDriverDataBase();

            var user = ifCorrectLogInData();
            if (user != null)
            {

                await DisplayAlert("Witaj " + user.name, "Logowanie powiodło się", "OK");
                if (user.accountType == "ADMIN")
                {
                    await Navigation.PushAsync(new Menu.MenuPage(dataBaseHandler));

                }
                else
                {
                    await Navigation.PushAsync(new Menu.DriverMenuPage(foundDriverAccout(user), dataBaseHandler));
                }
                loginEntry.Text = null;
                passwordEntry.Text = null;
            }
            else
            {
                await DisplayAlert("NIE MOŻNA SIĘ ZALOGOWAĆ", "Hasło lub e-mail nie prawidłowy", "OK");
                loginEntry.Text = null;
                passwordEntry.Text = null;
            }
        }

        async void RegisterButtonClicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage(dataBaseHandler));
            loginEntry.Text = null;
            passwordEntry.Text = null;

        }

        void passwordEntry_Completed(System.Object sender, System.EventArgs e)
        {
            LogInButtonClicked(sender, e);
        }

        RegisteredUsers ifCorrectLogInData()
        {
            foreach (var user in registeredUserDataBase)
            {
                if (loginEntry.Text == null || passwordEntry.Text == null)
                {
                    return null;
                }
                if(user.login == loginEntry.Text && user.password == passwordEntry.Text)
                {
                    return user;
                }
            }
            return null;
        }

        Driver foundDriverAccout(RegisteredUsers user)
        {
            foreach (var driver in driverDataBase)
            {
                if(driver.Name == user.name)
                {
                    return driver;
                }
            }
            return null;
        }

    }
}

