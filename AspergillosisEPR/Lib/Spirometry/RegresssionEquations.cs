using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Spirometry
{
    public static class RegressionEquations
    {
        public static List<Equation> Males { get; set; } = new List<Equation>
        {
            new Equation("6.10H - 0.028A - 4.65", "IVC"),
            new Equation("5.76H - 0.026A - 4.34", "FVC"),
            new Equation("7.99H -7.08", "TLC"),
            new Equation("1.31H + 0.022A - 1.23", "RV"),
            new Equation("2.34H + 0.009A - 1.09", "FRC"),
            new Equation("0.39A + 13.96", "RV/TLC"),
            new Equation("0.21A + 43.8", "FRC/TLC"),
            new Equation("4.30H - 0.029A -2.49", "FEV1"),
            new Equation("-0.18A + 87.21", "FEV1/VC"),
            new Equation("1.94H - 0.043A + 2.70", "FEF 25-75%"),
            new Equation("6.14H - 0.043A + 0.15", "PEF"),
            new Equation("5.46H - 0.029A - 0.47", "FEF 25%"),
            new Equation("3.79H - 0.031A - 0.35", "FEF 50%"),
            new Equation("0.109H *100 - 0.067A - 5.812", "DLCO"),
            new Equation("2.61H - 0.026A - 1.34", "FEF 75%"),            
        };
        public static List<Equation> Females { get; set; } = new List<Equation>
        {
            new Equation("4.66H - 0.026A - 3.28", "IVC"),
            new Equation("4.43H - 0.026A -2.89", "FVC"),
            new Equation("6.60H - 5.79", "TLC"),
            new Equation("1.81H + 0.016A-2.00", "RV"),
            new Equation("2.24H + 0.001A - 1.00", "FRC"),
            new Equation("0.34A + 18.96", "RV/TLC"),
            new Equation("0.10A + 45.1", "FRC/TLC"),
            new Equation("3.95H - 0.025A - 2.60", "FEV1"),
            new Equation("-0.19A + 89.10", "FEV1/VC"),
            new Equation("1.25H - 0.034A + 2.92", "FEF 25-75%"),
            new Equation("5.50H - 0.030A - 1.11", "PEF"),
            new Equation("3.221H - 0.025A + 1.60", "FEF 25%"),
            new Equation("2.45H - 0.025A + 1.16", "FEF 50%"),
            new Equation("1.05H - 0.025A + 1.11", "FEF 75%"),
            new Equation("0.078H  *100 - 0.04A - 3.858", "DLCO"),
        };
    }
}
