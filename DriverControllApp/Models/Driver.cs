using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using DriverControllApp.Persistence;
using SQLite;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace DriverControllApp.Models
{
    public class Driver
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public double longtitude { get; set; }
        public double latitude { get; set; }
        

        public void UpdateStatus(ObservableCollection<DeliveryPlace> currentDeliveryDataBase)
        {
            int completedCount = 0;
            int AllDriverDeliveries = 0;

            foreach (var element in currentDeliveryDataBase)
            {
                if (element.driverName != this.Name)
                {
                    continue;
                }

                AllDriverDeliveries += 1;
                if (element.deliveryStatus > 0)
                {
                    completedCount += 1;
                }
            }
            if(currentDeliveryDataBase.Count == 1)
            {
                this.Status = $"Kierowca dostarczył {completedCount} na {AllDriverDeliveries} zamowień ";
            }
            else
            {
                this.Status =  $"Kierowca dostarczył {completedCount} na {AllDriverDeliveries} zamowienia ";
            }

            if (AllDriverDeliveries == 0)
            {
                this.Status = "Kierowca nie ma zaplanowanych dostaw";
            }
        }

        public Position GetPosition()
        {
            return new Position(latitude, longtitude);
        }

        public void SetPosition(Position position)
        {
            longtitude = position.Longitude;
            latitude = position.Latitude;
        }

        public async void UpdatePosition()
        {
            var location = await Geolocation.GetLocationAsync();
            if (location != null)
            {
                latitude = location.Latitude;
                longtitude = location.Longitude;
            }
        }

        public double DistanceFromBase()
        {
            UpdatePosition();

            var driverLocation = new Location(latitude, longtitude);
            var basePosition = new Base().GetBaseLocation();
            var baseLocation = new Location(basePosition.Latitude, basePosition.Longitude);
            
            return driverLocation.CalculateDistance(baseLocation, DistanceUnits.Kilometers);
           
        }

    }
}

