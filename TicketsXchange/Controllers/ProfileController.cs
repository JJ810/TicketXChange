using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TicketsXchange.Helper;
using TicketsXchange.Models;

namespace TicketsXchange.Controllers
{

    public class ProfileController : Controller
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();

        // GET: Profile
        public ActionResult Index()
        {
            var profiles = db.Profiles.Include(p => p.User);
            return View(profiles.ToList());
        }

        // GET: Profile/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // GET: Profile/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: Profile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,Gender,DOB,MobileNumber,PhoneNumber,AddressLine1,AddressLine2,AddressLine3,PostCode,City,State,Country")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Profiles.Add(profile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", profile.UserId);
            return View(profile);
        }
        
        // GET: Profile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id != Session["UserID"] as int?)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", profile.UserId);
            return View(profile);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FirstName,LastName,Gender,DOB,MobileNumber,PhoneNumber,AddressLine1,AddressLine2,AddressLine3,PostCode,City,State,Country")] Profile profile)
        {
            Random r = new Random();
            int random = r.Next();
            if (ModelState.IsValid)
            {
                db.Profiles.Find(profile.UserId).FirstName = profile.FirstName;
                db.Profiles.Find(profile.UserId).LastName = profile.LastName;
                db.Profiles.Find(profile.UserId).Gender = profile.Gender;
                db.Profiles.Find(profile.UserId).DOB = profile.DOB;
                db.Profiles.Find(profile.UserId).MobileNumber = profile.MobileNumber;
                db.Profiles.Find(profile.UserId).PhoneNumber = profile.PhoneNumber;
                db.Profiles.Find(profile.UserId).AddressLine1 = profile.AddressLine1;
                db.Profiles.Find(profile.UserId).AddressLine2 = profile.AddressLine2;
                db.Profiles.Find(profile.UserId).AddressLine3 = profile.AddressLine3;
                db.Profiles.Find(profile.UserId).PostCode = profile.PostCode;
                db.Profiles.Find(profile.UserId).City = profile.City;
                db.Profiles.Find(profile.UserId).State = profile.State;
                db.Profiles.Find(profile.UserId).Country = profile.Country;
            }
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = random + fileName;
                    file.SaveAs(Path.Combine(Server.MapPath("~/Content/upload/"), path));
                    profile.Photo = "~/Content/upload/" + path;
                    db.Profiles.Find(profile.UserId).Photo = profile.Photo;            
                }
            }
            db.SaveChanges();
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", profile.UserId);
            return View(db.Profiles.Find(profile.UserId));
        }

        // GET: Profile/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profile profile = db.Profiles.Find(id);
            db.Profiles.Remove(profile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
