using System;
using System.Collections.Generic;

namespace ALFarhadresses.Models
{
    public partial class Categories
    {
        public Categories()
        {
            Orderitem = new HashSet<Orderitem>();
            Products = new HashSet<Products>();
            Rating = new HashSet<Rating>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Orderitem> Orderitem { get; set; }
        public ICollection<Products> Products { get; set; }
        public ICollection<Rating> Rating { get; set; }
    }
}
