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
    public class AccountController : Controller
    {
        // GET: Account
        private WebMobileEntities db = new WebMobileEntities();
        public ActionResult Index(int id)
        {

            var model = db.Customers.Where(x => x.customerID == id).FirstOrDefault();
            return View(model);

        }

        public ActionResult Edit(int id)
        {
            return View(db.Customers.Where(x => x.customerID == id).FirstOrDefault());

        }
        [HttpPost]
        public ActionResult Edit(Customer cus, int id)
        {
            try
            {
                db.Entry(cus).State = EntityState.Modified;


                db.SaveChanges();
                return RedirectToAction("Index", "Account", new { id = id });
            }
            catch { }
            return View(cus);
        }
        public ActionResult History(int id)
        {
            var model = db.Orders.Where(x => x.customerID == id).ToList();
            return View(model);

        }
        public ActionResult Details(int id)
        {
            var model = db.OrderDetails.Where(x => x.orderID == id).ToArray();
            return View(model);
        }
        public ActionResult huyDonhang(int id)
        {

            Order order = db.Orders.Find(id);
            order.Status = 2;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("History", new { id = order.customerID });
        }

        public ActionResult VoucherAvailable(int? page, string q)
        {
            var count_voucher = (from cate in db.Voucher select cate.IdVoucher).Count();

            ViewBag.count_cate = count_voucher;




            var model = from p in db.Voucher where p.Status == true orderby p.CreatedAt descending select p;
            int pagesize = 15;
            int pageNumber = (page ?? 1);


            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(x => x.NameVoucher.Contains(q)).OrderByDescending(x => x.CreatedAt);
            }

            ViewBag.keyword_search = q;

            return View(model.ToPagedList(pageNumber, pagesize));
        }

    }
}













/*using PagedList;
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
    public class AccountController : Controller
    {
        // GET: Account
        private WebMobileEntities db = new WebMobileEntities();
        public ActionResult Index(int id)
        {

            var model = db.Customers.Where(x => x.customerID == id).FirstOrDefault();
             return View(model);

        }
     
        public ActionResult Edit(int id)
        {
            return View( db.Customers.Where(x => x.customerID == id).FirstOrDefault());
            
        }
        [HttpPost]
        public ActionResult Edit(Customer cus, int id)
        {
            try
            {
                db.Entry(cus).State = EntityState.Modified;


                db.SaveChanges();
                return RedirectToAction("Index", "Account", new { id = id });
            }
            catch { }
            return View(cus);
        }
        public ActionResult History(int id)
        {
            var model = db.Orders.Where(x => x.customerID == id).ToList();
            return View(model);

        }
        public ActionResult Details(int id)
        {
            var model = db.OrderDetails.Where(x => x.orderID == id).ToArray();
            return View(model);
        }
        public ActionResult huyDonhang(int id)
        {
            
            Order order = db.Orders.Find(id);
            order.Status = 2;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("History", new { id = order.customerID });
        }

        public ActionResult VoucherAvailable(int? page, string q)
        {
            var count_voucher = (from cate in db.Voucher select cate.IdVoucher).Count();

            ViewBag.count_cate = count_voucher;




            var model = from p in db.Voucher where p.Status==true orderby p.CreatedAt descending select p;
            int pagesize = 15;
            int pageNumber = (page ?? 1);


            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(x => x.NameVoucher.Contains(q)).OrderByDescending(x => x.CreatedAt);
            }

            ViewBag.keyword_search = q;

            return View(model.ToPagedList(pageNumber, pagesize));
        }

    }
}
*/