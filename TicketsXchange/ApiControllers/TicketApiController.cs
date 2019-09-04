using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using TicketsXchange.DTO;
using TicketsXchange.Models;
using TicketsXchange.ViewModel;

namespace TicketsXchange.Controllers
{
    public class TicketApiController : ApiController
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();

        // GET: api/Ticket
        public IQueryable<Ticket> GetTickets()
        {       
            return db.Tickets;
        }

        public class SearchCriteria
        {
            public string Name { get; set; }
            public string Location { get; set; }
            public string Date { get; set; }
            public int MinPrice  { get; set; }
            public int MaxPrice { get; set; }
            public int Page { get; set; }
            public int[] Categories { get; set; }
            public int Sort { get; set; }
        }


        [Route("api/TicketSearch")]
        public IHttpActionResult Search([FromBody]SearchCriteria request)
        {
            const int ItemsPerPage = 5;

            string makeLower(string x) => String.IsNullOrEmpty(x) ? "" : x.ToLower();
            request.Location = makeLower(request.Location);
            request.Name = makeLower(request.Name);
            request.Date = makeLower(request.Date);
            List<SearchTicketDTO> list = new List<SearchTicketDTO>();
            IQueryable<Ticket> result;
            int totalRows = db.Tickets.Where(c => c.Price > request.MinPrice && c.Price < request.MaxPrice
                && request.Categories.Contains((int)c.CategoryId) && c.Location.ToLower().Contains(request.Location)
                && c.Name.ToLower().Contains(request.Name) && c.Date.ToLower().Contains(request.Date)).Count();
            if (request.Sort == 0)
            {
                result = db.Tickets.Where(c => c.Price > request.MinPrice && c.Price < request.MaxPrice
                && request.Categories.Contains((int)c.CategoryId) && c.Location.ToLower().Contains(request.Location)
                && c.Name.ToLower().Contains(request.Name) && c.Date.ToLower().Contains(request.Date)).OrderBy(c => c.Price).Skip(request.Page * ItemsPerPage).Take(ItemsPerPage);
            }
            else
            {
                result = db.Tickets.Where(c => c.Price > request.MinPrice && c.Price < request.MaxPrice
                && request.Categories.Contains((int)c.CategoryId) && c.Location.ToLower().Contains(request.Location)
                && c.Name.ToLower().Contains(request.Name)).OrderByDescending(c => c.Price).Skip(request.Page * ItemsPerPage).Take(ItemsPerPage);
            }
            foreach (var ticket in result)
            {
                SearchTicketDTO item = new SearchTicketDTO();
                item.Id = ticket.Id;
                item.Name = ticket.Name;
                item.Location = ticket.Location;
                item.Price = (float)ticket.Price;
                item.Date = ticket.Date;
                item.IsSold = ticket.Balance > 0 ? false : true;
                list.Add(item);
            }
            Dictionary<string, Object> res = new Dictionary<string, Object>();
            res.Add("Result", list);
            res.Add("TotalRows", totalRows);
            res.Add("CurrentPage", request.Page);
            return Json(res);
        }
        [HttpGet]
        [Route("api/TicketNameSearch")]
        public IQueryable<Ticket> NameSearch([FromUri]String name)
        {
            const int ItemsPerPage = 5;

            string makeLower(string x) => String.IsNullOrEmpty(x) ? "" : x.ToLower();
            name = makeLower(name);
           
            return db.Tickets.Where(c =>  c.Name.ToLower().Contains(name)).OrderBy(c => c.Price).Take(ItemsPerPage);
        }
        // GET: api/Ticket/5
        [ResponseType(typeof(Ticket))]
        public IHttpActionResult GetTicket(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }
        [HttpPost]
        [Route("api/Ticket/UploadPhoto")]
        public async Task<string> UploadPhoto()
        {
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach(var file in provider.FileData)
                {
                    var name = file.Headers.ContentDisposition.FileName;
                    name = name.Trim('"');
                    var localFileName = file.LocalFileName;
                    var filePath = Path.Combine(root, name);
                    File.Move(localFileName, filePath);
                }

            }catch(Exception e)
            {
                return $"Error: {e.Message}";
            }
            return "OK";
        }
        // PUT: api/Ticket/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTicket(int id, Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ticket.Id)
            {
                return BadRequest();
            }

            db.Entry(ticket).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
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

        // POST: api/Ticket
        [ResponseType(typeof(Ticket))]
        public IHttpActionResult PostTicket(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tickets.Add(ticket);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = ticket.Id }, ticket);
        }

        // DELETE: api/Ticket/5
        [ResponseType(typeof(Ticket))]
        public IHttpActionResult DeleteTicket(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            db.Tickets.Remove(ticket);
            db.SaveChanges();

            return Ok(ticket);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TicketExists(int id)
        {
            return db.Tickets.Count(e => e.Id == id) > 0;
        }
        
        [System.Web.Http.Route("api/Ticket/Admin")]
        public IHttpActionResult Admin([FromBody] TicketDTO request)
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
            List<TicketDTO> list = new List<TicketDTO>();
            if (action == "list")
            {
                foreach (var ticket in db.Tickets.OrderBy(a => a.Id).Skip(start).Take(page))
                {
                    TicketDTO item = new TicketDTO();
                    item.Id = ticket.Id;
                    item.CategoryId = (int)ticket.CategoryId;
                    item.Name = ticket.Name;
                    item.UserId = (int)ticket.UserId;
                    item.CategoryId = (int)ticket.CategoryId;
                    item.Details = ticket.Details;
                    item.Date = ticket.Date;
                    item.Location = ticket.Location;
                    item.Price = (float)ticket.Price;
                    item.PaymentMethod = (int)ticket.PaymentMethod;
                    item.Featured = (int)(ticket.Featured == null ? 0 : ticket.Featured);
                    item.Active = (int)(ticket.Active == null?0:ticket.Active);
                    item.Balance = (int)ticket.Balance;
                //    item.CreatedAt = String.Format("{0:dd/MM/yyyy}", ticket.CreatedAt);
                    list.Add(item);
                }
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Records", list);
                res.Add("TotalRecordCount", db.Tickets.Count());
                return Json(res);
            }
            else if (action == "update")
            {
                Ticket item = db.Tickets.Find(request.Id);
                item.CategoryId = (int)request.CategoryId;
                item.Name = request.Name;
                item.UserId = request.UserId;
                item.CategoryId = request.CategoryId;
                item.Details = request.Details;
                item.Date = request.Date;
                item.Location = request.Location;
                item.PaymentMethod = (int)request.PaymentMethod;
                item.Featured = (byte)request.Featured;
                item.Active = (byte)request.Active;
                item.Price = (float)request.Price;
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if (action == "delete")
            {
                db.Tickets.Remove(db.Tickets.Find(request.Id));
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if (action == "create")
            {
                Ticket item = new Ticket();
                item.CategoryId = (int)request.CategoryId;
                item.Name = request.Name;
                item.Details = request.Details;
                item.CategoryId = request.CategoryId;
                item.Date = request.Date;
                item.Location = request.Location;
                item.PaymentMethod = (int)request.PaymentMethod;
                item.Featured = (byte)request.Featured;
                item.Active = (byte)request.Active;
                item.CreatedAt = DateTime.Now;
                item.Balance = request.Balance;
                item.UserId = request.UserId;
                db.Tickets.Add(item);
                db.SaveChanges();               
                request.Id = item.Id;
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Record", request);
                return Json(res);
            }
            return Json(1);
        }
   
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/TicketName/GetJson")]
        public IHttpActionResult GetJson()
        {
            List<JsonDataDTO> list = new List<JsonDataDTO>();
            foreach (var ev in db.Tickets)
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
    }
}