using AspergillosisEPR.Models;
using System;
using System.Linq;

namespace AspergillosisEPR.Data
{
    public static class AppDbInitializer
    {
           
        public static void Iniitalize(ApplicationDbContext context)
        {
                context.Database.EnsureCreated();
                if (context.Roles.Any())
                {
                    return;
                }

                var roles = new ApplicationRole[]
               {
            new ApplicationRole{Name="Read Role",   NormalizedName = "Read Role".ToUpper(), CreatedAt = DateTime.Now, Description="User with this role can read any information in database. By default is added to each newly created user"},
            new ApplicationRole{Name="Create Role", NormalizedName = "Create Role".ToUpper(), CreatedAt = DateTime.Now, Description="User with this role can add new items to database"},
            new ApplicationRole{Name="Update Role", NormalizedName = "Update Role".ToUpper(), CreatedAt = DateTime.Now, Description="User with this role can edit existing items in database"},
            new ApplicationRole{Name="Delete Role", NormalizedName = "Delete Role".ToUpper(), CreatedAt = DateTime.Now, Description="User with this role can remove items from database. Use with caution!"},
            new ApplicationRole{Name="Admin Role",  NormalizedName = "Admin Role".ToUpper(),CreatedAt = DateTime.Now, Description="User with this role can assign roles and edit roles for other users. They can also change any information in database in any way they want"},
               };
                foreach (ApplicationRole ar in roles)
                {
                    context.Roles.Add(ar);
                }
                var adminUser = new ApplicationUser()
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = "superadmin@example.net",
                    EmailConfirmed = true,
                    UserName = ApplicationUser.GenerateUsername("Super", "Admin")
                };
                context.Users.Add(adminUser);
                context.SaveChanges();
            }

        }
 }