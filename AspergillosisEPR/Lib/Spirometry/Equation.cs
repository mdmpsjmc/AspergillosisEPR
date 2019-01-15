using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AspergillosisEPR.Lib.Spirometry
{
    public class Equation
    {
        public string PFT { get; set; }
        public string Formula { get; set; }

        public Equation(string formula, string pft)
        {
            PFT = pft;
            Formula = formula;
        }

        public double Calculate(int age, double height)
        {
            var formula = Formula.Replace("A", "* "+ age.ToString()).Replace("H", " * "+height);
            return CSharpScript.EvaluateAsync<double>(formula).Result;
        }

        public double Percentage(int age, double height, double orignalValue)
        {
            var current = Calculate(age, height);
            return (orignalValue / current) * 100;
        }
    }
}
