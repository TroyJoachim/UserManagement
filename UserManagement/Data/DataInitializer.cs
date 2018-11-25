using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserManagement.Models;
using UserManagement.Data;

namespace UserManagement.Data
{
    public static class DataInitializer
    {
        public static async void SeedRolesAsync(RoleManager<Role> roleManager)
        {
            if (roleManager.RoleExistsAsync("Administrator").Result)
                return;

            var role = new Role
            {
                Name = "Administrator",
                Description = "Perform all the operations."
            };
            await roleManager.CreateAsync(role);
        }

        public static async void SeedRoleClaimsAsync(RoleManager<Role> roleManager)
        {
            var role = await roleManager.FindByNameAsync("Administrator");

            foreach (var claim in AllClaims.GetList())
            {
                await roleManager.AddClaimAsync(role, new Claim(claim, ""));
            }
        }

        public static async void SeedUsersAsync(UserManager<User> userManager)
        {
            var user = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                Enabled = true
            };
            var result = await userManager.CreateAsync(user, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Administrator");
            }
        }

        public static void SeedData(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            SeedRolesAsync(roleManager);
            SeedRoleClaimsAsync(roleManager);
            SeedUsersAsync(userManager);
        }
    }

}
