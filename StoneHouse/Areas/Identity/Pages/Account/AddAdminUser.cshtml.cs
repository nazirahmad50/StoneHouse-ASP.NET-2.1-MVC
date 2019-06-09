using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoneHouse.Models;
using StoneHouse.Utility;

namespace StoneHouse.Areas.Identity.Pages.Account
{
    public class AddAdminUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        //we need the role manager to create roles
        //changes has to be made in the startup.cs file when using ' RoleManager<IdentityRole>'
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddAdminUserModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGet()
        {
            //create roles for our websites and create super Admin User
            //if the admin end user does not exist
            if (!await _roleManager.RoleExistsAsync(StaticDetails.AdminEndUser))
            {
                //create new admin end user role
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.AdminEndUser));

            }


            //if the super admin end user does not exist
            if (!await _roleManager.RoleExistsAsync(StaticDetails.SuperAdminEndUser))
            {
                //create new super admin end user role
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.SuperAdminEndUser));

                //after creating super admin user then create the admin user
                var userAdmin = new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    PhoneNumber = "1112223333",
                    Name = "Admin Ben"

                };

                var resultUser = await _userManager.CreateAsync(userAdmin, "Admin@123");
                await _userManager.AddToRoleAsync(userAdmin, StaticDetails.SuperAdminEndUser);



            }


            return Page();



        }
    }
}