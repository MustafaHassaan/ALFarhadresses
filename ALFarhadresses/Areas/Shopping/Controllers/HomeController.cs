using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ALFarhadresses.Areas.Data;
using ALFarhadresses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ALFarhadresses.Areas.Shopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ALFarhadressesContext _Con;
        private readonly ISessioncart _ISC;
        public HomeController(ALFarhadressesContext Con, ISessioncart ISC)
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
        public IActionResult Opencart(int id)
        {
            var Cart = _Con.Products.Where(x => x.Id == id).FirstOrDefault();
            return View(Cart);
        }
        [HttpPost]
        public IActionResult Opencart(int id, int Qty)
        {
            var Content = _Con.Products.Where(x => x.Id == id).FirstOrDefault();
            Content.Quantity = Qty;
            List<Products> cart = new List<Products>();
            int index = isExist(id);
            if (string.IsNullOrEmpty(ViewBag.userFname))
            {
                return Redirect("/Role/Account/Login");
            }
            else if (Content.Quantity > Content.Ammount)
            {
                ViewBag.Msg = "The Quantity You Are Requasted His Bigr than Ammount";
            }
            else if (index != -1)
            {
                ViewBag.Msg = "The Product His Already Add To Cart";
            }
            else
            {

                    cart = _ISC.AddToCart(Content);
                    ViewBag.Msg = "The Product Added To Cart";
            }
            return View(Content);
        }
        private int isExist(int id)
        {
            List<Products> cart = _ISC.GetCart();
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
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
        public IActionResult Search(string Search = null)
        {
            if (Search != null)
            {

                var Pro = _Con.Products.OrderByDescending(x => x.Id).Where(x => x.Name.Contains(Search));
                ViewBag.Pro = Pro;
                return View(Pro);
            }
            else
            {
                return Redirect("/Shopping/Home/Index");
            }
        }
        public IActionResult Rate(Rating Rate, int RN, int ProId, int ProCi, int Cust)
        {
            //RN : Rate Number
            Rate.Date = DateTime.Today;
            Rate.Value = RN;
            Rate.Cust = Cust;
            Rate.Pro = ProId;
            Rate.Cat = ProCi;
            _Con.Rating.Add(Rate);
            _Con.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Contactus(Contactus con,string Email)
        {
            var Emailused = _Con.Customers.Where(x => x.Email == Email).FirstOrDefault();
            if (Emailused != null)
            {
                
                string username = con.Name;
                Email = con.Email;
                string Title = "ALFarhadresses";
                string Body = "Welcom :" + " " + username + "<br />" + "<br />";
                Body += "Your participation has been confirmed in the website ALFarhadresses ." + "<br />" + "<br />";
                MailMessage MS = new MailMessage("ALFarhadresses@gmail.com", Email);
                MS.Subject = Title;
                MS.Body = Body;
                MS.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.Credentials = new NetworkCredential()
                {
                    UserName = "ALFarhadresses@gmail.com",
                    Password = "01212341220",
                };
                client.EnableSsl = true;
                client.Send(MS);
                _Con.Contactus.Add(con);
                _Con.SaveChanges();
                ViewBag.Msg = "Registered We Send Masseage For You In Mail Please Checked And confirme";
                return View();
                //return Redirect("/Role/Account/Login");
            }
            else
            {
                ViewBag.Msg = "Email Not Found";
                return View();
            }
        }
    }
}