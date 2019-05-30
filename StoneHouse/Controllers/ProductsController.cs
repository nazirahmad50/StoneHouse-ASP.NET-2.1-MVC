using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Data;
using StoneHouse.Models;
using StoneHouse.Models.ViewModel;

namespace StoneHouse.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;

        //Whenever you are posting or retrieving anything the 'BindProperty' will automatically bidn htis view model
        //to it and you dont have to put it in the method parameters 
        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext db)
        {
            _db = db;

            //initiled the constructor for the productsViewModl controller
            ProductsVM = new ProductsViewModel()
            {

                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                Products = new Models.Products()
            };

        }


        public async  Task<IActionResult> Index()
        {
            //add the producttypes and special tags to the products database
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }
    }
}