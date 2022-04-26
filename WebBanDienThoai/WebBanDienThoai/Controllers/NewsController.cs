using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Controllers
{
    public class NewsController : Controller
    {
        private WebMobileEntities db = new WebMobileEntities();
        public ActionResult Index(int? page, string q)
        {
            var count_new = (from cate in db.News select cate.IdNew).Count();

            ViewBag.count_cate = count_new;




            var model = from p in db.News orderby p.CreatedAt descending select p;
            int pagesize = 15;
            int pageNumber = (page ?? 1);


            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(x => x.NameNew.Contains(q)).OrderByDescending(x => x.CreatedAt);
            }

            ViewBag.keyword_search = q;

            return View(model.ToPagedList(pageNumber, pagesize));
        }
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(New news, bool status_mi, HttpPostedFileBase image_avatar)
        {
            if (ModelState.IsValid)
            {


                bool tus;
                if (status_mi == true)
                {
                    tus = true;
                }
                else
                {
                    tus = false;
                }
                var check_id = db.News.Count(x => x.IdNew == news.IdNew);
                Random ran = new Random();
                int Id = ran.Next(1, 999);
                    while(Id==check_id)
                {
                   
                     Id = ran.Next(1, 999);
                }
                news.IdNew = Id; 
                news.Author = Session["Username"].ToString();
                news.CreatedAt = DateTime.Now;
                news.Status = tus;


                var check_name = db.News.Count(x => x.NameNew == news.NameNew);
                var check_alias = db.News.Count(x => x.Alias == news.Alias);
                if (check_name > 0)
                {
                    ViewBag.check_name = "Tên Tin Tức Đã Tồn Tại";
                }
                else if (check_alias > 0)
                {
                    ViewBag.check_alias = "Đường Dẫn URL Đã Tồn Tại";
                }
                else
                {

                    db.News.Add(news);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }


            return View(news);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }

            return View(news);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(New news, HttpPostedFileBase image_avatars)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }



        public ActionResult Delete(int? id)
        {

            New news = db.News.Find(id);

            db.News.Remove(news);
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