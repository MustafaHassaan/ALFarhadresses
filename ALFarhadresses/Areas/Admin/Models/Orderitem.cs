using System;
using System.Collections.Generic;

namespace ALFarhadresses.Models
{
    public partial class Orderitem
    {
        public int Id { get; set; }
        public int Pro { get; set; }
        public int Cat { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }

        public Categories CatNavigation { get; set; }
        public Orders Order { get; set; }
        public Products ProNavigation { get; set; }
    }
}
