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
    public class UserController : Controller
    {
        // GET: Voucher
        private WebMobileEntities db = new WebMobileEntities();
        public ActionResult Index(int? page, string q)
        {
            var count_user = (from cate in db.Users select cate.UserID).Count();

            ViewBag.count_cate = count_user;




            var model = from p in db.Users orderby p.CreatedAt descending select p;
            int pagesize = 15;
            int pageNumber = (page ?? 1);


            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(x => x.Fullname.Contains(q)).OrderByDescending(x => x.CreatedAt);
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
        public ActionResult Create(User _user, bool status_mi, string status_role, HttpPostedFileBase image_avatar)
        {
      
            


              

                var check_name = db.Users.Count(x => x.Email == _user.Email);
              
                if (check_name > 0)
                {
                    ViewBag.check_name = "Tên Sản Phẩm  Đã Tồn Tại";
                }
                
                else
                {

                    if (image_avatar != null)
                    {
                        var filename = Path.GetFileName(image_avatar.FileName);
                        var path = Path.Combine(Server.MapPath("~/Upload/Images"), filename);


                        image_avatar.SaveAs(path);
                        _user.Image = "/Upload/Images/" + image_avatar.FileName;
                    }
                    else
                    {
                        _user.Image = "";
                    }
                    bool tus;
                    if (status_mi == true)
                    {
                        tus = true;
                    }
                    else
                    {
                        tus = false;
                    }
                   

                    _user.CreatedAt = DateTime.Now;
                    _user.Role =Convert.ToInt32(status_role);
                    _user.Status = tus;


                    db.Users.Add(_user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            

            
            return View(_user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User _user = db.Users.Find(id);
            if (_user == null)
            {
                return HttpNotFound();
            }
           
            return View(_user);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User _user, HttpPostedFileBase image_avatars)
        {
            

                if (image_avatars != null)
                {
                    var filename = Path.GetFileName(image_avatars.FileName);
                    var path = Path.Combine(Server.MapPath("~/Upload/Images"), filename);
                    image_avatars.SaveAs(path);
                    _user.Image = "/Upload/Images/" + image_avatars.FileName;
                }
                else
                {
                    _user.Image = "";
                }



           
                db.Entry(_user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            
          
            return View(_user);
        }



       /* public ActionResult Delete(int? id)
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
*/
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