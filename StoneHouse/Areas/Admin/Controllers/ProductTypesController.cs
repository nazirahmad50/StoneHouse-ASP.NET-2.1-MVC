using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoneHouse.Data;
using StoneHouse.Models;
using StoneHouse.Utility;

namespace StoneHouse.Controllers
{
    //set authorization as only the role SuperAdminEndUser will be able to access these controllers or views
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;

        }

        public IActionResult Index()
        {
            //convert whatever is inside teh database to a list
            return View(_db.ProductTypes.ToList());
        }

        //GET: Create Action Method
        public IActionResult Create()

        {
            return View();

        }

        //GET: Edit Action Method
        public async Task<IActionResult> Edit(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var productType = await _db.ProductTypes.FindAsync(id);
                if (productType == null)
                {
                    return NotFound();

                }
                else
                {
                    return View(productType);
                }

            }

        }

        //GET: Details Action Method
        public async Task<IActionResult> Details(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var productType = await _db.ProductTypes.FindAsync(id);
                if (productType == null)
                {
                    return NotFound();

                }
                else
                {
                    return View(productType);
                }

            }

        }

        //POST: Create Action Method
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            //For e.g. the name field in the ProductTypes model is required and if a new product is created and this field is left empty then the modelstate will return invalid
            if (ModelState.IsValid)
            {
                _db.Add(productTypes);
                //whenever the asycn method is used then teh await keyword has to be used in front of it
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                //This way an error can be shown 
                return View(productTypes);

            }

        }

        //POST: Edit Action Method
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int id,ProductTypes productTypes)
        {
            if (id!= productTypes.Id)
            {
                return NotFound();
            }
            else
            {
                //For e.g. the name field in the ProductTypes model is required and if a new product is created and this field is left empty then the modelstate will return invalid
                if (ModelState.IsValid)
                {
                    _db.Update(productTypes);
                    //whenever the asycn method is used then teh await keyword has to be used in front of it
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    //This way an error can be shown 
                    return View(productTypes);

                }

            }
           

        }

        //GET: Delete Action Method
        public async Task<IActionResult> Delete(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var productType = await _db.ProductTypes.FindAsync(id);
                if (productType == null)
                {
                    return NotFound();

                }
                else
                {
                    return View(productType);
                }

            }

        }

        //POST: Delete Action Method
        [HttpPost, ActionName("Delete")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productTypeToDelete = await _db.ProductTypes.FindAsync(id);
            _db.ProductTypes.Remove(productTypeToDelete);      
             //whenever the asycn method is used then teh await keyword has to be used in front of it
             await _db.SaveChangesAsync();
             return RedirectToAction(nameof(Index));

            
         

        }
    }
}