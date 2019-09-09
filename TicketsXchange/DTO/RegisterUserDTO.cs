using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketsXchange.DTO
{
    public class RegisterUserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    };
}