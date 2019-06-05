using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Data;
using StoneHouse.Models.ViewModel;
using StoneHouse.Utility;

namespace StoneHouse.Areas.Admin.Controllers
{
    //add multiple authorization roles to this controller
    [Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    { 
        private readonly ApplicationDbContext _db;

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        //receieve this parameters if the user eneters it in the view
        public async Task<IActionResult> Index(string searchName=null, string searchEmail=null, string searchPhone=null, string searchDate=null)
        {
            //identityfy the current user that is logged in
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;

            //get the claim value
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            //instantiate the AppointmentViewModel
            AppointmentViewModel appointmentVM = new AppointmentViewModel()
            {
                //set the Appointments list in the AppointmentViewModel
                //to the list of the Appointments model
                Appointments = new List<Models.Appointments>()
            };

            //populate the virtual applicaiton user (SalesPerson) in the Appointments model class 
            //with application users from the database based on the 'salesPersonId'
            //thats why 'Include' is used
            appointmentVM.Appointments = _db.Appointments.Include(a => a.SalesPerson).ToList();

            //check if the logged in user role is of admin end user
            //AdminEndUser is basically a sales person
            if (User.IsInRole(StaticDetails.AdminEndUser))
            {
                //check if the current user logged in id (claim.Value) is equal to the 'SalesPersonId' in the database
                //the 'claim.Value' is the current user logged in id
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();
            }

            //--------------------------------------
            //Search functionality
            //--------------------------------------

            if (searchName!= null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (searchEmail != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerPhone.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {
                //try to convert this string into an integer
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);

                    appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();

                }
                catch (Exception ex)
                {

                }

            }


            return View(appointmentVM);
        }
    }
}