using RabbitConsumers.PatientAdministrationSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitConsumers.DbFactories
{
    class PASConnectionFactory : IDesignTimeDbContextFactory<PASDbContext>
    {
        private static string _connectionString;

        public PASDbContext CreateDbContext()
        {
            return CreateDbContext(null);
        }

        private PASDbContext CreateDbContext(object p)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                LoadConnectionString();
            }

            var builder = new DbContextOptionsBuilder<PASDbContext>();
            builder.UseSqlServer(_connectionString);

            return new PASDbContext(builder.Options);
        }

        PASDbContext IDesignTimeDbContextFactory<PASDbContext>.CreateDbContext(string[] args)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                LoadConnectionString();
            }

            var builder = new DbContextOptionsBuilder<PASDbContext>();
            builder.UseSqlServer(_connectionString);

            return new PASDbContext(builder.Options);
        }

        private static void LoadConnectionString()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build();

            _connectionString = configuration.GetConnectionString("PASConnection");
        }
    }
}
