using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ALFarhadresses.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using ALFarhadresses.Areas.Data;
using Microsoft.AspNetCore.Http;
using ALFarhadresses.Areas.Role.Data;

namespace ALFarhadresses.Areas.Admin.Controllers
{
    public class OrderitemsController : BaseController
    {
        public OrderitemsController(ALFarhadressesContext Con) : base(Con)
        {
        }
        public async Task<IActionResult> Index()
        {
            var aLFarhadressesContext = _Con.Orderitem.Include(o => o.CatNavigation).Include(o => o.Order).Include(o => o.ProNavigation);
            return View(await aLFarhadressesContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderitem = await _Con.Orderitem
                .Include(o => o.CatNavigation)
                .Include(o => o.Order)
                .Include(o => o.ProNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderitem == null)
            {
                return NotFound();
            }

            return View(orderitem);
        }
        public IActionResult Create()
        {
            ViewData["Cat"] = new SelectList(_Con.Categories, "Id", "Name");
            ViewData["OrderId"] = new SelectList(_Con.Orders, "Id", "ShipAddress");
            ViewData["Pro"] = new SelectList(_Con.Products, "Id", "Description");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Pro,Cat,OrderId,Quantity")] Orderitem orderitem)
        {
            if (ModelState.IsValid)
            {
                _Con.Add(orderitem);
                await _Con.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cat"] = new SelectList(_Con.Categories, "Id", "Name", orderitem.Cat);
            ViewData["OrderId"] = new SelectList(_Con.Orders, "Id", "ShipAddress", orderitem.OrderId);
            ViewData["Pro"] = new SelectList(_Con.Products, "Id", "Description", orderitem.Pro);
            return View(orderitem);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderitem = await _Con.Orderitem.FindAsync(id);
            if (orderitem == null)
            {
                return NotFound();
            }
            ViewData["Cat"] = new SelectList(_Con.Categories, "Id", "Name", orderitem.Cat);
            ViewData["OrderId"] = new SelectList(_Con.Orders, "Id", "ShipAddress", orderitem.OrderId);
            ViewData["Pro"] = new SelectList(_Con.Products, "Id", "Description", orderitem.Pro);
            return View(orderitem);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Pro,Cat,OrderId,Quantity")] Orderitem orderitem)
        {
            if (id != orderitem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _Con.Update(orderitem);
                    await _Con.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderitemExists(orderitem.Id))
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
            ViewData["Cat"] = new SelectList(_Con.Categories, "Id", "Name", orderitem.Cat);
            ViewData["OrderId"] = new SelectList(_Con.Orders, "Id", "ShipAddress", orderitem.OrderId);
            ViewData["Pro"] = new SelectList(_Con.Products, "Id", "Description", orderitem.Pro);
            return View(orderitem);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderitem = await _Con.Orderitem
                .Include(o => o.CatNavigation)
                .Include(o => o.Order)
                .Include(o => o.ProNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderitem == null)
            {
                return NotFound();
            }

            return View(orderitem);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderitem = await _Con.Orderitem.FindAsync(id);
            _Con.Orderitem.Remove(orderitem);
            await _Con.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool OrderitemExists(int id)
        {
            return _Con.Orderitem.Any(e => e.Id == id);
        }
    }
}
