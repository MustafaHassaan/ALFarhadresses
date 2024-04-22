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
    public class CartController : Controller
    {
        private readonly ALFarhadressesContext _Con;
        private readonly ISessioncart _ISC;
        public CartController(ALFarhadressesContext Con, ISessioncart ISC)
        {
            _Con = Con;
            _ISC = ISC;
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
        public IActionResult Cash()
        {
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Cash(int Custid,string Address,string Phone)
        {
            try
            {
                var cart = _ISC.GetCart();
                if (cart != null && cart.Count > 0)
                {
                    Orders order = new Orders();
                    order.Phone = Phone;
                    order.Payment = "Cash";
                    order.Date = DateTime.Now;
                    order.Fees = 0.2;
                    order.ShipAddress = Address;
                    order.Status = "InProcess";
                    order.Cust = Custid;
                    _Con.Orders.Add(order);
                    _Con.SaveChanges();
                    foreach (var item in cart)
                    {
                        if (cart != null && cart.Count > 0)
                        {
                            Orderitem orderitem = new Orderitem();
                            orderitem.Pro = item.Id;
                            orderitem.OrderId = order.Id;
                            orderitem.Cat = item.Cat;
                            orderitem.Quantity = item.Quantity;
                            _Con.Orderitem.Add(orderitem);
                        }
                    }

                    _Con.SaveChanges();
                    _ISC.ClearCart();
                    ViewBag.Msg = "Congratolation Your Order His Sended To Admin We Will Contact With You As Soon";
                }

            }
      
            catch(Exception)
            {
                ViewBag.Msg = "Your Detailes Is Empty";
            }
            return View();
        }
        [HttpGet]
        public IActionResult Visa()
        {
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Visa(int Custid, string Address, string Phone)
        {
            try
            {
                var cart = _ISC.GetCart();
                if (cart != null && cart.Count > 0)
                {
                    Orders order = new Orders();
                    order.Phone = Phone;
                    order.Payment = "Visa";
                    order.Date = DateTime.Now;
                    order.Fees = 0.2;
                    order.ShipAddress = Address;
                    order.Status = "InProcess";
                    order.Cust = Custid;
                    _Con.Orders.Add(order);
                    _Con.SaveChanges();
                    foreach (var item in cart)
                    {
                        if (cart != null && cart.Count > 0)
                        {
                            Orderitem orderitem = new Orderitem();
                            orderitem.Pro = item.Id;
                            orderitem.OrderId = order.Id;
                            orderitem.Cat = item.Cat;
                            orderitem.Quantity = item.Quantity;
                            _Con.Orderitem.Add(orderitem);
                        }
                    }

                    _Con.SaveChanges();
                    _ISC.ClearCart();
                    ViewBag.Msg = "Congratolation Your Order His Sended To Admin We Will Contact With You As Soon";
                }
                else
                {
                    ViewBag.Msg = "Your Detailes Is Empty";
                }
            }

            catch (Exception)
            {
            }
            return View();
        }
        [HttpGet]
        public IActionResult Cart()
        {
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            else
            {
                List<Products> cart = new List<Products>();

                cart = _ISC.GetCart();

                return View(cart);
            }
        }
        [HttpPost]
        public IActionResult Cart(int Tottal,string Payment)
        {
            var Result = Tottal;
            List<Products> cart = new List<Products>();
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            else if(Result == 0)
            {
                ViewBag.Msg = "Your Cart Is Empty";
                return View(cart);
            }
            else if(Payment == "Visa")
            {
                return Redirect("/Shopping/Cart/Visa");
            }
            else if (Payment == "Cash")
            {
                return Redirect("/Shopping/Cart/Cash");
            }
            else
            {
                return RedirectToAction("Cart");
            }
        }
        public IActionResult DeleteFromCart(int id)
        {

            List<Products> cart = new List<Products>();

            cart = _ISC.RemoveFromCart(id);

            return RedirectToAction("Cart");
        }
    }
}
