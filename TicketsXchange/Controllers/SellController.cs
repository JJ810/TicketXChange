using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TicketsXchange.Models;
using TicketsXchange.ViewModel;

namespace TicketsXchange.Controllers
{
    public class SellController : Controller
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();
        // GET: Sell
        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories;
            if (Session["UserID"] as int? > 0)
            {                
                return View();
            }
            else
            {
                ViewBag.modal = "login";
                return RedirectToAction("Index", "Home", new { modal="login"});
            }            
        }
        public ActionResult Photo(int? id)
        {
            ViewBag.CategoryId = id;
            return View();
        }

        [System.Web.Http.HttpPost]
        public ActionResult Add([FromBody] SellForm request)
        {
            Ticket ticket = new Ticket
            {
                UserId = Session["UserId"] as int?,
                CategoryId = request.CategoryId,
                Name = request.Name,
                Details = request.Details,
                Date = request.Date,
                Price = request.Price,
                Featured = 0,
                Active = 0,
                Location = request.Location,
                Balance = request.Quantity,
                PaymentMethod = request.PaymentMethod,
                CreatedAt = DateTime.Now
            };
            db.Tickets.Add(ticket);
            db.SaveChanges();
            return RedirectToAction("Detail", "Search", new {ticket.Id});
        }
        public ActionResult Submit(int? id)
        {
            ViewBag.CategoryId = id;
            return View();
        }
    }
}