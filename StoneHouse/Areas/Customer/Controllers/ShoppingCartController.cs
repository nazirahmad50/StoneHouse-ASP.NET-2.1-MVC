using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoneHouse.Data;
using StoneHouse.Extensions;
using StoneHouse.Models;
using StoneHouse.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace StoneHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {

        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };

        }

        //Get: Index Shopping Cart
        public  IActionResult Index()
        {
            //get the list of items in the shopping cart
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstShoppingCart.Count > 0)
            {
                //loop through each cart item and add it to the products list inside the ShoppingCartViewModel
                foreach (int cartItem in lstShoppingCart)
                {
                    Products prod = _db.Products.Include(p=>p.SpecialTags).Include(p=>p.ProductTypes).Where(p => p.Id == cartItem).FirstOrDefault();
                    ShoppingCartVM.Products.Add(prod);
                }
            }

            return View(ShoppingCartVM);
        }


        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost()
        {
            //retireve the list of items displayed in the shopping cart
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            //Merge both the appointment date and time inside the AppointmentDate
            ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate
                                                            .AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour)
                                                            .AddHours(ShoppingCartVM.Appointments.AppointmentTime.Minute);

            //create Appointments and add it to the database
            Appointments appointments = ShoppingCartVM.Appointments;
            _db.Appointments.Add(appointments);
            _db.SaveChanges();

            //get the appoappointments id
            int appointmentId = appointments.Id;

            //loop through teh items in teh cart
            foreach(int productId in lstCartItems)
            {
                //create an object of 'ProductsSelectedForAppointment' each time
                ProductsSelectedForAppointment ProductsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };
                //add it to the database 
                _db.ProductsSelectedForAppointment.Add(ProductsSelectedForAppointment);
            }
            //after the loop is finished save changes to databse
            _db.SaveChanges();

            //empty the list 'lstCartItems'
            lstCartItems = new List<int>();
            //set the session so it can be emptied
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Remove(int id)
        {
            //retireve the list of items displayed in the shopping cart
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstCartItems.Count > 0)
            {
                if (lstCartItems.Contains(id))
                {
                    lstCartItems.Remove(id);
                }
            }

            //set the session so it can be empty
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);
            return RedirectToAction(nameof(Index));

        }


    }
}