using System;

namespace DriverControllApp.Models
{
    public class DoneDeliveriesData
    {
        public int deliveryId { get; set; }

        public string driverName { get; set; }

        public string deliveryAddress { get; set; }

        public DateTime deliveryDoneTime { get; set; }
    }
}

