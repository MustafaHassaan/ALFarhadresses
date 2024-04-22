using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFarhadresses.Areas.Data;
using ALFarhadresses.Areas.Role.Data;
using ALFarhadresses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ALFarhadresses.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(ALFarhadressesContext Con):base(Con)
        {
        }



        public IActionResult Cat(int id, int PN = 1)
        {

            var Categ = _Con.Categories.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.CC = Categ;

            var CL = _Con.Products.Where(x => x.CatNavigation.Id == id).ToList();


            int PS = 12;
            int Tottal = CL.Count;

            ViewBag.PS = PS;
            ViewBag.Tottal = Tottal;
            ViewBag.PN = PN;

            var FCL = CL.Skip((PN - 1) * PS).Take(PS).ToList();

            return View(FCL);
        }
        public IActionResult OC(int id, int PN = 1)
        {

            var Categ = _Con.Categories.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.CC = Categ;


            var CL = _Con.Products.Where(x => x.CatNavigation.Id == id).ToList();


            int PS = 4;
            int Tottal = CL.Count;

            ViewBag.PS = PS;
            ViewBag.Tottal = Tottal;
            ViewBag.PN = PN;

            var FCL = CL.Skip((PN - 1) * PS).Take(PS).ToList();

            return View(FCL);
        }
        [HttpGet]
        public IActionResult MgCategory()
        {
            return View(_Con.Categories.ToList());
        }
        [HttpPost]
        public IActionResult MgCategory(string[] CatId, int id)
        {

            try
            {
                int[] GetId = null;
                if (CatId != null)
                {
                    GetId = new int[CatId.Length];
                    int j = 0;
                    foreach (string i in CatId)
                    {
                        int.TryParse(i, out GetId[j++]);
                    }
                    List<Categories> GetCatId = new List<Categories>();
                    GetCatId = _Con.Categories.Where(x => GetId.Contains(x.Id)).ToList();
                    foreach (var Del in GetCatId)
                    {
                        _Con.Categories.Remove(Del);
                    }
                    _Con.SaveChanges();
                }
            }
            catch(Exception)
            {
                ViewBag.Msg = "You Cant Delete This Category Because His Have Products";
                return View(_Con.Categories.ToList());
            }
            return RedirectToAction(nameof(MgCategory));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Category = await _Con.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Category == null)
            {
                return NotFound();
            }

            return View(Category);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Categories Category)
        {
            if (ModelState.IsValid)
            {
                _Con.Add(Category);
                await _Con.SaveChangesAsync();
                return RedirectToAction(nameof(MgCategory));
            }
            return View(Category);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Category = await _Con.Categories.FindAsync(id);
            if (Category == null)
            {
                return NotFound();
            }
            return View(Category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Categories Category)
        {
            if (id != Category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _Con.Update(Category);
                    await _Con.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(Category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MgCategory));
            }
            return View(Category);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Category = await _Con.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Category == null)
            {
                return NotFound();
            }

            return View(Category);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var Category = _Con.Categories.Find(id);
            try
            {
                _Con.Categories.Remove(Category);
                _Con.SaveChanges();
            }
            catch(Exception)
            {
                ViewBag.Msg = "You Cant Delete This Category Because His Have Products";
                return View(Category);
            }
            return RedirectToAction(nameof(MgCategory));
        }
        private bool CategoryExists(int id)
        {
            return _Con.Categories.Any(e => e.Id == id);
        }
    }
}
