using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Models;
using StoneHouse.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Data
{
    public class DBInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;

        }

        //seed database
        public async void Initialize()
        {
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                //it will create database schema if does not exist
                _db.Database.Migrate();

            }

            //check if user super admin exists and if it exists it will return back from this type and wont proccede furthur
            if (_db.Roles.Any(r => r.Name == StaticDetails.SuperAdminEndUser)) return;

            
            //If no roles exist then two roles
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.AdminEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.SuperAdminEndUser)).GetAwaiter().GetResult();

            //create a new user
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name = "Admin Bob",
                EmailConfirmed = true

            }, "Admin@123").GetAwaiter().GetResult();

            //this will assign super admin user based on the email of the user created which is 'admin@gmail.com'
            await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync("admin@gmail.com"), StaticDetails.SuperAdminEndUser);
        }
    }
}
