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
using TicketsXchange.Helper;
using System.Web.Security;
using System.Web;
using TicketsXchange.Models;
using System.Net.Mail;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using TicketsXchange.DTO;

namespace TicketsXchange.Controllers
{
    public class UserApiController : ApiController
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();

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
     
        [System.Web.Http.Route("api/login")]

        public IHttpActionResult Login([FromBody] User request)
        {
            var password = request.Password;
            User user = db.Users.Single(a => a.Email == request.Email);

            if (user == null)
            {
                return Ok(-2);
            }

            if(PasswordHelper.verifyMd5Hash(request.Password, user.Password))
            {
                //   return RedirectToAction("Edit", "Profile", new { Id = "Admin" });
                FormsAuthentication.SetAuthCookie(string.Format("{0}", user.Id), false);
             
                //       HttpContext.Current.Session["UserID"] = s;
                int? a = SessionHelper.UserID;
             
                return Ok(user.Id);
            }
            else
            {
                return Ok(-1);
            }
            
        }
        public class RegisterUser
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        };

        [System.Web.Http.Route("api/register")]
        public IHttpActionResult Register([FromBody] RegisterUser request)
        {

            if (db.Users.Where(a => a.Email == request.Email).FirstOrDefault() != null)
            {
                return Ok(-3);
            }
            string activationCode = Guid.NewGuid().ToString();
            User user = new User { Email = request.Email, Password = PasswordHelper.getMd5Hash(request.Password), CreatedAt = DateTime.Now, Verified = 0, Role = 1, Active = 0, ActivationCode = activationCode };
        
            
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
                    return Ok(-3);
                }
                else
                {
                    return Ok(-2);
                }
            }
            Profile profile = new Profile { FirstName = request.FirstName, LastName = request.LastName, User
            = user, UserId = user.Id};
            db.Profiles.Add(profile);
            try
            {
                db.SaveChanges();
                EmailHelper.SendActivationMail(user.Email, user.ActivationCode);
            }
            catch (DbUpdateException)
            {
                return Ok(-1);  
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
        
        public IHttpActionResult Admin([FromBody] UserDTO request)
        {
            var query = Request.GetQueryNameValuePairs();
            string action = "list";
            int start = 0, page = 0;
            foreach (var param in Request.GetQueryNameValuePairs())
            {
                if (param.Key == "action") action = param.Value;
                if (param.Key == "jtStartIndex") start = Int32.Parse(param.Value);
                if (param.Key == "jtPageSize") page = Int32.Parse(param.Value);
            }
            List<UserDTO> list = new List<UserDTO>();
            if (action == "list")
            {               
                foreach (var user in db.Users.OrderBy(a=>a.Id).Skip(start).Take(page))
                {
                    UserDTO item = new UserDTO();
                    item.Id = user.Id;
                    item.FirstName = user.Profile.FirstName;
                    item.LastName = user.Profile.LastName;
                    item.Email = user.Email;
                    item.Sex = (int)user.Profile.Gender;
                    item.DOB = user.Profile.DOB;
                    item.Verified = (int)user.Verified;
                    item.Active = (int)user.Active;
                    item.MobileNumber = user.Profile.MobileNumber;
                    item.PhoneNumber = user.Profile.PhoneNumber;
                    item.AddressLine1 = user.Profile.AddressLine1;
                    item.AddressLine2= user.Profile.AddressLine2;
                    item.AddressLine3 = user.Profile.AddressLine3;
                    item.PostCode = user.Profile.PostCode;
                    item.City = user.Profile.City;
                    item.State = user.Profile.State;
                    item.Country = user.Profile.Country;
                    item.CreatedAt = String.Format("{0:dd/MM/yyyy}", user.CreatedAt);
                    list.Add(item);
                }
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Records", list);
                res.Add("TotalRecordCount", db.Users.Count());
                return Json(res);
            }else if(action == "update")
            {
                User user = db.Users.Find(request.Id);
                user.Email = request.Email;
                user.Active = request.Active;
                user.Verified = (byte)request.Verified;
                Profile profile = db.Profiles.Find(request.Id);
                profile.FirstName = request.FirstName;
                profile.LastName = request.LastName;
                profile.DOB = request.DOB;
                profile.Gender = (byte)request.Sex;
                profile.MobileNumber = request.MobileNumber;
                profile.PhoneNumber = request.PhoneNumber;
                profile.AddressLine1 = request.AddressLine1;
                profile.AddressLine2 = request.AddressLine2;
                profile.AddressLine3 = request.AddressLine3;
                profile.PostCode = request.PostCode;
                profile.City = request.City;
                profile.State = request.State;
                profile.Country = request.Country;
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if(action == "delete")
            {
                db.Users.Remove(db.Users.Find(request.Id));
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if(action == "create")
            {
                User user = new User();
                user.Email = request.Email;
                user.Active = request.Active;
                user.Verified = (byte)request.Verified;
                user.CreatedAt = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                Profile profile = new Profile();
                profile.UserId = user.Id;
                profile.FirstName = request.FirstName;
                profile.LastName = request.LastName;
                profile.DOB = request.DOB;
                profile.Gender = (byte)request.Sex;
                profile.MobileNumber = request.MobileNumber;
                profile.PhoneNumber = request.PhoneNumber;
                profile.AddressLine1 = request.AddressLine1;
                profile.AddressLine2 = request.AddressLine2;
                profile.AddressLine3 = request.AddressLine3;
                profile.PostCode = request.PostCode;
                profile.City = request.City;
                profile.State = request.State;
                profile.Country = request.Country;
                db.Profiles.Add(profile);
                db.SaveChanges();
                request.Id = user.Id;
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Record", request);
                return Json(res);
            }
            return Json(1);
        }
     
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/UserName/GetJson")]
        public IHttpActionResult GetJson()
        {
            List<JsonDataDTO> list = new List<JsonDataDTO>();
            foreach (var ev in db.Users)
            {
                JsonDataDTO data = new JsonDataDTO();
                data.DisplayText = ev.Profile.FirstName + " " + ev.Profile.LastName;
                data.Value = ev.Id;
                list.Add(data);
            }
            Dictionary<string, Object> res = new Dictionary<string, Object>();
            res.Add("Result", "OK");
            res.Add("Options", list);
            return Json(res);
        }
    }
}