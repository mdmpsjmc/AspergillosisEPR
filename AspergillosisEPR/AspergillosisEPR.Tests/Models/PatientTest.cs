using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspergillosisEPR.AspergillosisEPR.Tests.Models
{
    public class PatientTest
    {

        [Fact]
        public void WhenPatientIsDead_AgeIsCalculatedFromDateOfDeath()
        {
            var patient = new Patient { DOB=DateTime.Parse("15/12/2001"),
                                        DateOfDeath = DateTime.Parse("15/12/2017")
                                        };
            Assert.True(patient.IsDeceased());
            Assert.Equal(16, patient.Age());
        }

        [Fact] 
        public void WhenPatientIsAlive_AgeIsCalculatedFromCurrentDate()
        {
            var currentYear = DateTime.Now.Year;
            var patient = new Patient
            {
                DOB = DateTime.Parse("15/12/2001"),
            };
            var expectedAge = currentYear - patient.DOB.Year;
            if (DateTime.Now.DayOfYear < patient.DOB.DayOfYear) expectedAge = expectedAge - 1;
            Assert.False(patient.IsDeceased());
            Assert.Equal(expectedAge, patient.Age());
            Assert.Null(patient.DateOfDeath);
        }
    }
}
