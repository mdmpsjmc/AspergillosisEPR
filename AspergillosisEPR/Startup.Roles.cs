using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR
{
    public partial class Startup
    {
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

        private async Task CreateReportingRole(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var RoleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                var role = new ApplicationRole
                {
                    Name = "Reporting Role",
                    NormalizedName = "Reporting Role".ToUpper(),
                    CreatedAt = DateTime.Now,
                    Description = "User with this role have access to system reporting section"
                };
                var roleExist = await RoleManager.RoleExistsAsync(role.Name);
                if (!roleExist)
                {
                    IdentityResult roleResult = await RoleManager.CreateAsync(role);
                }
            }
        }

        private async Task CreateAnonymousRole(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var RoleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                var role = new ApplicationRole
                {
                    Name = "Anonymous Role",
                    NormalizedName = "Anonymous Role".ToUpper(),
                    CreatedAt = DateTime.Now,
                    Description = "User with this role can read information in database in an anonymised way where no personal information is revealed."
                };
                var roleExist = await RoleManager.RoleExistsAsync(role.Name);
                if (!roleExist)
                {
                    IdentityResult roleResult = await RoleManager.CreateAsync(role);
                }
            }
        }
    }
}
