using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketsXchange.DTO
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }
        public int PaymentMethod { get; set; }
        public string CreatedAt { get; set; }
    }
}