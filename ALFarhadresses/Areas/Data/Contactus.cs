using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFarhadresses.Models
{
    public partial class Contactus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Quistion { get; set; }
        public string Status { get; set; }
    }
}
