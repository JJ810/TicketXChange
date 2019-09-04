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
using TicketsXchange.DTO;
using TicketsXchange.Models;

namespace TicketsXchange.Controllers
{
    public class SubscribeEmailApiController : ApiController
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();

        // GET: api/SubscribeEmail
        public IQueryable<SubscribeEmail> GetSubscribeEmails()
        {
            return db.SubscribeEmails;
        }

        // GET: api/SubscribeEmail/5
        [ResponseType(typeof(SubscribeEmail))]
        public IHttpActionResult GetSubscribeEmail(int id)
        {
            SubscribeEmail subscribeEmail = db.SubscribeEmails.Find(id);
            if (subscribeEmail == null)
            {
                return NotFound();
            }

            return Ok(subscribeEmail);
        }

        // PUT: api/SubscribeEmail/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSubscribeEmail(int id, SubscribeEmail subscribeEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subscribeEmail.Id)
            {
                return BadRequest();
            }

            db.Entry(subscribeEmail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscribeEmailExists(id))
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

        // POST: api/SubscribeEmail
        [ResponseType(typeof(SubscribeEmail))]
        public IHttpActionResult PostSubscribeEmail(SubscribeEmail subscribeEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SubscribeEmails.Add(subscribeEmail);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = subscribeEmail.Id }, subscribeEmail);
        }

        // DELETE: api/SubscribeEmail/5
        [ResponseType(typeof(SubscribeEmail))]
        public IHttpActionResult DeleteSubscribeEmail(int id)
        {
            SubscribeEmail subscribeEmail = db.SubscribeEmails.Find(id);
            if (subscribeEmail == null)
            {
                return NotFound();
            }

            db.SubscribeEmails.Remove(subscribeEmail);
            db.SaveChanges();

            return Ok(subscribeEmail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SubscribeEmailExists(int id)
        {
            return db.SubscribeEmails.Count(e => e.Id == id) > 0;
        }

        [System.Web.Http.Route("api/Subscribe/Admin")]
        public IHttpActionResult Admin([FromBody] SubscribeMailDTO request)
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
            List<SubscribeMailDTO> list = new List<SubscribeMailDTO>();
            if (action == "list")
            {
                foreach (var mail in db.SubscribeEmails.OrderBy(a => a.Id).Skip(start).Take(page))
                {
                    SubscribeMailDTO item = new SubscribeMailDTO();
                    item.Id = mail.Id;
                    item.Email = mail.Email;
                    item.CreatedAt = String.Format("{0:dd/MM/yyyy}", mail.CreatedAt);
                    list.Add(item);
                }
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Records", list);
                res.Add("TotalRecordCount", db.SubscribeEmails.Count());
                return Json(res);
            }
            else if (action == "update")
            {
                SubscribeEmail item = db.SubscribeEmails.Find(request.Id);
                item.Email = request.Email;
               
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if (action == "delete")
            {
                db.SubscribeEmails.Remove(db.SubscribeEmails.Find(request.Id));
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if (action == "create")
            {
                SubscribeEmail item = new SubscribeEmail();
                item.Email = request.Email;
                db.SubscribeEmails.Add(item);
                db.SaveChanges();
                request.Id = item.Id;
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Record", request);
                return Json(res);
            }
            return Json(1);
        }
    }
}