using AspergillosisEPR.Geodesy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Geodesy
{
    public class VincentysFormula
    {
        private Position _position1;
        private Position _position2;

        public VincentysFormula(Position position1, Position position2)
        {
            _position1 = position1;
            _position2 = position2;
        }

        public double CalculateDistance()
        {
            GeodeticCalculator geoCalc = new GeodeticCalculator();
            Ellipsoid reference = Ellipsoid.WGS84;
            double lat1 = (double) _position1.Latitude;
            double lon1 = (double) _position1.Longitude;

            GlobalCoordinates position1 = new GlobalCoordinates(Angle.FromDegrees(lat1), Angle.FromDegrees(lon1));

            double lat2 = (double)_position2.Latitude;
            double lon2 = (double)_position2.Longitude;

            GlobalCoordinates position2 = new GlobalCoordinates(Angle.FromDegrees(lat2), Angle.FromDegrees(lon2));

            GeodeticCurve geoCurve = geoCalc.CalculateGeodeticCurve(reference, position1, position2);
            double ellipseKilometers = geoCurve.EllipsoidalDistanceMeters / 1000;
            double ellipseMiles = ellipseKilometers * 0.621371192;

            return ellipseMiles;
        }        
    }
}
