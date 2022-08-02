using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanDienThoai.Models;
namespace WebBanDienThoai.Controllers
{
    public class VoucherController : Controller
    {
        // GET: Voucher
        private WebMobileEntities db = new WebMobileEntities();
        public ActionResult Index(int? page, string q)
        {
            var count_voucher = (from cate in db.Voucher select cate.IdVoucher).Count();

            ViewBag.count_cate = count_voucher;




            var model = from p in db.Voucher orderby p.CreatedAt descending select p;
            int pagesize = 15;
            int pageNumber = (page ?? 1);


            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(x => x.NameVoucher.Contains(q)).OrderByDescending(x => x.CreatedAt);
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
        public ActionResult Create(Voucher voucher, bool status_mi, HttpPostedFileBase image_avatar)
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

                voucher.Author = Session["Username"].ToString();
                voucher.CreatedAt = DateTime.Now;
                voucher.Status = tus;


                var check_name = db.Voucher.Count(x => x.NameVoucher == voucher.NameVoucher);
                var check_alias = db.Voucher.Count(x => x.Alias == voucher.Alias);
                if (check_name > 0)
                {
                    ViewBag.check_name = "Tên Sản Phẩm  Đã Tồn Tại";
                }
                else if (check_alias > 0)
                {
                    ViewBag.check_alias = "Đường Dẫn URL Đã Tồn Tại";
                }
                else
                {

                    if (image_avatar != null)
                    {
                        var filename = Path.GetFileName(image_avatar.FileName);
                        var path = Path.Combine(Server.MapPath("~/Upload/Images"), filename);


                        image_avatar.SaveAs(path);
                        voucher.Images = "/Upload/Images/" + image_avatar.FileName;
                    }
                    else
                    {
                        voucher.Images = "";
                    }

                    db.Voucher.Add(voucher);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }

            
            return View(voucher);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher Voucher = db.Voucher.Find(id);
            if (Voucher == null)
            {
                return HttpNotFound();
            }
           
            return View(Voucher);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Voucher voucher, HttpPostedFileBase image_avatars)
        {
            if (ModelState.IsValid)
            {


                if (image_avatars != null)
                {
                    var filename = Path.GetFileName(image_avatars.FileName);
                    var path = Path.Combine(Server.MapPath("~/Upload/Images"), filename);
                    image_avatars.SaveAs(path);
                    voucher.Images = "/Upload/Images/" + image_avatars.FileName;
                }
                else
                {
                    voucher.Images = "";
                }




                db.Entry(voucher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          
            return View(voucher);
        }



        public ActionResult Delete(int? id)
        {

            Voucher voucher = db.Voucher.Find(id);

            if (voucher.Images != "")
            {
                var filename = Path.GetFileName(voucher.Images);
                System.IO.File.Delete(Request.PhysicalApplicationPath + "/Upload/Images/" + filename);
            }

            db.Voucher.Remove(voucher);
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