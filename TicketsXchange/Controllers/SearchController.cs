using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TicketsXchange.DTO;
using TicketsXchange.Models;
using Worldpay.Sdk;
using Worldpay.Sdk.Enums;
using Worldpay.Sdk.Models;

namespace TicketsXchange.Controllers
{
    public class SearchController : Controller
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();
        // GET: Search
        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories;
            ViewBag.Event = Request.QueryString["event"];
            ViewBag.Category = Request.QueryString["category"];
            ViewBag.Location = Request.QueryString["location"];
            return View();
        }
       //Detail page
        public ActionResult Detail(int? id, string modal)
        {
            //if the user is not logined, login is modal is popped out.
            ViewBag.modal = modal;
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }      
            return View(ticket);
        }
        //Order page
        public ActionResult Order(int? id)
        {
            if (Session["UserID"] as int? > 0)
            {
                Ticket ticket = db.Tickets.Find(id);
                if (ticket == null)
                {
                    return HttpNotFound();
                }

                return View(ticket);
            }
            else
            {   //User is not logined, it takes the user to the search page
                ViewBag.modal = "login";
                return RedirectToAction("Detail", "Search", new {id=id, modal = "login" });
            }
           
        }
        //Review Page
        public ActionResult Review(int? id)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.Amount = Request.QueryString["amount"];
            Session["TicketId"] = id;
            Session["Quantity"] = Convert.ToInt32(Request.QueryString["amount"]);
            Session["Payment_Total"] = (float)ticket.Price * Convert.ToInt32(Request.QueryString["amount"]);
            return View(ticket);
        }
        //Payment Page
        public ActionResult Payment(int? id)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }
        //Make payment when user submit the payment
        public ActionResult Pay(PayModelDTO data)
        {
            WorldpayRestClient restClient = new WorldpayRestClient("https://api.worldpay.com/v1", "T_S_29134e84-936f-4418-8e4e-7a757f307cc6");

            var orderRequest = new OrderRequest()
            {
                token = data.Token,
                amount = (int)(float.Parse( Convert.ToString(Session["Payment_Total"])) * 100),
                currencyCode = CurrencyCode.GBP.ToString(),
                name = "test name",
                orderDescription = "Order description",
                customerOrderCode = "Order code"
            };

            var address = new Address()
            {
                address1 = "123 House Road",
                address2 = "A village",
                city = "London",
                countryCode = CountryCode.GB.ToString(),
                postalCode = "EC1 1AA"
            };

            orderRequest.billingAddress = address;

            try
            {
                OrderResponse orderResponse = restClient.GetOrderService().Create(orderRequest);
                Console.WriteLine("Order code: " + orderResponse.orderCode);
                ViewBag.Status = "Success";
                ViewBag.Description = String.Format("Order Code: {0} <br/> Amount: £ {1} <br /> State: {2}"
                    , orderResponse.orderCode, (float)orderResponse.amount / 100, orderResponse.paymentStatus);

                Payment payment = new Payment { UserId = Session["UserID"] != null ? (int)(Session["UserID"] as int?) : -1,
                    TicketId = (int)(Session["TicketId"] as int?), Quantity = (int)(Session["Quantity"] as int?)
                                    , TotalPrice = (float)(Session["Payment_Total"] as float?), PaymentMethod = 0 ,
                            CreatedAt = DateTime.Now};
                db.Payments.Add(payment);
                db.SaveChanges();
                db.Tickets.Find(payment.TicketId).Balance -= payment.Quantity;
                db.SaveChanges();
            }
            catch (WorldpayException e)
            {
                ViewBag.Status = "Failure";
                ViewBag.Description = String.Format("Error Code: {0} <br/> Error Description: {1} <br /> Error Message: {2}"
                    , e.apiError.customCode, e.apiError.description, e.apiError.message);
                Console.WriteLine("Error code:" + e.apiError.customCode);
                Console.WriteLine("Error description: " + e.apiError.description);
                Console.WriteLine("Error message: " + e.apiError.message);
            }

            return View("~/Views/Search/payment.cshtml");
        }
    }
}