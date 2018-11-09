using AspergillosisEPR.Geodesy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Geodesy
{
    [Serializable]
    public struct GlobalCoordinates : IComparable<GlobalCoordinates>, IComparable, IEquatable<GlobalCoordinates>, ISerializable
    {
        private const double PiOver2 = Math.PI / 2;
        private const double TwoPi = Math.PI + Math.PI;
        private const double NegativePiOver2 = -PiOver2;
        private const double NegativeTwoPi = -TwoPi;


        public GlobalCoordinates(Angle latitude, Angle longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Canonicalize();
        }

        public Angle Latitude { get; private set; }

        public Angle Longitude { get; private set; }


        private void Canonicalize()
        {

            double latitudeRadians = this.Latitude.Radians;
            double longitudeRadians = this.Longitude.Radians;

            latitudeRadians = (latitudeRadians + Math.PI) % TwoPi;
            if (latitudeRadians < 0) latitudeRadians += TwoPi;
            latitudeRadians -= Math.PI;

            if (latitudeRadians > PiOver2)
            {
                latitudeRadians = Math.PI - latitudeRadians;
                longitudeRadians += Math.PI;
            }
            else if (latitudeRadians < NegativePiOver2)
            {
                latitudeRadians = -Math.PI - latitudeRadians;
                longitudeRadians += Math.PI;
            }

            longitudeRadians = ((longitudeRadians + Math.PI) % TwoPi);
            if (longitudeRadians <= 0) longitudeRadians += TwoPi;
            longitudeRadians -= Math.PI;

            this.Latitude = Angle.FromRadians(latitudeRadians);
            this.Longitude = Angle.FromRadians(longitudeRadians);
        }

        public static int GetHashCode(GlobalCoordinates value) => HashCodeBuilder.Seed
                                                                                 .HashWith(value.Longitude)
                                                                                 .HashWith(value.Latitude);

        public static bool Equals(GlobalCoordinates first, GlobalCoordinates second) => first.Latitude == second.Latitude &&
                                                                                        first.Longitude == second.Longitude;

        public static int Compare(GlobalCoordinates first, GlobalCoordinates second)
        {
            int a = first.Longitude.CompareTo(second.Longitude);

            return a == 0
                ? first.Latitude.CompareTo(second.Latitude)
                : a;
        }

        public static string ToString(GlobalCoordinates value) => $"GlobalCoordinates[Longitude={value.Longitude}, Latitude={value.Latitude}]";

        /// <summary>
        /// Get a hash code for these coordinates.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => GetHashCode(this);

        /// <summary>
        /// Compare these coordinates to another object for equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is GlobalCoordinates &&
                                                   Equals(this, (GlobalCoordinates)obj);


        public bool Equals(GlobalCoordinates other) => Equals(this, other);

        public int CompareTo(object obj)
        {
            if (!(obj is GlobalCoordinates))
            {
                throw new ArgumentException("Can only compare GlobalCoordinates with other GlobalCoordinates.", nameof(obj));
            }

            return Compare(this, (GlobalCoordinates)obj);
        }

        public int CompareTo(GlobalCoordinates other) => Compare(this, other);
        public override string ToString() => ToString(this);

        #region Serialization / Deserialization

        private GlobalCoordinates(SerializationInfo info, StreamingContext context)
        {
            double longitudeRadians = info.GetDouble("longitudeRadians");
            double latitudeRadians = info.GetDouble("latitudeRadians");

            this.Longitude = Angle.FromRadians(longitudeRadians);
            this.Latitude = Angle.FromRadians(latitudeRadians);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("longitudeRadians", this.Longitude.Radians);
            info.AddValue("latitudeRadians", this.Latitude.Radians);
        }

        #endregion

        #region Operators

        public static bool operator ==(GlobalCoordinates lhs, GlobalCoordinates rhs) => Equals(lhs, rhs);
        public static bool operator !=(GlobalCoordinates lhs, GlobalCoordinates rhs) => !Equals(lhs, rhs);
        public static bool operator <(GlobalCoordinates lhs, GlobalCoordinates rhs) => Compare(lhs, rhs) < 0;
        public static bool operator <=(GlobalCoordinates lhs, GlobalCoordinates rhs) => Compare(lhs, rhs) <= 0;
        public static bool operator >(GlobalCoordinates lhs, GlobalCoordinates rhs) => Compare(lhs, rhs) > 0;
        public static bool operator >=(GlobalCoordinates lhs, GlobalCoordinates rhs) => Compare(lhs, rhs) >= 0;

        #endregion
    }
}
