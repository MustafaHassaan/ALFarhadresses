using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ALFarhadresses.Areas.Data;
using ALFarhadresses.Areas.Role.Data;
using ALFarhadresses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ALFarhadresses.Areas.Role.Controllers
{
    public class AccountController : Controller
    {
        private readonly ALFarhadressesContext _Con;
        public AccountController(ALFarhadressesContext Con)
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
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(Signup CPW, string Email)
        {
            if (ModelState.IsValid)
            {
                var Emailused = _Con.Customers.Where(x => x.Email == CPW.Cust.Email).FirstOrDefault();
                if (CPW.ConfirmPassword == CPW.Cust.Password && Emailused == null)
                {
                    
                    int userid = CPW.Cust.Id;
                    string username = CPW.Cust.Fname +" "+ CPW.Cust.Lname;
                    Email = CPW.Cust.Email;
                    string Title = "ALFarhadresses";
                    string Body = "Welcom :" +" "+ username +"<br />"+"<br />";
                    Body += "Your participation has been confirmed in the website ALFarhadresses ." + "<br />" + "<br />";
                    Body += "IF Your Forget The Password You Can Com Back To Her To Remmember You Password ." + "<br />" + "<br />";
                    Body += "So Save This Masseage To Can See The Password ." + "<br />" + "<br />";
                    Body += "Your Password Is :" +" "+ CPW.Cust.Password +"<br />"+"<br />";
                    Body += "Folow Login From This Address From Here" + "<br />" + "<br />";
                    Body += "https://localhost:44354/Role/Account/Login/?id=" + userid ;
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
                    _Con.Customers.Add(CPW.Cust);
                    _Con.SaveChanges();
                    ViewBag.Msg = "Registered We Send Masseage For You In Mail Please Checked And confirme";
                    return View();
                    //return Redirect("/Role/Account/Login");
                }
                else if (CPW.ConfirmPassword == CPW.Cust.Password && Emailused != null)
                {
                    ViewBag.Msg = "This Email Is Used";
                    return View();
                }
                else
                {
                    ViewBag.Msg = "Password Not Matched";
                    return View();
                }
            }
            else
            {
                ViewBag.Msg = "Error";
                return View();
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login Log)
        {
            if (ModelState.IsValid)
            {
                var user = _Con.Customers.Where(X => X.Email == Log.Email && X.Password == Log.Password).FirstOrDefault();
                if (user != null)
                {
                    if (user.Admin == true)
                    {
                        HttpContext.Session.SetString("Email", Log.Email);
                        HttpContext.Session.SetString("Password", Log.Password);
                        ViewBag.Msg = "Login Is Successfully";
                        return Redirect("/Admin/Products/Index");
                    }
                    else
                    {
                        HttpContext.Session.SetString("Email", Log.Email);
                        HttpContext.Session.SetString("Password", Log.Password);
                        ViewBag.Msg = "Login Is Successfully";
                        return Redirect("/Shopping/Home/Index");
                    }
                }
                else
                {

                    ViewBag.Msg = "Email Or Password Not Corrected";
                    return View();
                }

            }
            else
            {
                ViewBag.Msg = "Error";
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("Email", "");
            HttpContext.Session.SetString("Password", "");
            return Redirect("/Shopping/Home/Index");
        }


    }
}