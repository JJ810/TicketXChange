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
using TicketsXchange.DTO;

namespace TicketsXchange.Controllers
{
    public class CategoryApiController : ApiController
    {
        private TicketsXchangeEntities db = new TicketsXchangeEntities();

        // GET: api/Category

        public IQueryable<Category> GetCategory()
        {
            return db.Categories;
        }

        // GET: api/Category/5
        [ResponseType(typeof(Category))]
        public IHttpActionResult GetCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
        // POST: api/Category
        public IHttpActionResult PostCateogry([FromBody] Category request)
        {

            if (db.Categories.Where(a => a.Name == request.Name).FirstOrDefault() != null)
            {
                return Ok(-3);
            }
            Category category = new Category { Name = request.Name};
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Categories.Add(category);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
               return Ok(-2);
                
            }
         
            return Ok(category);
        }
        // PUT: api/Category/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCategory(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.Id)
            {
                return BadRequest();
            }

            db.Entry(category).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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



        // DELETE: api/Category/5
        [ResponseType(typeof(Category))]
        public IHttpActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return Ok(category);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return db.Categories.Count(e => e.Id == id) > 0;
        }
     
        [System.Web.Http.Route("api/Category/Admin")]
        public IHttpActionResult Admin([FromBody] Category request)
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
            List<CategoryDTO> list = new List<CategoryDTO>();
            if (action == "list")
            {
                foreach(var category in db.Categories.OrderBy(a => a.Id).Skip(start).Take(page))
                {
                    CategoryDTO item = new CategoryDTO();
                    item.Id = category.Id;
                    item.Name = category.Name;
                    item.Photo = category.Photo;
                    list.Add(item);
                }
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Records", list);
                res.Add("TotalRecordCount", list.Count());
                return Json(res);
            }
            else if (action == "update")
            {
                Category category = db.Categories.Find(request.Id);
                category.Name = request.Name;
               
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if (action == "delete")
            {
                db.Categories.Remove(db.Categories.Find(request.Id));
                db.SaveChanges();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                return Json(res);
            }
            else if (action == "create")
            {
                Category category = new Category();
                category.Name = request.Name;
                db.Categories.Add(category);
                db.SaveChanges();
                request.Id = category.Id;
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                res.Add("Result", "OK");
                res.Add("Record", request);
                return Json(res);
            }
            return Json(1);
        }

        [HttpPost]
        [System.Web.Http.Route("api/Category/GetJson")]
        public IHttpActionResult GetJson()
        {
            List<JsonDataDTO> list = new List<JsonDataDTO>();
            foreach(var category in db.Categories)
            {
                JsonDataDTO data = new JsonDataDTO();
                data.DisplayText = category.Name;
                data.Value = category.Id;
                list.Add(data);
            }
            Dictionary<string, Object> res = new Dictionary<string, Object>();
            res.Add("Result", "OK");
            res.Add("Options", list);
            return Json(res);
        }
        [HttpGet]
        [Route("api/CategorySearch")]
        public IQueryable<Category> NameSearch([FromUri]String name)
        {
            const int ItemsPerPage = 5;

            string makeLower(string x) => String.IsNullOrEmpty(x) ? "" : x.ToLower();
            name = makeLower(name);

            return db.Categories.Where(c => c.Name.ToLower().Contains(name)).OrderBy(c => c.Id).Take(ItemsPerPage);
        }
    }
}