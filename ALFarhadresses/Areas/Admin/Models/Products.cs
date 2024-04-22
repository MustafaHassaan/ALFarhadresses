using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFarhadresses.Models
{
    public partial class Products
    {
        public Products()
        {
            Orderitem = new HashSet<Orderitem>();
            Rating = new HashSet<Rating>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description Is Required")]
        public string Description { get; set; }

        public string Ephoto { get; set; }

        [Required(ErrorMessage = "Price Is Required")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Ammount Is Required")]
        public int Ammount { get; set; }

        [NotMapped]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Cat Is Required")]
        public int Cat { get; set; }

        public Categories CatNavigation { get; set; }
        public ICollection<Orderitem> Orderitem { get; set; }
        public ICollection<Rating> Rating { get; set; }
    }
}
