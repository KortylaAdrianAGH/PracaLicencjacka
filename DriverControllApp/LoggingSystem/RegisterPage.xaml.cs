using System;
using System.Collections.Generic;
using DriverControllApp.Models;
using SQLite;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using DriverControllApp.Persistence;

namespace DriverControllApp.LoggingSystem
{

    public partial class RegisterPage : ContentPage
    {
        private ObservableCollection<RegisteredUsers> registeredUserDataBase = new ObservableCollection<RegisteredUsers>();
        private DataBaseHandler dataBaseHandler;


        public RegisterPage(DataBaseHandler dataBaseHandler)
        {
            InitializeComponent();
            this.dataBaseHandler = dataBaseHandler;
        }

        async void RegisterButtonClicked(System.Object sender, System.EventArgs e)
        {
            registeredUserDataBase = dataBaseHandler.GetRegisteredUsersDataBase();

            if(passwordEntry.Text == null || loginEntry.Text.Length == null || userNameEntry.Text.Length == null)
            {
                await DisplayAlert("Błąd rejestracji", "Nie wszystkie pola zostały poprawnie wypełnione", "Popraw");
            }
            else if (IfUserAlreadyRegistered())
            {                
            }
            else
            {
                DisplayAlert("Rejestracja udana", "Stworzono nowe konto, można się zalogować", "OK");
                AddUserToDataBase();
                Navigation.PopAsync();
            }

            passwordEntry.Text = null;
            loginEntry.Text = null;
            userNameEntry.Text = null;
                
        }

        void passwordEntry_Completed(System.Object sender, System.EventArgs e)
        {
            RegisterButtonClicked(sender, e);
        }

        void Handle_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if(choiceLabel.Text == "Konto kierowcy")
            {
                choiceLabel.Text = "Konto nadzorującego"; 
            }
            else
            {
                choiceLabel.Text = "Konto kierowcy";
            }
        }

        void AddUserToDataBase()
        {
            if (choiceSwitch.IsToggled == true)
            {

                var enteredPassword = passwordEntry.Text;
                var enteredName = userNameEntry.Text;
                var enteredLogin = loginEntry.Text;

                var newUserTuple = new RegisteredUsers
                {
                    accountId = registeredUserDataBase.Count,
                    login = enteredLogin,
                    name = enteredName,
                    password = enteredPassword,
                    accountType = "KIEROWCA"
                };

                dataBaseHandler.AddNewUserToDataBase(newUserTuple);

                var newDriverTuple = new Driver()
                {
                    Name = enteredName,
                    Status = "Kierowca nie ma zaplanowanych dostaw",
                };
                dataBaseHandler.AddNewDriverToDataBase(newDriverTuple);
            }
            else
            {
                var newUserTuple = new RegisteredUsers
                {
                    accountId = registeredUserDataBase.Count,
                    login = loginEntry.Text,
                    name = userNameEntry.Text,
                    password = passwordEntry.Text,
                    accountType = "ADMIN"
                };

                dataBaseHandler.AddNewUserToDataBase(newUserTuple);

            }
        }

        bool IfUserAlreadyRegistered()
        {
            foreach(var user in registeredUserDataBase)
            {
                if(loginEntry.Text == user.login)
                {
                    DisplayAlert("Błąd", "Użytkownik o wprowadzonym loginie już istnieje", "Wpisz inny login");
                    return true;
                }
                else if (userNameEntry.Text == user.name)
                {
                    DisplayAlert("Błąd", "Użytkownik o wprowadzonej nazwie użytkownika już istnieje", "Wpisz inną nazwę");
                    return true;
                }
            }
            return false;
        }
    }
}

