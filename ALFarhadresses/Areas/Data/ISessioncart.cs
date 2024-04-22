using ALFarhadresses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFarhadresses.Areas.Data
{
    public interface ISessioncart
    {
        List<Products> AddToCart(Products Pro);
        List<Products> GetCart();
        List<Products> RemoveFromCart(int id);
        List<Products> UpdateCart(List<Products> cart);
        void ClearCart();
    }
}
