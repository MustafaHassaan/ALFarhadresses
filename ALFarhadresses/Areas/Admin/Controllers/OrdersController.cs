using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ALFarhadresses.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using ALFarhadresses.Areas.Data;
using ALFarhadresses.Areas.Role.Data;

namespace ALFarhadresses.Areas.Admin.Controllers
{
    public class OrdersController : BaseController
    {

        public OrdersController(ALFarhadressesContext Con) : base(Con)
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
        public IActionResult Delorder(int id)
        {
            var orderitem = _Con.Orderitem.Find(id);
            _Con.Orderitem.Remove(orderitem);
            _Con.SaveChanges();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index()
        {
            var aLFarhadressesContext = _Con.Orders.Include(o => o.CustNavigation);
            return View(await aLFarhadressesContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            var orders = await _Con.Orders
                                       .Include(s => s.Orderitem)
                                       .ThenInclude(e => e.ProNavigation)
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }
        public IActionResult Create()
        {
            ViewData["Cust"] = new SelectList(_Con.Customers, "Id", "Address");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Payment,Tquantity,Date,Fees,ShipAddress,Status,Cust")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                _Con.Add(orders);
                await _Con.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cust"] = new SelectList(_Con.Customers, "Id", "Address", orders.Cust);
            return View(orders);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _Con.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            ViewData["Cust"] = new SelectList(_Con.Customers, "Id", "Address", orders.Cust);
            return View(orders);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Orders orders)
        {
            if (ModelState.IsValid)
            {
                _Con.Update(orders);
                _Con.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }
        public IActionResult Confirmed()
        { 
                return View();
        }
        public IActionResult Delete(int? id)
        {
            var orders = _Con.Orders.FirstOrDefault(m => m.Id == id);
            return View(orders);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            
            var orders = _Con.Orders.Find(id);
            try
            {
                _Con.Orders.Remove(orders);
                _Con.SaveChanges();
            }
            catch(Exception)
            {
                ViewBag.Msg = "You Cant Delete This order Because The Customers Have Orders";
                return View(orders);
            }
            return RedirectToAction(nameof(Index));
        }
        private bool OrdersExists(int id)
        {
            return _Con.Orders.Any(e => e.Id == id);
        }
    }
}
