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
    public class EventApiController : ApiController
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();

        // GET: api/SellApi
        public IQueryable<Event> GetSells()
        {
            return db.Events;
        }

        // GET: api/SellApi/5
        [ResponseType(typeof(Event))]
        public IHttpActionResult GetSell(int id)
        {
            Event sell = db.Events.Find(id);
            if (sell == null)
            {
                return NotFound();
            }

            return Ok(sell);
        }

        // PUT: api/SellApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSell(int id, Event sell)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sell.Id)
            {
                return BadRequest();
            }

            db.Entry(sell).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellExists(id))
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

        // POST: api/SellApi
        [ResponseType(typeof(Event))]
        public IHttpActionResult PostSell(Event sell)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Events.Add(sell);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (SellExists(sell.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = sell.Id }, sell);
        }

        // DELETE: api/SellApi/5
        [ResponseType(typeof(Event))]
        public IHttpActionResult DeleteSell(int id)
        {
            Event sell = db.Events.Find(id);
            if (sell == null)
            {
                return NotFound();
            }

            db.Events.Remove(sell);
            db.SaveChanges();

            return Ok(sell);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SellExists(int id)
        {
            return db.Events.Count(e => e.Id == id) > 0;
        }
        public class AdminEvent
        {
            public int Id { get; set; }
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public string Details { get; set; }
            public string CreatedAt { get; set; }
        }
        [System.Web.Http.Route("api/Event/Admin")]
        public IHttpActionResult Admin([FromBody] AdminEvent request)
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
            List<AdminEvent> list = new List<AdminEvent>();
            if (action == "list")
            {
                foreach (var ev in db.Events.OrderBy(a => a.Id).Skip(start).Take(page))
                {
                    AdminEvent item = new AdminEvent();
                    item.Id = ev.Id;
                    item.CategoryId = (int)ev.CategoryId;
                    item.Name = ev.Name;
                    item.Details = ev.Details;
                    item.CreatedAt = String.Format("{0:dd/MM/yyyy}", ev.CreatedAt);
                    list.Add(item);
                }
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Records", list);
                res.Add("TotalRecordCount", db.Events.Count());
                return Json(res);
            } else if (action == "delete")
            {
                db.Events.Remove(db.Events.Find(request.Id));
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if (action == "update")
            {
                Event item = db.Events.Find(request.Id);
                item.CategoryId = request.CategoryId;
                item.Name = request.Name;
                item.Details = request.Details;
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if (action == "create")
            {
                Event item = new Event();
                item.CategoryId = request.CategoryId;
                item.Name = request.Name;
                item.Details = request.Details;
                item.CreatedAt = DateTime.Now;
                db.Events.Add(item);
                db.SaveChanges();
                request.Id = item.Id;
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Record", request);
                return Json(res);
            }

            return Json(1);
        }
       
        [HttpPost]
        [System.Web.Http.Route("api/Event/GetJson")]
        public IHttpActionResult GetJson()
        {
            List<JsonDataDTO> list = new List<JsonDataDTO>();
            foreach (var ev in db.Events)
            {
                JsonDataDTO data = new JsonDataDTO();
                data.DisplayText = ev.Name;
                data.Value = ev.Id;
                list.Add(data);
            }
            Dictionary<string, Object> res = new Dictionary<string, Object>();
            res.Add("Result", "OK");
            res.Add("Options", list);
            return Json(res);
        }
        [HttpGet]
        [Route("api/EventNameSearch")]
        public IQueryable<Event> NameSearch([FromUri]String name)
        {
            const int ItemsPerPage = 5;

            string makeLower(string x) => String.IsNullOrEmpty(x) ? "" : x.ToLower();
            name = makeLower(name);

            return db.Events.Where(c => c.Name.ToLower().Contains(name)).OrderBy(c => c.Id).Take(ItemsPerPage);
        }
    }
}