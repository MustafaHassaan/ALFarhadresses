using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFarhadresses.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ALFarhadresses.Areas.Data
{
    public class Cartsession : ISessioncart
    {
        IHttpContextAccessor _HCA;
        ALFarhadressesContext _Con;
        public Cartsession(IHttpContextAccessor HCA, ALFarhadressesContext Con)
        {
            _HCA = HCA;
            _Con = Con;
        }

        public List<Products> AddToCart(Products Pro)
        {
            List<Products> cart = GetCart();
            cart.Add(Pro);
            _HCA.HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            return cart;
        }

        public void ClearCart()
        {
            _HCA.HttpContext.Session.SetString("Cart", "");
        }


        public List<Products> GetCart()
        {
            List<Products> cart;
            if (string.IsNullOrWhiteSpace(_HCA.HttpContext.Session.GetString("Cart")) == false &&
                _HCA.HttpContext.Session.GetString("Cart") != "")
            {
                cart = JsonConvert.DeserializeObject<List<Products>>
                    (_HCA.HttpContext.Session.GetString("Cart"));
            }
            else
            {
                cart = new List<Products>();
            }

            return cart;
        }

        public List<Products> RemoveFromCart(int id)
        {
            List<Products> cart = GetCart();
            if (cart.Count > 0)
            {
                cart.RemoveAt(cart.FindIndex(x => x.Id == id));
            }
            _HCA.HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));

            return cart;
        }

        public List<Products> UpdateCart(List<Products> cart)
        {
            _HCA.HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            return cart;
        }
    }
}
