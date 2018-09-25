using System;

namespace AspergillosisEPR.Lib
{
    public enum DistanceType { Miles, Kilometers };

    public struct Position
    {
        public decimal Latitude;
        public decimal Longitude;
    }

    class Haversine
    {
        public double Distance(Position pos1, Position pos2, DistanceType type)
        {
            double R = (type == DistanceType.Miles) ? 3960 : 6371;

            double dLat = toRadian((double) (pos2.Latitude - pos1.Latitude));
            double dLon = toRadian((double) (pos2.Longitude - pos1.Longitude));

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(toRadian((double) pos1.Latitude)) * Math.Cos(toRadian((double) pos2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;

            return d;
        }

        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}