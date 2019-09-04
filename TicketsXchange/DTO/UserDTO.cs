using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketsXchange.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Sex { get; set; }
        public string DOB { get; set; }
        public int Verified { get; set; }
        public int Active { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string CreatedAt { get; set; }
    }
}