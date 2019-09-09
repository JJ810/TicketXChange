using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketsXchange.DTO
{
    public class EventDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string CreatedAt { get; set; }
    }
}