using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed
{
    public static class AppDbInitializer
    {

        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

        }


    }

    }