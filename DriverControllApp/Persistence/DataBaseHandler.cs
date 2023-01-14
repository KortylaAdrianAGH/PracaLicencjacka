using System;
using DriverControllApp.Models;
using Firebase.Database;
using System.Collections.ObjectModel;
using Firebase.Database.Query;
using System.Data.Common;
using Firebase;
using FireSharp.Response;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseClient = Firebase.Database.FirebaseClient;
using Xamarin.Essentials;
using System.Linq;
using Xamarin.Forms.Maps;

namespace DriverControllApp.Persistence
{
    public class DataBaseHandler
    {
        FirebaseClient firebaseConnection = new FirebaseClient("https://drivercontrollapp-default-rtdb.europe-west1.firebasedatabase.app/");
        
        private ObservableCollection<RegisteredUsers> registeredUserDataBase = new ObservableCollection<RegisteredUsers>();
        private ObservableCollection<Driver> driverDataBase = new ObservableCollection<Driver>();
        private ObservableCollection<DeliveryPlace> currentDeliveryDataBase = new ObservableCollection<DeliveryPlace>();
        private ObservableCollection<DoneDeliveriesData> doneDeliveriesDataBase = new ObservableCollection<DoneDeliveriesData>();

        public DataBaseHandler()
        {
            var userDataBaseUpdate = firebaseConnection
                .Child("RegisteredUsers")
                .AsObservable<RegisteredUsers>()
                .Subscribe(async (dataBaseEvent) =>
                {
                    if (dataBaseEvent.Object != null && IfUserAlreadyStored(dataBaseEvent.Object))
                    {
                        registeredUserDataBase.Add(dataBaseEvent.Object);
                    }
                });

            var driverDataBaseUpdate = firebaseConnection
                .Child("Driver")
                .AsObservable<Driver>()
                .Subscribe(async (dataBaseEvent) =>
                {
                    if (dataBaseEvent.Object != null && IfDriverAlreadyStored(dataBaseEvent.Object))
                    {
                        driverDataBase.Add(dataBaseEvent.Object);
                    }
                });

            var currentDeliveriesDataBaseUpdate = firebaseConnection
                .Child("DeliveryPlace")
                .AsObservable<DeliveryPlace>()
                .Subscribe(async (dataBaseEvent) =>
                {
                    if (dataBaseEvent.Object != null && IfDeliveryAlreadyStarted(dataBaseEvent.Object))
                    {
                        currentDeliveryDataBase.Add(dataBaseEvent.Object);
                    }
                });


            var doneDeliveriesDataBaseUpdate = firebaseConnection
                .Child("doneDeliveriesDataBase")
                .AsObservable<DoneDeliveriesData>()
                .Subscribe(async (dataBaseEvent) =>
                {
                    if (dataBaseEvent.Object != null && IfDeliveryAlreadyDone(dataBaseEvent.Object))
                    {
                        doneDeliveriesDataBase.Add(dataBaseEvent.Object);
                    }
                });

            UpdateAllDrivers();


        }

        public async void PollRegisteredUserData()
        {
            var polledData = (await firebaseConnection
                           .Child("RegisteredUsers")
                           .OnceAsync<RegisteredUsers>())
                          .Select(item => new RegisteredUsers
                          {
                              accountId = item.Object.accountId,
                              accountType = item.Object.accountType,
                              login = item.Object.login,
                              name = item.Object.name,
                              password = item.Object.password
                          });

            registeredUserDataBase.Clear();
            foreach (var user in polledData)
            {
                registeredUserDataBase.Add(user);
            }
        }

        public async void PollDriverData()
        {
            var driverPolledData = (await firebaseConnection
                            .Child("Driver")
                            .OnceAsync<Driver>())
                            .Select(item => new Driver
                            {
                                Name = item.Object.Name,
                                Status = item.Object.Status,
                                longtitude = item.Object.longtitude,
                                latitude = item.Object.latitude
                            });

            driverDataBase.Clear();
            foreach (var driver in driverPolledData)
            {
                driverDataBase.Add(driver);
            }
        }

        public async void PollCurrentDeliveriesData()
        {
            var currentDeliveriesDataPolled = (await firebaseConnection
                          .Child("DeliveryPlace")
                          .OnceAsync<DeliveryPlace>())
                          .Select(item => new DeliveryPlace
                          {
                              deliveryId = item.Object.deliveryId,
                              driverName = item.Object.driverName,
                              deliveryAdress = item.Object.deliveryAdress,
                              deliveryDoneTime = item.Object.deliveryDoneTime,
                              deliveryStatus = item.Object.deliveryStatus,
                              latitude = item.Object.latitude,
                              longtitude = item.Object.longtitude
                          });

            currentDeliveryDataBase.Clear();
            foreach (var delivery in currentDeliveriesDataPolled)
            {
                currentDeliveryDataBase.Add(delivery);
            }
        }

        public async void PollDoneDeliveriesData()
        {
            var doneDeliveriesPollData = (await firebaseConnection
                            .Child("doneDeliveriesDataBase")
                            .OnceAsync<DoneDeliveriesData>())
                            .Select(item => new DoneDeliveriesData
                            {
                                deliveryId = item.Object.deliveryId,
                                driverName = item.Object.driverName,
                                deliveryDoneTime = item.Object.deliveryDoneTime,
                                deliveryAddress = item.Object.deliveryAddress
                            });

            doneDeliveriesDataBase.Clear();
            foreach (var doneDelivery in doneDeliveriesPollData)
            {

                doneDeliveriesDataBase.Add(doneDelivery);
            }
        }


