//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TicketsXchange.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SubscribeLog
    {
        public int Id { get; set; }
        public int EmailId { get; set; }
        public int TicketId { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
    
        public virtual SubscribeEmail SubscribeEmail { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}