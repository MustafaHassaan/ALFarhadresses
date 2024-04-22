using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ALFarhadresses.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using ALFarhadresses.Areas.Data;

namespace ALFarhadresses.Areas.Shopping.Controllers
{
    public class ContactusController : Controller
    {
        private readonly ALFarhadressesContext _context;

        public ContactusController(ALFarhadressesContext context)
        {
            _context = context;
        }
        public IActionResult Cat(int id, int PN = 1)
        {

            var Categ = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.CC = Categ;

            var CL = _context.Products.Where(x => x.CatNavigation.Id == id).ToList();


            int PS = 12;
            int Tottal = CL.Count;

            ViewBag.PS = PS;
            ViewBag.Tottal = Tottal;
            ViewBag.PN = PN;

            var FCL = CL.Skip((PN - 1) * PS).Take(PS).ToList();

            return View(FCL);
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
                var user = _context.Customers.AsNoTracking().Where(u => u.Email == Email && u.Password == Password).FirstOrDefault();
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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contactus.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactus = await _context.Contactus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactus == null)
            {
                return NotFound();
            }

            return View(contactus);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Phone,Quistion,Status")] Contactus contactus, string Email)
        {
            if (ModelState.IsValid)
            {
                var Emailused = _context.Customers.Where(x => x.Email == Email).FirstOrDefault();
                if (Emailused != null)
                {
                    string username = contactus.Name;
                    Email = contactus.Email;
                    string Title = "ALFarhadresses";
                    string Body = "Welcom :" + " " + username + "<br />" + "<br />";
                    Body += "We have successfully received your message and will contact you as soon as possible ." + "<br />" + "<br />";
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
                    _context.Add(contactus);
                    await _context.SaveChangesAsync();
                    ViewBag.Msg = "Your Masseage Hase Been Sended Successfully Cheack From Your Mail And Confirmed";
                    return View();
                }
                    else
                {
                    ViewBag.Msg = "Email Not Found";
                }
            }
            return View(contactus);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactus = await _context.Contactus.FindAsync(id);
            if (contactus == null)
            {
                return NotFound();
            }
            return View(contactus);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,Quistion,Status")] Contactus contactus)
        {
            if (id != contactus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactusExists(contactus.Id))
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
            return View(contactus);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactus = await _context.Contactus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactus == null)
            {
                return NotFound();
            }

            return View(contactus);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactus = await _context.Contactus.FindAsync(id);
            _context.Contactus.Remove(contactus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ContactusExists(int id)
        {
            return _context.Contactus.Any(e => e.Id == id);
        }
    }
}
