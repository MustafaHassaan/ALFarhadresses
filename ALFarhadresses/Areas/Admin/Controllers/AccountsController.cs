using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFarhadresses.Areas.Role.Data;
using ALFarhadresses.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALFarhadresses.Areas.Admin.Controllers
{
    public class AccountsController : BaseController
    {
        public AccountsController(ALFarhadressesContext Con):base(Con)
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




        public IActionResult Profile()
        {
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Profile(int? id)
        {
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            else
            {
                var Cust = _Con.Customers.Where(C => C.Id == id);
                return Redirect("Edit/" + id);
            }
        }
        public IActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            else
            {
                var Cust = _Con.Customers.Find(id);
                return View(Cust);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customers Cust)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(ViewBag.userFname))
                {
                    return Redirect("/Role/Account/Login");
                }
                else
                {
                    _Con.Customers.Update(Cust).State = EntityState.Modified;
                    _Con.SaveChanges();
                }
            }
            return RedirectToAction("Profile");
        }
    }
}