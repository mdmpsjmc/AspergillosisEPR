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
using System.Threading.Tasks;
using Audit.Core;

namespace AspergillosisEPR
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AspergillosisContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.UseRowNumberForPaging()));
            services.AddDbContext<ApplicationDbContext>(options =>
                       options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.UseRowNumberForPaging())
           );
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<PatientViewModel>();
            services.AddMvc();

            Audit.Core.Configuration.Setup()
                .UseSqlServer(config => config
                .ConnectionString(Configuration.GetConnectionString("DefaultConnection"))
                .Schema("dbo")
                .TableName("AuditEvents")
                .IdColumnName("ID")
                .JsonColumnName("Data")
                .LastUpdatedColumnName("LastUpdatedDate"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
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
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            await CreateRoles(app);
        }

        private async Task CreateRoles(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var RoleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                var UserManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roles = new ApplicationRole[]
                  {
                    new ApplicationRole{Name="Read Role",   NormalizedName = "Read Role".ToUpper(), CreatedAt = DateTime.Now, Description="User with this role can read any information in database. By default is added to each newly created user"},
                    new ApplicationRole{Name="Create Role", NormalizedName = "Create Role".ToUpper(), CreatedAt = DateTime.Now, Description="User with this role can add new items to database"},
                    new ApplicationRole{Name="Update Role", NormalizedName = "Update Role".ToUpper(), CreatedAt = DateTime.Now, Description="User with this role can edit existing items in database"},
                    new ApplicationRole{Name="Delete Role", NormalizedName = "Delete Role".ToUpper(), CreatedAt = DateTime.Now, Description="User with this role can remove items from database. Use with caution!"},
                    new ApplicationRole{Name="Admin Role",  NormalizedName = "Admin Role".ToUpper(),CreatedAt = DateTime.Now, Description="User with this role can assign roles and edit roles for other users. They can also change any information in database in any way they want"},
                  };
                IdentityResult roleResult;

                foreach (var role in roles)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(role.Name);
                    if (!roleExist)
                    {
                        roleResult = await RoleManager.CreateAsync(role);
                    }
                }
                var _user = await UserManager.FindByEmailAsync("superadmin@example.net");

                // check if the user exists
                if (_user == null)
                {
                    var adminUser = new ApplicationUser()
                    {
                        FirstName = "Super",
                        LastName = "Admin",
                        Email = "superadmin@example.net",
                        EmailConfirmed = true,
                        UserName = ApplicationUser.GenerateUsername("Super", "Admin")
                    };
                    string adminPassword = "P@$$w0rd212";

                    var createPowerUser = await UserManager.CreateAsync(adminUser, adminPassword);
                    if (createPowerUser.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(adminUser, "Admin Role");
                    }
                }
            }
        }

    }

}
