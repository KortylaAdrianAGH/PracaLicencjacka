using System;

namespace DriverControllApp.Models
{
    public class DeliveryPlace
    {
        public int deliveryId { get; set; }

        public string driverName { get; set; }

        public double latitude { get; set; }

        public double longtitude { get; set; }

        public string deliveryAdress { get; set; }

        public int deliveryStatus { get; set; }

        public DateTime deliveryDoneTime { get; set; }  
    }
}