        public ObservableCollection<RegisteredUsers> GetRegisteredUsersDataBase()
        {
            PollRegisteredUserData();
            return registeredUserDataBase;
        }

        public ObservableCollection<Driver> GetDriverDataBase()
        {
            PollDriverData();
            return driverDataBase;
        }

        public ObservableCollection<DeliveryPlace> GetDeliveriesDataBase()
        {
            
            PollCurrentDeliveriesData();
            
            return currentDeliveryDataBase;
        }

        public ObservableCollection<DoneDeliveriesData> GetDoneDeliveriesDataBase()
        {
            
            PollDoneDeliveriesData();
            
            return doneDeliveriesDataBase;
        }


        public async void AddNewUserToDataBase(RegisteredUsers newUser)
        {
            await firebaseConnection.Child("RegisteredUsers").PostAsync(newUser);
        }

        public async void AddNewDriverToDataBase(Driver newDriver)
        {
            await firebaseConnection.Child("Driver").PostAsync(newDriver);
        }

        public async void AddNewDeliveryPlaceToDataBase(DeliveryPlace newDeliveryPlace)
        {
            await firebaseConnection.Child("DeliveryPlace").PostAsync(newDeliveryPlace);
        }

        public async void AddNewDoneDeliveryToDataBase(DoneDeliveriesData newDoneDelivery)
        {
            await firebaseConnection.Child("doneDeliveriesDataBase").PostAsync(newDoneDelivery);
        }

        public async void DeleteDeliveryPlaceFromDataBase(DeliveryPlace deliveryPlace)
        {
            var updatedDelivery = (await firebaseConnection
            .Child("DeliveryPlace")
            .OnceAsync<DeliveryPlace>()).Where(a => a.Object.deliveryId == deliveryPlace.deliveryId).FirstOrDefault();

            if (updatedDelivery != null)
            {
                updatedDelivery.Object.deliveryStatus = 1;
                updatedDelivery.Object.deliveryDoneTime = deliveryPlace.deliveryDoneTime;
                await firebaseConnection
                   .Child("DeliveryPlace")
                   .Child(updatedDelivery.Key)
                   .DeleteAsync();
            }

        }
       
        bool IfDriverAlreadyStored(Driver newDriver)
        {
            PollDriverData();
            foreach (var driver in driverDataBase)
            {
                if(driver.Name == newDriver.Name)
                {
                    return false;
                }
            }
            return true;
        }

        bool IfUserAlreadyStored(RegisteredUsers newUser)
        {
            PollRegisteredUserData();
            foreach (var user in registeredUserDataBase)
            {
                if (user.login == newUser.login)
                {
                    return false;
                }
            }
            return true;
        }

        bool IfDeliveryAlreadyStarted(DeliveryPlace newDelivery)
        {
            foreach (var currentDelivery in currentDeliveryDataBase)
            {
                if (currentDelivery.deliveryAdress == newDelivery.deliveryAdress)
                {
                    return false;
                } else if(newDelivery.deliveryId == currentDelivery.deliveryId)
                {
                    return false;
                }
            }
            return true;
        }

        bool IfDeliveryAlreadyDone(DoneDeliveriesData newDoneDelivery)
        {
            PollDoneDeliveriesData();
            foreach (var doneDelivery in doneDeliveriesDataBase)
            {
                if(doneDelivery.deliveryId == null)
                {
                    continue;
                }
                if (doneDelivery.deliveryId == newDoneDelivery.deliveryId)
                {
                    return false;
                }
            }
            return true;
        }

        public async void UpdateDeliveryStatus(DeliveryPlace deliveryPlace)
        {
            var updatedDelivery = (await firebaseConnection
            .Child("DeliveryPlace")
            .OnceAsync<DeliveryPlace>()).Where(a => a.Object.deliveryId == deliveryPlace.deliveryId).FirstOrDefault();

            if (updatedDelivery != null)
            {
                updatedDelivery.Object.deliveryStatus = 1;
                updatedDelivery.Object.deliveryDoneTime = deliveryPlace.deliveryDoneTime;
                await firebaseConnection
                   .Child("DeliveryPlace")
                   .Child(updatedDelivery.Key)
                   .PutAsync(updatedDelivery.Object);
            }
        }

        public async void UpdateDriverState(Driver driver)
        {
            var updatedDriver = (await firebaseConnection
            .Child("Driver")
            .OnceAsync<Driver>()).Where(a => a.Object.Name == driver.Name).FirstOrDefault();

            if (updatedDriver != null)
            {
                updatedDriver.Object.Status = driver.Status;
                updatedDriver.Object.longtitude = driver.longtitude;
                updatedDriver.Object.latitude = driver.latitude;

                await firebaseConnection
                   .Child("Driver")
                   .Child(updatedDriver.Key)
                   .PutAsync(updatedDriver.Object);
            }
        }

        public async void UpdateAllDrivers()
        {
            foreach(var driver in driverDataBase)
            {
                driver.UpdatePosition();
                driver.UpdateStatus(this.GetDeliveriesDataBase());
                await Task.Delay(2000);
                UpdateDriverState(driver);

            }
        }

        public Position GetGivenDriverPositon(string driverName)
        {
            foreach(var driverTupple in driverDataBase)
            {
                if (driverTupple.Name == driverName)
                {
                    return new Position(driverTupple.latitude, driverTupple.longtitude);
                }
            }
            return new Position();
        }

    }
}

