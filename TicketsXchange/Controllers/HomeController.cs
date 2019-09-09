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
using TicketsXchange.DTO;

namespace TicketsXchange.Controllers
{
    public class HomeController : Controller
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();

        public ActionResult Index(string modal, string message, int userId = -1)
        {
            ViewBag.Page = "Home";
            ViewBag.modal = modal;
            ViewBag.message = message;
            ViewBag.UserId = userId;
            ViewBag.Categories = db.Categories.OrderBy(a=>a.Id).Take(6);
            ViewBag.Features = db.Tickets.Where(a => a.Featured == 1).OrderBy(a => a.Id).Take(3);
            ViewBag.ActiveEvents = db.Tickets.Where(a => a.Active == 1).Count();
            ViewBag.ActiveUsers = db.Users.Where(a => a.Active == 1).Count();
            ViewBag.SoldTickets = db.Payments.Sum(a=>a.Quantity);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }   

        public ActionResult Login([FromBody] User request)
        {
            var password = request.Password;
            User user = db.Users.Where(a => a.Email == request.Email).FirstOrDefault();
            string modal = "", message = "";
            
            if (user == null)
            {
                modal = "login";
                message = "User Not Found";
            }
            else
            {
                if(user.Verified != 1)
                {
                    modal = "login";
                    message = "Not Verified";
                }
                else
                {
                    if (PasswordHelper.verifyMd5Hash(request.Password, user.Password))
                    {
                        Session["UserID"] = user.Id;
                        if (user.Role == 0)
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            var controller = (Request.UrlReferrer.Segments.Skip(1).Take(1).SingleOrDefault() ?? "Home").Trim('/');
                            var action = (Request.UrlReferrer.Segments.Skip(2).Take(1).SingleOrDefault() ?? "Index").Trim('/');
                            if (controller == "Search" && action == "Detail") {
                                var ticketId = (Request.UrlReferrer.Segments.Skip(3).Take(1).SingleOrDefault()).Trim('/');
                                return RedirectToAction("Order", "Search", new { id=ticketId });
                            }                          
                        }                        
                    }
                    else
                    {
                        modal = "login";
                        message = "Incorrect Password";
                    }
                }                
            }
           
            return RedirectToAction("Index", "Home", new { modal = modal, message = message});
        }

        public ActionResult Logout()
        {
            Session["UserID"] = -1;
            return Redirect("Index");
        }
    

        public ActionResult Confirm([FromUri] string activationCode)
        {
            User user = db.Users.Where(a => a.ActivationCode == activationCode).FirstOrDefault();
            string modal = "";
            if(user != null)
            {
                user.Verified = 1;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                modal = "confirm";
            }
            return RedirectToAction("Index", "Home", new { modal= modal});
        }

        public ActionResult Forgot(string activationCode)
        {
            User user = db.Users.Where(a => a.ActivationCode == activationCode).FirstOrDefault();
            string modal = "reset";
            return RedirectToAction("Index", "Home",new { modal = modal, message = "", userId = user.Id });
        }
        public ActionResult ForgotPassword([FromBody] User request)
        {
            User user = db.Users.Where(a => a.Email == request.Email).FirstOrDefault();

            if(user != null)
            {
                EmailHelper.SendForgotPasswordMail(user.Email, user.ActivationCode);
            }
           
            return RedirectToAction("Index");
        }
 
        
        public ActionResult ResetPassword([FromBody] ResetFormDTO request)
        {
            var password = request.Password;
            User user = db.Users.Where(a => a.Id == request.Id).FirstOrDefault();
            string modal, message = "";
            if (user == null)
            {
                modal = "login";
                message = "User Not Found";
            }
            else
            {
                user.Password = PasswordHelper.getMd5Hash(request.Password);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                modal = "success";
                message = "Password is reset.";
            }

            return RedirectToAction("Index", "Home", new { modal = modal, message = message });
        }
  
        public ActionResult Contact([FromBody] Models.Contact contact)
        {
            contact.CreatedAt = DateTime.Now;
            db.Contacts.Add(contact);
            db.SaveChanges();
            return RedirectToAction("Index", "Home", new { modal = "success", message = "Your information was sent successfully." });
        }

        public ActionResult Subscribe([FromBody] SubscribeEmail mail)
        {
            mail.CreatedAt = DateTime.Now;
            db.SubscribeEmails.Add(mail);
            db.SaveChanges();
            EmailHelper.SendSubscribeMail(mail.Email);
            return RedirectToAction("Index", "Home", new { modal = "success", message = "You are subscribed successfully." });
        }


    }
}