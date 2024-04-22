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
    public class CustomersController : BaseController
    {
        public CustomersController(ALFarhadressesContext Con) : base(Con)
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
        [HttpPost]
        public IActionResult DelSelect(string[] CustId)
        {
            int[] GetId = null;
            if (CustId != null)
            {
                GetId = new int[CustId.Length];
                int j = 0;
                foreach (string i in CustId)
                {
                    int.TryParse(i, out GetId[j++]);
                }
                List<Customers> GetCustId = new List<Customers>();
                GetCustId = _Con.Customers.Where(x => GetId.Contains(x.Id)).ToList();
                foreach (var Del in GetCustId)
                {
                    _Con.Customers.Remove(Del);
                }
                _Con.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index()
        {
            return View(await _Con.Customers.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _Con.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customers Cust)
        {
            if (ModelState.IsValid)
            {
                _Con.Add(Cust);
                _Con.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(Cust);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _Con.Customers.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }
            return View(customers);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id,Customers Cust)
        {
            if (ModelState.IsValid)
            {
                _Con.Update(Cust);
                _Con.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(Cust);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _Con.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customers = await _Con.Customers.FindAsync(id);
            try
            {
                _Con.Customers.Remove(customers);
                await _Con.SaveChangesAsync();
            }
            catch(Exception)
            {
                ViewBag.Msg = "You Cant Delete This Customers Because The Operation Orders Not Completed";
                return View(customers);
            }
            return RedirectToAction(nameof(Index));
        }
        private bool CustomersExists(int id)
        {
            return _Con.Customers.Any(e => e.Id == id);
        }
    }
}
