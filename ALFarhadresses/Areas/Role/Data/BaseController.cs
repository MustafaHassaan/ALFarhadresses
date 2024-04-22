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

namespace ALFarhadresses.Areas.Role.Data
{
    public class BaseController : Controller
    {
        public ALFarhadressesContext _Con;
        public BaseController(ALFarhadressesContext Con)
        {
            _Con = Con;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Email")) && string.IsNullOrEmpty(HttpContext.Session.GetString("Password")))
            {
                context.Result = new RedirectResult("/Shopping/Home/Index");
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
                        context.Result = new RedirectResult("/Shopping/Home/Index");
                    }
                }
                else
                {
                    context.Result = new RedirectResult("/Shopping/Home/Index");
                }
            }
        }

    }
}