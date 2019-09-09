using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketsXchange.DTO
{
    public class PayModelDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public float Total { get; set; }
    }
}