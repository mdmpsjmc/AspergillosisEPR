using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.AspergillosisViewModels;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Identity;
using AspergillosisEPR.Services;
using System;
using Audit.Core;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Features;
using static AspergillosisEPR.Services.ViewToString;
using AspNetCore.RouteAnalyzer;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using AspergillosisEPR.BackgroundTasks;
using System.Runtime.Loader;
using System.Reflection;
using Microsoft.Extensions.Logging;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;
using System.Linq;

namespace AspergillosisEPR
{
    public partial class Startup
    {
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment { get; }
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var physicalProvider = HostingEnvironment.ContentRootFileProvider;
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            var compositeProvider = new CompositeFileProvider(physicalProvider, embeddedProvider);
            var scopeFactory = services.BuildServiceProvider().GetServices<IServiceScopeFactory>().FirstOrDefault();
            var serviceProv = services.BuildServiceProvider().GetServices<IServiceProvider>().FirstOrDefault();
            var logger = services.BuildServiceProvider().GetServices<ILogger<AllEmptyPostCodesUpdateNowTask>>().FirstOrDefault();

            services.AddSingleton<IFileProvider>(compositeProvider);
            services.AddSingleton<IConfiguration>(Configuration);
            ConfigureContexts(services);
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 100000000;
            });

            // Add application services.

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<PatientViewModel>();
            services.AddMvc();
            services.AddRouteAnalyzer();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.MaxDepth = 1;
            });
            Audit.Core.Configuration.Setup()
                .UseSqlServer(config => config
                .ConnectionString(Configuration.GetConnectionString("DefaultConnection"))
                .Schema("dbo")
                .TableName("AuditEvents")
                .IdColumnName("ID")
                .JsonColumnName("Data")
                .LastUpdatedColumnName("LastUpdatedDate"));
            ConfigurePdfService(services);
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddSingleton<IHostedService, PatientAdministrationSystemStatusTask>(); //sunday
            services.AddSingleton<IHostedService, PatientVoriconazoleLevelBackgruondTask>();//monday
            services.AddSingleton<IHostedService, ImmunoglobulinUpdateBackgroundTask>(); //runs tuesday
            services.AddSingleton<IHostedService, EmptyPostCodesUpdateScheduledTask>(); //runs wednesday
            services.AddSingleton<IHostedService, PatientTestResultBackgroundUpdateTask>();//thursday       
            services.AddSingleton<IHostedService, PatientRadiologyUpdateBackgroundTask>();//friday       
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }

        private void ConfigureContexts(IServiceCollection services)
        {
            services.AddDbContext<AspergillosisContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), 
                                             b => b.UseRowNumberForPaging()));
            services.AddDbContext<ApplicationDbContext>(options =>
                       options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), 
                                            b => b.UseRowNumberForPaging()));
            services.AddDbContext<PASDbContext>(options =>
                       options.UseSqlServer(Configuration.GetConnectionString("PASConnection"), 
                                            b => b.UseRowNumberForPaging()));
            services.AddDbContext<ExternalImportDbContext>(options =>
                       options.UseSqlServer(Configuration.GetConnectionString("ImportDbConnection"), 
                                                b => b.UseRowNumberForPaging())
           );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, 
                                    Microsoft.AspNetCore.Hosting.IHostingEnvironment env, 
                                    IServiceProvider serviceProvider,
                                    IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRouteAnalyzer("/routes");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                     name: "AnonPatients",
                     template: "Anonymous/Patients/{action}/{id?}"
                 );
            });
            await CreateRoles(app);
            await CreateAnonymousRole(app);
            await CreateReportingRole(app);
        }        

        internal class CustomAssemblyLoadContext : AssemblyLoadContext
        {
            public IntPtr LoadUnmanagedLibrary(string absolutePath)
            {
                return LoadUnmanagedDll(absolutePath);
            }
            protected override IntPtr LoadUnmanagedDll(String unmanagedDllName)
            {
                return LoadUnmanagedDllFromPath(unmanagedDllName);
            }
      
            protected override Assembly Load(AssemblyName assemblyName)
            {
                throw new NotImplementedException();
            }

        }

    }

}
