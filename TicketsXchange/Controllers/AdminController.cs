using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using MailChimp;
using MailChimp.Helper;
using MailChimp.Lists;
using RestSharp;
using RestSharp.Authenticators;
using TicketsXchange.Helper;
using TicketsXchange.Models;

using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace TicketsXchange.Controllers
{
    public class AdminController : Controller
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();

        public ActionResult Index()
        {
            ViewBag.Page = "Index";
            if (Session["UserID"] as int? > 0)
            {
                if (db.Users.Find(Session["UserID"]).Role == 0)
                {
                    ViewBag.TotalUsers = db.Users.Where(a => a.Role == 1).Count();
                    ViewBag.ActiveUsers = db.Users.Where(a => a.Active == 1).Count();
                    ViewBag.TotalTickets = db.Tickets.Count();
                    ViewBag.FeaturedTickets = db.Tickets.Where(a => a.Featured == 1).Count();
                    
                    return View(); 
                }
            }                
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Logout()
        {
            Session["UserID"] = -1;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Mailbox()
        {
            ViewBag.Page = "Mailbox";
            ViewBag.Mails = db.Contacts;
            return View();
        }
        public ActionResult Users()
        {
            ViewBag.Page = "Users";
            return View();
        }
        public ActionResult Categories()
        {
            ViewBag.Page = "Categories";
            return View();
        }
        public ActionResult Events()
        {
            ViewBag.Page = "Events";
            return View();
        }
        public ActionResult Tickets()
        {
            ViewBag.Page = "Tickets";
            return View();
        }
        public ActionResult Payments()
        {
            ViewBag.Page = "Payments";
            return View();
        }
        
        public ActionResult Subscribe()
        {
            ViewBag.Page = "Subscribe";
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }          

    }
}