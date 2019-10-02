using AspergillosisEPR.Models.SGRQContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspergillosisEPR.Data
{
    public class SGRQContext : DbContext
    {
        public DbSet<QuestionnaireResult> QuestionnaireResults { get; set; }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<Patient> Patients { get; set; }


    public SGRQContext(DbContextOptions<SGRQContext> options) : base(options)
        {           
        }
    }
}
