using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketsXchange.ViewModel
{
    public class SearchTicket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public float Price { get; set; }
        public string Date { get; set; }
        public bool  IsSold { get; set; }
    }
}