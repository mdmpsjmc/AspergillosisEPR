
using System;
using System.Runtime.Serialization;

namespace AspergillosisEPR.Geodesy
{
 
    [Serializable]
    public struct Angle : IComparable<Angle>, IComparable, IEquatable<Angle>, ISerializable
    {
        public static readonly Angle Zero = new Angle(0);
        public static readonly Angle Angle180 = Angle.FromRadians(Math.PI);
        public static readonly Angle NaN = Angle.FromRadians(Double.NaN);
        private const double PiOver180 = Math.PI / 180;

        private Angle(double radians)
        {
            this.Radians = radians;
        }

        /// <summary>
        /// Get angle measured in degrees.
        /// </summary>
        public double Degrees => this.Radians / PiOver180;

      
        public double Radians { get; }

        public static Angle FromRadians(double radians) => new Angle(radians);
        public static Angle FromDegrees(double degrees) => new Angle(degrees * PiOver180);

        public static Angle FromDegreesAndMinutes(int degrees, double minutes)
        {
            double d = minutes / 60;
            d = degrees < 0 ? degrees - d : degrees + d;

            return new Angle(d * PiOver180);
        }

        public static Angle FromDegreesMinutesAndSeconds(int degrees, int minutes, double seconds)
        {
            double d = (seconds / 3600) + (minutes / 60.0);
            d = degrees < 0 ? degrees - d : degrees + d;

            return new Angle(d * PiOver180);
        }

   
        public static Angle Abs(Angle angle) => new Angle(Math.Abs(angle.Radians));
        public static bool IsNaN(Angle angle) => Double.IsNaN(angle.Radians);
        public static string ToString(Angle value) => $"Angle[Degrees={value.Degrees}, Radians={value.Radians}]";

        public int CompareTo(object obj)
        {
            if (!(obj is Angle))
            {
                throw new ArgumentException("Can only compare Angles with other Angles.", nameof(obj));
            }

            return this.CompareTo((Angle)obj);
        }

        public int CompareTo(Angle other) => this.Radians.CompareTo(other.Radians);
        public override int GetHashCode() => this.Radians.GetHashCode();
        public override bool Equals(object obj) => obj is Angle && this.Equals((Angle)obj);
        public bool Equals(Angle other) => this.Radians == other.Radians;
        public override string ToString() => ToString(this);

        #region Serialization / Deserialization

        private Angle(SerializationInfo info, StreamingContext context)
        {
            this.Radians = info.GetDouble("radians");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("radians", this.Radians);
        }

        #endregion

        #region Operators

        public static Angle operator +(Angle lhs, Angle rhs) => new Angle(lhs.Radians + rhs.Radians);
        public static Angle operator -(Angle lhs, Angle rhs) => new Angle(lhs.Radians - rhs.Radians);
        public static bool operator ==(Angle lhs, Angle rhs) => lhs.Radians == rhs.Radians;
        public static bool operator !=(Angle lhs, Angle rhs) => lhs.Radians != rhs.Radians;
        public static bool operator <(Angle lhs, Angle rhs) => lhs.Radians < rhs.Radians;
        public static bool operator <=(Angle lhs, Angle rhs) => lhs.Radians <= rhs.Radians;
        public static bool operator >(Angle lhs, Angle rhs) => lhs.Radians > rhs.Radians;
        public static bool operator >=(Angle lhs, Angle rhs) => lhs.Radians >= rhs.Radians;

        #endregion
    }
}