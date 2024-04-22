using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using ALFarhadresses.Models;
using ALFarhadresses.Areas.Data;
using ALFarhadresses.Areas.Role.Data;

namespace ALFarhadresses.Controllers
{
    public class ProductsController : BaseController
    {
        private IHostingEnvironment _env;
        public ProductsController(ALFarhadressesContext Con, IHostingEnvironment env) : base(Con)
        {
            _env = env;
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
        public IActionResult Opencart(int id)
        {
            var Cart = _Con.Products.Where(x => x.Id == id).FirstOrDefault();
            return View(Cart);
        }
        [HttpGet]
        public IActionResult AllProducts(int? CatFilter = null)
        {
            
            ViewBag.Cats = new SelectList(_Con.Categories, "Id", "Name");

            if (CatFilter != null)
            {
                var contin = _Con.Products.Include(x => x.CatNavigation)
                        .Where(x => x.Cat == CatFilter.Value);
                return View(contin.ToList());
            }
            else
            {
                var contin = _Con.Products.Include(p => p.CatNavigation);
                return View(contin.ToList());
            }

        }
        [HttpPost]
        public IActionResult DelSelect(string[] ProId)
        {
            int[] GetId = null;
            if (ProId != null)
            {
                GetId = new int[ProId.Length];
                int j = 0;
                foreach (string i in ProId)
                {
                    int.TryParse(i, out GetId[j++]);
                }
                List<Products> GetProId = new List<Products>();
                GetProId = _Con.Products.Where(x => GetId.Contains(x.Id)).ToList();
                foreach(var Del in GetProId)
                {
                    _Con.Products.Remove(Del);
                    string folder = _env.ContentRootPath +
                         "/wwwroot/Dresses/" + Del.Id;

                    if (System.IO.Directory.Exists(folder))
                    {
                        string[] filesinDir = System.IO.Directory.GetFiles(folder);
                        foreach (string file in filesinDir)
                        {
                            System.IO.File.Delete(file);

                        }
                    }
                    System.IO.Directory.Delete(folder, true);
                }
                _Con.SaveChanges();
            }
            return RedirectToAction("AllProducts");
        }
        [HttpGet]
        public IActionResult Index(int PN = 1, int PS = 12)
        {
            int skip = (PN - 1) * PS;
            var Pro = _Con.Products.OrderByDescending(x => x.Id).Skip(skip).Take(PS).ToList();
            ViewBag.Pro = Pro;
            ViewBag.Tottal = _Con.Products.Count();
            ViewBag.PN = PN;
            ViewBag.PS = PS;
            return View();
        }
        public IActionResult Orders()
        {
            return View();
        }
        public IActionResult Notifications()
        {
            return View();
        }
        public IActionResult Create()
        {
            if (ModelState.IsValid)
            {
                var CatList = _Con.Categories.ToList();
                List<SelectListItem> SLI = new List<SelectListItem>();
                foreach (var Categories in CatList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = Categories.Id.ToString();
                    item.Text = Categories.Name;
                    SLI.Add(item);
                }
                ViewBag.SLI = SLI;
                ViewData["Cat"] = new SelectList(_Con.Categories, "Id", "Name");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Products Pro)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                _Con.Products.Add(Pro);

                if (files.Count > 0)
                {
                    _Con.SaveChanges();

                    string folder = _env.ContentRootPath +
                    "/wwwroot/Dresses/" + Pro.Id;
                    Pro.Ephoto = folder;
                    _Con.Products.Update(Pro);
                    _Con.SaveChanges();
                    if (System.IO.Directory.Exists(folder))
                    {
                        string[] filesinDir = System.IO.Directory.GetFiles(folder);
                        foreach (string file in filesinDir)
                        {
                            System.IO.File.Delete(file);

                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(folder);
                    }
                }

                for (int i = 0; i < files.Count; i++)
                {
                    int lastIndex = files[i].FileName.LastIndexOf(".");
                    int filenameLenght = files[i].FileName.Length;
                    int numberOfchar = filenameLenght - lastIndex;
                    string extension = files[i].FileName.Substring(lastIndex, numberOfchar);
                    string path = _env.ContentRootPath + "/wwwroot/Dresses/"
                        + Pro.Id + "/" + i.ToString() + extension;
                    using (FileStream fs = System.IO.File.Create(path))
                    {
                        files[i].CopyTo(fs);
                        fs.Flush();
                    }


                }
                return RedirectToAction("AllProducts");
            }
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var CatList = _Con.Categories.ToList();
            List<SelectListItem> SLI = new List<SelectListItem>();
            foreach (var Categories in CatList)
            {
                SelectListItem item = new SelectListItem();
                item.Value = Categories.Id.ToString();
                item.Text = Categories.Name;
                SLI.Add(item);
            }
            ViewBag.SLI = SLI;

            if (id == null)
            {
                return NotFound();
            }

            var Products = await _Con.Products.FindAsync(id);
            if (Products == null)
            {
                return NotFound();
            }
            ViewData["Cat"] = new SelectList(_Con.Categories, "Id", "Id", Products.Cat);
            return View(Products);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Photo,Ammount,Price,Cat")] Products Pro)
        {
            if (id != Pro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _Con.Update(Pro);
                    await _Con.SaveChangesAsync();
                    var files = HttpContext.Request.Form.Files;
                    _Con.Products.Update(Pro);

                    if (files.Count == 0)
                    {
                        _Con.SaveChanges();

                        string folder = _env.ContentRootPath +
                        "/wwwroot/Dresses/" + Pro.Id;

                        Pro.Ephoto = folder;
                        _Con.Products.Update(Pro);
                        _Con.SaveChanges();


                        for (int i = 0; i < files.Count; i++)
                        {
                            int lastIndex = files[i].FileName.LastIndexOf(".");
                            int filenameLenght = files[i].FileName.Length;
                            int numberOfchar = filenameLenght - lastIndex;
                            string extension = files[i].FileName.Substring(lastIndex, numberOfchar);



                            string path = _env.ContentRootPath + "/wwwroot/Dresses/"
                                + Pro.Id + "/" + i.ToString() + extension;


                            using (FileStream fs = System.IO.File.Create(path))
                            {
                                files[i].CopyTo(fs);
                                fs.Flush();
                            }


                        }
                        return RedirectToAction("AllProducts");
                    }
                    else
                    {
                        if (files.Count > 0)
                        {
                            _Con.SaveChanges();

                            string folder = _env.ContentRootPath +
                            "/wwwroot/Dresses/" + Pro.Id;

                            Pro.Ephoto = folder;
                            _Con.Products.Update(Pro);
                            _Con.SaveChanges();
                            if (System.IO.Directory.Exists(folder))
                            {
                                string[] filesinDir = System.IO.Directory.GetFiles(folder);
                                foreach (string file in filesinDir)
                                {
                                    System.IO.File.Delete(file);

                                }
                            }
                            else
                            {
                                System.IO.Directory.CreateDirectory(folder);
                            }
                            for (int i = 0; i < files.Count; i++)
                            {
                                int lastIndex = files[i].FileName.LastIndexOf(".");
                                int filenameLenght = files[i].FileName.Length;
                                int numberOfchar = filenameLenght - lastIndex;
                                string extension = files[i].FileName.Substring(lastIndex, numberOfchar);



                                string path = _env.ContentRootPath + "/wwwroot/Dresses/"
                                    + Pro.Id + "/" + i.ToString() + extension;


                                using (FileStream fs = System.IO.File.Create(path))
                                {
                                    files[i].CopyTo(fs);
                                    fs.Flush();
                                }


                            }
                            return RedirectToAction("AllProducts");
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(Pro.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cat"] = new SelectList(_Con.Categories, "Id", "Id", Pro.Cat);
            return View(Pro);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Products = await _Con.Products
                .Include(p => p.CatNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Products == null)
            {
                return NotFound();
            }

            return View(Products);
        }
        [HttpPost]
        public IActionResult Delete(Products Pro)
        {
            _Con.Products.Remove(Pro);
            _Con.SaveChanges();
            string folder = _env.ContentRootPath +
                           "/wwwroot/Dresses/" + Pro.Id;

            if (System.IO.Directory.Exists(folder))
            {
                string[] filesinDir = System.IO.Directory.GetFiles(folder);
                foreach (string file in filesinDir)
                {
                    System.IO.File.Delete(file);

                }
            }
            System.IO.Directory.Delete(folder, true);
            return RedirectToAction("AllProducts");
        }
        private bool ProductsExists(int id)
        {
            return _Con.Products.Any(e => e.Id == id);
        }
    }
}
