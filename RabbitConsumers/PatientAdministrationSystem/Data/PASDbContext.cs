using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitConsumers.PatientAdministrationSystem.Data
{
    class PASDbContext  : DbContext
    {
        public PASDbContext(DbContextOptions<PASDbContext> options) : base(options)
        {
            public DbSet<LpiPatientData> LpiPatientData { get; set; }
        }

    }
}
