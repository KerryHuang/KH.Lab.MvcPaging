using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcPagingSample.Models;
using MvcPaging;
using Newtonsoft.Json;

namespace MvcPagingSample.Controllers
{
    public class ProductsController : Controller
    {
        private OAuthTEntities db = new OAuthTEntities();
        private int DefaultPageSize = 8;

        public List<Product> Product
        {
            get
            {
                return this.db.Product.OrderByDescending(x=> x.ClickRate).ThenByDescending(x => x.CreateTime).ToList();
            }
        }

        // GET: Products
        public ActionResult Index()
        {
            return View(db.Product.ToList());
        }

        // GET: Products/IndexAjax
        public ActionResult IndexAjax()
        {
            int currentPageIndex = 0;
            int skip = 0;
            var products = Product.Skip(skip).Take(DefaultPageSize).ToPagedList(currentPageIndex, DefaultPageSize, Product.Count());
            return View(products); 
        }

        // GET: Products/IndexAjax
        public ActionResult AjaxPage(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int skip = ((currentPageIndex + 1) * DefaultPageSize) - DefaultPageSize;
            var products = this.Product.Skip(skip).Take(DefaultPageSize).ToPagedList(currentPageIndex, DefaultPageSize, Product.Count());
            return PartialView("_ProductGrid", products);
        }

        // GET: Products/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Price,Description,ImageUrl,ClickRate,CreateTime")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.Id = Guid.NewGuid();
                product.ClickRate = 0;
                product.CreateTime = DateTime.Now;
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price,Description,ImageUrl,ClickRate,CreateTime")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Click(Guid? id)
        {
            Dictionary<string, string> jo = new Dictionary<string, string>();
            if (id == null)
            {
                jo.Add("Valid", "Fail");
                //jo.Add("Msg", "請輸入產品ID編號.");
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                jo.Add("Valid", "Fail");
                //jo.Add("Msg", "查產品ID編號.");
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            product.ClickRate += 1;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            jo.Add("Valid", "OK");
            return Content(JsonConvert.SerializeObject(jo), "application/json");
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
