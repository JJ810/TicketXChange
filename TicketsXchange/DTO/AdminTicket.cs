using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketsXchange.ViewModel
{
    public class AdminTicket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public float Price { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public int PaymentMethod { get; set; }
        public int Featured { get; set; }
        public int Balance { get; set; }
        public int Active { get; set; }
        public string CreatedAt { get; set; }
    }
}