using PagedList;
using System;
using System.Collections.Generic;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanDienThoai.Models;
using Gremlin.Net.Process.Traversal;

namespace WebBanDienThoai.Controllers
{
    public class TurnoverController : Controller
    {
        private WebMobileEntities db = new WebMobileEntities();
        // GET: Turnover
        public ActionResult Doanhthu(string q)
        {
            decimal sum = 0;
            decimal sumIn = 0;
            var count_order = (from or in db.Orders select or.orderID).Count();
            ViewBag.count_product = count_order;
            var model = db.Orders.Where(x => x.Status == 3).ToList();
            if (!string.IsNullOrEmpty(q))
            {
                int a = Convert.ToInt32(q);
                model = model.Where(x => x.CreatedAt.Value.Month == a).ToList();
                foreach (var iteam in model)
                {

                    sum += iteam.TotalMoney;
                    var model2 = db.OrderDetails.Where(x => x.orderID == iteam.orderID).ToList();
                    foreach (var iteam2 in model2)
                    {
                        var model3 = db.Products.Where(x => x.ProductID == iteam2.ProductID).SingleOrDefault();
                        sumIn += (decimal)model3.PriceIn;
                    }


                }
                decimal DoanhThuOut = sum - sumIn;

                ViewBag.TotalMoney = sum;
                ViewBag.TotalMoney1 = sumIn;
                ViewBag.TotalMoney2 = DoanhThuOut;
            }

            var printorder = from i in db.Configures select new ConfigModel { Logo = i.Logo, Address_NameCompany = i.Address, Hotline = i.Hotline, Email_config = i.Email, NameCompany = i.NameCompany };
            ViewBag.printorder = printorder.ToList();
            ViewBag.keyword_search = q;
            return View(model);
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