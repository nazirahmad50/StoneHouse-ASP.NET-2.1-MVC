using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Data;
using StoneHouse.Models;
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

        //display 3 appointments per page
        private int pageSize = 3;

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        //receieve this parameters if the user eneters it in the view
        //productPage will be set to 1 by default
        public IActionResult Index(int productPage=1, string searchName=null, string searchEmail=null, string searchPhone=null, string searchDate=null)
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

            //-------------------------
            //Pagination: build url
            //-------------------------

            StringBuilder param = new StringBuilder();

            //need to make sure that whatever parameter is passed for teh search criteria still remains in the other pages
            //the ':' will be replaced with the page number which is done in the PageLinkTagHelper
            param.Append("/Admi/Appointments?productPage=:");

            param.Append("&searchName");
            if (searchName!= null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail");
            if (searchName != null)
            {
                param.Append(searchEmail);
            }

            param.Append("&searchPhone");
            if (searchName != null)
            {
                param.Append(searchPhone);
            }
               param.Append("&searchDate");
            if (searchName != null)
            {
                param.Append(searchDate);
            }




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

            //count how many appoitnments we have after the search criteria
            var count = appointmentVM.Appointments.Count;

            /* for e.g. there is 5 total appoitnemtns and the page size is 3
            So the count variable will be five
            and say the productPage(current page) is 1 then -1 it will be 0, multiply by 3 it will then 'Skip' 0 and 'Take' first 3 (pageSize)
             */
            appointmentVM.Appointments = appointmentVM.Appointments.OrderBy(p => p.AppointmentDate)
                .Skip((productPage - 1) * pageSize)
                .Take(pageSize).ToList();

            appointmentVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = pageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            return View(appointmentVM);
        }

        //GET:Edit
        public IActionResult Edit(int? id)
        {
            if (id != null)
            {

                //retrieve list of products
                //join products and ProductsSelectedForAppointment on where their id is equal
                //filter based on where (where) the appointment id is equal to the products id
                //select all the products which is p
                //aslo include the product types
                var productLst = (IEnumerable<Products>)(from p in _db.Products
                                                       join a in _db.ProductsSelectedForAppointment
                                                       on p.Id equals a.ProductId
                                                       where a.AppointmentId == id
                                                       select p).Include("ProductTypes");

                AppointmentDetailViewModel objAppointmentVM = new AppointmentDetailViewModel()
                {
                    Appointments = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                    SalesPerson = _db.ApplicationUser.ToList(),
                    Products = productLst.ToList()

                };

            return View(objAppointmentVM);


            }
            else
            {
                return NotFound();
            }
        }

        //POST:Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(int id, AppointmentDetailViewModel objAppointmentVM)
        {
            if (ModelState.IsValid)
            {
                //add appointment hours and minutes to the AppointmentDate
                objAppointmentVM.Appointments.AppointmentDate = objAppointmentVM.Appointments.AppointmentDate
                                                                   .AddHours(objAppointmentVM.Appointments.AppointmentTime.Hour)
                                                                   .AddMinutes(objAppointmentVM.Appointments.AppointmentTime.Minute);

                //load appointments from database based on the appointment id inside the appointment View Model
                var appointmentFromDB = _db.Appointments.Where(a => a.Id == objAppointmentVM.Appointments.Id).FirstOrDefault();

                //update the appoitnments database fields to changes made in the objAppointmentVM which are from the view
                appointmentFromDB.CustomerName = objAppointmentVM.Appointments.CustomerName;
                appointmentFromDB.CustomerEmail = objAppointmentVM.Appointments.CustomerEmail;
                appointmentFromDB.CustomerPhone = objAppointmentVM.Appointments.CustomerPhone;
                appointmentFromDB.AppointmentDate = objAppointmentVM.Appointments.AppointmentDate;
                appointmentFromDB.isConfirmed = objAppointmentVM.Appointments.isConfirmed;

                //only the super admin can update the salesperson or assign them
                if (User.IsInRole(StaticDetails.SuperAdminEndUser))
                {
                    appointmentFromDB.SalesPersonId = objAppointmentVM.Appointments.SalesPersonId;
                }

                _db.SaveChanges();

                return RedirectToAction(nameof(Index));

            }
            else
            {
                return View(objAppointmentVM);


            }
        }


        //GET:Details
        public IActionResult Details(int? id)
        {
            if (id != null)
            {

                //retrieve list of products
                //join products and ProductsSelectedForAppointment on where their id is equal
                //filter based on where (where) the appointment id is equal to the products id
                //select all the products which is p
                //aslo include the product types
                var productLst = (IEnumerable<Products>)(from p in _db.Products
                                                         join a in _db.ProductsSelectedForAppointment
                                                         on p.Id equals a.ProductId
                                                         where a.AppointmentId == id
                                                         select p).Include("ProductTypes");

                AppointmentDetailViewModel objAppointmentVM = new AppointmentDetailViewModel()
                {
                    Appointments = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                    SalesPerson = _db.ApplicationUser.ToList(),
                    Products = productLst.ToList()

                };

                return View(objAppointmentVM);


            }
            else
            {
                return NotFound();
            }
        }

        //GET:Delete
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {

                //retrieve list of products
                //join products and ProductsSelectedForAppointment on where their id is equal
                //filter based on where (where) the appointment id is equal to the products id
                //select all the products which is p
                //aslo include the product types
                var productLst = (IEnumerable<Products>)(from p in _db.Products
                                                         join a in _db.ProductsSelectedForAppointment
                                                         on p.Id equals a.ProductId
                                                         where a.AppointmentId == id
                                                         select p).Include("ProductTypes");

                AppointmentDetailViewModel objAppointmentVM = new AppointmentDetailViewModel()
                {
                    Appointments = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                    SalesPerson = _db.ApplicationUser.ToList(),
                    Products = productLst.ToList()

                };

                return View(objAppointmentVM);


            }
            else
            {
                return NotFound();
            }
        }

        //POST:Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppointment(int? id)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
 