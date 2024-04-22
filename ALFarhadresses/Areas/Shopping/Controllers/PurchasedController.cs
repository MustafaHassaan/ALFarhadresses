using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFarhadresses.Areas.Data;
using ALFarhadresses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ALFarhadresses.Areas.Shopping.Controllers
{
    public class PurchasedController : Controller
    {
        private readonly ALFarhadressesContext _Con;
        public PurchasedController(ALFarhadressesContext Con)
        {
            _Con = Con;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Email")) && string.IsNullOrEmpty(HttpContext.Session.GetString("Password")))
            {
                ViewBag.users = Cust.Customers;
            }
            else
            {
                string Email = HttpContext.Session.GetString("Email");
                string Password = HttpContext.Session.GetString("Password");
                var user = _Con.Customers.AsNoTracking().Where(u => u.Email == Email && u.Password == Password).FirstOrDefault();
                if (user != null)
                {
                    ViewBag.userId = user.Id;
                    ViewBag.userFname = user.Fname;
                    ViewBag.userLname = user.Lname;
                    ViewBag.userEmail = user.Email;
                    ViewBag.userPassword = user.Password;
                    ViewBag.userPhone = user.Phone;
                    ViewBag.userCity = user.City;
                    ViewBag.userAddress = user.Address;
                    ViewBag.userGender = user.Gender;
                    ViewBag.userUtype = user.Admin;
                    if (user.Admin == false)
                    {
                        ViewBag.users = Cust.Customers;
                    }
                    else
                    {
                        ViewBag.users = Cust.Admin;
                    }
                }
                else
                {
                    ViewBag.users = Cust.Anonymous;
                }
            }
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
        [HttpGet]
        public IActionResult Index(string Purchased = null)
        {
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            else
            {
                if (Purchased != null)
                {

                    var Pur = _Con.Orders.OrderByDescending(x => x.Id).Where(x => x.CustNavigation.Email.Contains(Purchased));
                    ViewBag.Pur = Pur;
                    return View(Pur);
                }
                else
                {
                    return Redirect("/Shopping/Home/Index");
                }
            }
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            else
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
        }
    }
}