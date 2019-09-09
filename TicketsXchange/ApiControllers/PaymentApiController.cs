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
    public class PaymentApiController : ApiController
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();


        //CRUD for payments page of admin
        [System.Web.Http.Route("api/Payment/Admin")]
        public IHttpActionResult Admin([FromBody] PaymentDTO request)
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
            List<PaymentDTO> list = new List<PaymentDTO>();
            if (action == "list")
            {
                foreach (var payment in db.Payments.OrderBy(a => a.Id).Skip(start).Take(page))
                {
                    PaymentDTO item = new PaymentDTO();
                    item.Id = payment.Id;
                    item.TicketId = (int)payment.TicketId;
                    item.Quantity = payment.Quantity;
                    item.TotalPrice = (float)payment.TotalPrice;
                    item.PaymentMethod = payment.PaymentMethod;
                    item.CreatedAt = String.Format("{0:dd/MM/yyyy}", payment.CreatedAt);
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
                Payment item = db.Payments.Find(request.Id);
                item.TicketId = request.TicketId;
                item.Quantity = request.Quantity;
                item.TotalPrice = request.TotalPrice;
                item.PaymentMethod = request.PaymentMethod;
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
                Payment item = new Payment();
                item.TicketId = request.TicketId;
                item.Quantity = request.Quantity;
                item.TotalPrice = request.TotalPrice;
                item.PaymentMethod = request.PaymentMethod;
                item.CreatedAt = DateTime.Now;
                db.Payments.Add(item);
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