using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR
{
    public partial class Startup
    {
        private void ConfigurePdfService(IServiceCollection services)
        {
            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            string path = @"C:\" + "libwkhtmltox.dll";
            context.LoadUnmanagedLibrary(path);
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        }
    }
}
