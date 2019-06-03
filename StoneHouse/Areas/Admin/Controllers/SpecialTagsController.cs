using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoneHouse.Data;
using StoneHouse.Models;
using StoneHouse.Utility;

namespace StoneHouse.Areas.Admin.Controllers
{
    //set authorization as only the role SuperAdminEndUser will be able to access these controllers or views
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SpecialTagsController(ApplicationDbContext db)
            {
                _db=db;
            }


        public IActionResult Index()
        {
            return View(_db.SpecialTags.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( SpecialTags specialTags)
        {
            if (ModelState.IsValid)
            {
                _db.Add(specialTags);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                return View(specialTags);
            }

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var specialTag = await _db.SpecialTags.FindAsync(id);

                if (specialTag != null)
                {
                    return View(specialTag);
                }
                else
                {
                    return NotFound();
                }


            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTags specialTags)
        {
            if (id == specialTags.Id)
            {
                if (ModelState.IsValid)
                {
                    _db.Update(specialTags);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    return NotFound();
                }


            }
            else
            {
                return View(specialTags);

            }

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var specialTag = await _db.SpecialTags.FindAsync(id);
                if (specialTag == null)
                {
                    return NotFound();

                }
                else
                {
                    return View(specialTag);
                }

            }


        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var specialTag = await _db.SpecialTags.FindAsync(id);
                if (specialTag == null)
                {
                    return NotFound();

                }
                else
                {
                    return View(specialTag);
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
           
                var specialTag = await _db.SpecialTags.FindAsync(id);
                 _db.Remove(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));



            }
          

        







    }
}