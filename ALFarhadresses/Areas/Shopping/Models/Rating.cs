using System;
using System.Collections.Generic;

namespace ALFarhadresses.Models
{
    public partial class Rating
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Value { get; set; }
        public int Cust { get; set; }
        public int Pro { get; set; }
        public int Cat { get; set; }

        public Categories CatNavigation { get; set; }
        public Customers CustNavigation { get; set; }
        public Products ProNavigation { get; set; }
    }
}
