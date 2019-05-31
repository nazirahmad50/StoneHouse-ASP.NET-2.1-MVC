using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Data;
using StoneHouse.Models;
using StoneHouse.Models.ViewModel;
using StoneHouse.Utility;

namespace StoneHouse.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _HostingEnvironment;


        //Whenever you are posting or retrieving anything the 'BindProperty' will automatically bidn htis view model
        //to it and you dont have to put it in the method parameters 
        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext db, HostingEnvironment HostingEnvironment)
        {
            _db = db;
            _HostingEnvironment = HostingEnvironment;

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

        //-----------------------------GET/POST METHODS---------------------------

        //GET: Products Create
        public IActionResult Create()
        {
            //The 'ProductsVM' is passed becasue in the create view a drop down list will be used ot display all the ProductTpes and SpecialTags
            return View(ProductsVM);

        }

        //Post:Products Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        //If the BindProperty was not used with 'ProductsViewModel' then the 'CreatePOST' method would look like this 'CreatePOST(ProductsViewModel ProductsVM)'
        public async Task<IActionResult> CreatePOST()
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(ProductsVM.Products);
                await _db.SaveChangesAsync();

                //Image being saved

                //This will retirve the path of 'wwwroot' which can be seen in the Solution Explorer
                string webRootPath = _HostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var ProductsFromDB = _db.Products.Find(ProductsVM.Products.Id);

                if (files.Count != 0)
                {
                    //Image has been upladoed

                    //exact locaiton of the imagefolder inside the wwwroot and then images folder
                    string uplaods = Path.Combine(webRootPath, StaticDetails.ImageFolder);
                    //find the extension of the file once inside taht folder
                    var extension = Path.GetExtension(files[0].FileName);

                    //copy the file from the upladoed to the server and rename it
                    //THe second parameter for 'Path.Combine' will be the name of teh file
                    //so the file name will be named to the Products id and with extension
                    using (var filestream = new FileStream(Path.Combine(uplaods, ProductsVM.Products.Id+extension),FileMode.Create))
                        {
                        files[0].CopyTo(filestream);

                        }

                    //In the products image property the exact path for where the image is saved on the server will be there
                    ProductsFromDB.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + extension;


                }
                else
                {
                    //When user does not upload iamge

                    //The uplaods var will hold the exact name of the default image
                    var uplaods = Path.Combine(webRootPath, StaticDetails.ImageFolder + @"\" + StaticDetails.DefaultProductImg);

                    //copy the iamge from server so the iamge can have the exact name of the default image
                    //the 'uploads' is the original locaiton and the second parameter is the destination
                    System.IO.File.Copy(uplaods, webRootPath + @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + ".jpg");

                    //update the ProductsFromDB Image folder

                    ProductsFromDB.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + ".jpg";

                }


                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            else
            {
                return View(ProductsVM);
            }

        }


    }
}