using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoneHouse.Data;
using StoneHouse.Models;

namespace StoneHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUsersController : Controller
    {

        private readonly ApplicationDbContext _db;


        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;

        }

        public IActionResult Index()
        {
            return View(_db.ApplicationUser.ToList());
        }

        //GET: Edit
        //the id is string because it will have strings and numbers in the database
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }
            else
            {
                var userFromDb = await _db.ApplicationUser.FindAsync(id);

                if (userFromDb != null)
                {
                    return View(userFromDb);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        //GET: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, ApplicationUser applicationUser)
        {
            if (id == applicationUser.Id)
            {
                if (ModelState.IsValid)
                {
                    //load the user fro mdatabase based on their id being equal to the id passed into the parameter from the View
                    ApplicationUser userFromDb = _db.ApplicationUser.Where(a => a.Id == id).FirstOrDefault();
                    //set the fields in the ApplicationUser database
                    userFromDb.Name = applicationUser.Name;
                    userFromDb.PhoneNumber = applicationUser.PhoneNumber;

                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    return View(applicationUser);
                }
            }
            else
            {
                return NotFound();
            }
            

          

        }


        //GET: Delete
        //the id is string because it will have strings and numbers in the database
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }
            else
            {
                var userFromDb = await _db.ApplicationUser.FindAsync(id);

                if (userFromDb != null)
                {
                    return View(userFromDb);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        //GET: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(string id)
        {
         
              //load the user fro mdatabase based on their id being equal to the id passed into the parameter from the View
             ApplicationUser userFromDb = _db.ApplicationUser.Where(a => a.Id == id).FirstOrDefault();
            //Their account will be disabled for 1000 years from the current date and time
            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));



        }

    }
}