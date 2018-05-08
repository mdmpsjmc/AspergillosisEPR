
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitConsumers.DbFactories
{
    public class AspergillosisContextFactory : IDesignTimeDbContextFactory<AspergillosisContext>
    {
        private static string _connectionString;

        public AspergillosisContext CreateDbContext()
        {
            return CreateDbContext(null);
        }

        private AspergillosisContext CreateDbContext(object p)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                LoadConnectionString();
            }

            var builder = new DbContextOptionsBuilder<AspergillosisContext>();
            builder.UseSqlServer(_connectionString);

            return new AspergillosisContext(builder.Options);
        }

        AspergillosisContext IDesignTimeDbContextFactory<AspergillosisContext>.CreateDbContext(string[] args)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                LoadConnectionString();
            }

            var builder = new DbContextOptionsBuilder<AspergillosisContext>();
            builder.UseSqlServer(_connectionString);

            return new AspergillosisContext(builder.Options);
        }

        private static void LoadConnectionString()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

       
    }
}
