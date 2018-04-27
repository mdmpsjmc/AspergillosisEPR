
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SGRQProducer
{
    class Program
    {
        private static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json");

            configuration = builder.Build();
            string foo = configuration.GetSection("sgrqApiUrl").Value;
            Console.WriteLine(foo);
        }
    }
}
