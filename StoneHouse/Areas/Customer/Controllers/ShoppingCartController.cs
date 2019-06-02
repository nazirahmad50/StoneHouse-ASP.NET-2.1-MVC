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
    }
}