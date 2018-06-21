using AspergillosisEPR.Models.PatientAdministrationSystem;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspergillosisEPR.Data
{
    public class PASDbContext : DbContext
    {
        public DbSet<LpiPatientData> LpiPatientData { get; set; }

        public PASDbContext(DbContextOptions<PASDbContext> options) : base(options)
        {           
        }
    }
}
