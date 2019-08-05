using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TicketsXchange.Models.DAL;
using TicketsXchange.Helper;

namespace TicketsXchange.Controllers
{
    public class UserController : ApiController
    {
        private TxcDevEntities db = new TxcDevEntities();

        // GET: api/User
    
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/User/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/User/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
     
        [Route("api/login")]

        public IHttpActionResult Login([FromBody] User request)
        {
            var password = request.Password;
            User user = db.Users.Single(a => a.Email == request.Email);

            if (user == null)
            {
                return Ok(3);
            }

            if(PasswordHelper.verifyMd5Hash(request.Password, user.Password))
            {
                return Ok("OK");
            }
            else
            {
                return Ok(2);
            }
            
        }
        public class RegisterUser
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        };
        // POST: api/User
        [Route("api/register")]
        public IHttpActionResult Register([FromBody] RegisterUser request)
        {

            User user = new User { Email=request.Email, Password= PasswordHelper.getMd5Hash(request.Password), CreatedAt=DateTime.Now, Verified=0};
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Users.Add(user);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            Profile profile = new Profile { FirstName = request.FirstName, LastName = request.LastName, User
            = user, UserId = user.Id};
            db.Profiles.Add(profile);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
               
            }
            return Ok(1);
        }

        // DELETE: api/User/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}