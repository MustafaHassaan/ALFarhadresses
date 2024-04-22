using ALFarhadresses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ALFarhadresses.Areas.Role.Data
{
    public class Signup
    {
        public Customers Cust { set; get; }
        [DisplayName("Confirm Password")]

        [Required(ErrorMessage = "Confirm Password Is Required")]
        [MinLength(8)]
        [MaxLength(12)]
        public string ConfirmPassword { set; get; }
    }
}
