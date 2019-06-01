using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Data;
using StoneHouse.Extensions;
using StoneHouse.Models;

namespace StoneHouse.Controllers
{
    //identity that this controller resides in the area called Customer
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;

        }

        public async  Task<IActionResult> Index()
        {
            var productList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();

            return View(productList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(m=>m.Id==id).FirstOrDefaultAsync();


            return View(product);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            //'ssShoppingCart' is used to identify the session
            //The 'Get' is used from the 'SessionExtensions' class which includes a Get and a Set
            //the 'Get' is generic in SessionExtensions class so thats why int is given
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            //if the 'lstShoppingCart' is null then create a new list
            if (lstShoppingCart == null)
            {
                lstShoppingCart = new List<int>();
            }

            //setting the session 
            //add the id to the list
            lstShoppingCart.Add(id);

            //The 'Set' is used from the 'SessionExtensions' class which includes a Get and a Set
            //pass the value(lstShoppingCart) to the session 'ssShoppingCart'  
            //setting session variable
            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);

            //redirect to the index action insdie the Home controller
            //define the area as well
            return RedirectToAction("Index", "Home", new { area = "Customer" });

        }

        //remove based on the id because we are passing the id in the 'asp-roud-id' in the details view
        public IActionResult Remove(int id)
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstShoppingCart.Count > 0)
            {
                if (lstShoppingCart.Contains(id))
                {
                    lstShoppingCart.Remove(id);
                }
            }

            //need to set teh session again
            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);

            return RedirectToAction(nameof(Index));



        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
