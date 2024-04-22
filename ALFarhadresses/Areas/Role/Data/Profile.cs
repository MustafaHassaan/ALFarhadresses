using ALFarhadresses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFarhadresses.Areas.Role.Data
{
    public class Profile
    {
        public Customers Cust { set; get; }

        [NotMapped]
        public int? Id { set; get; }
    }
}
