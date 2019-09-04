using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketsXchange.ViewModel
{
    public class SellFormDTO
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public int PaymentMethod { get; set; }
    }
}