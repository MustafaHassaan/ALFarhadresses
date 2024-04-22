using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALFarhadresses.Models
{
    public partial class Orders
    {
        public Orders()
        {
            Orderitem = new HashSet<Orderitem>();
        }

        public int Id { get; set; }
        public string Phone { get; set; }
        public string Payment { get; set; }
        public DateTime Date { get; set; }
        public double Fees { get; set; }
        public string ShipAddress { get; set; }
        public string Status { get; set; }
        public int Cust { get; set; }

        [NotMapped]
        public string BankName { get; set; }

        [NotMapped]
        public string Pin { get; set; }


        [NotMapped]
        public string CardNumber { get; set; }

        [NotMapped]
        public DateTime ExpirationDate { get; set; }



        public Customers CustNavigation { get; set; }
        public ICollection<Orderitem> Orderitem { get; set; }
    }
}
