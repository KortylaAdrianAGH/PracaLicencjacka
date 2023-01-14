using System;
using Xamarin.Forms.Maps;

namespace DriverControllApp.Models
{
    public class Base
    {
        Position baseLocation = new Position(50.065364, 19.923704);

        public Position GetBaseLocation()
        {
            return baseLocation;
        }

        public void SetBaseLocation(Position value)
        {
            baseLocation = value;
        }
    }
}

