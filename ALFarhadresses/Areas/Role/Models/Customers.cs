using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALFarhadresses.Models
{
    public partial class Customers
    {
        public Customers()
        {
            Orders = new HashSet<Orders>();
            Rating = new HashSet<Rating>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "First Name Is Required")]
        public string Fname { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        public string Lname { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [MinLength(8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Gender Is Required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Address Is Required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone Is Required")]
        [MaxLength(11)]
        [Phone]
        public string Phone { get; set; }
        public bool Admin { get; set; }

        public ICollection<Orders> Orders { get; set; }
        public ICollection<Rating> Rating { get; set; }
    }
}
