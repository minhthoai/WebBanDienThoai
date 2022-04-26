using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Controllers
{
    public class AdminHomeController : Controller
    {
        // GET: AdminHome
        private WebMobileEntities db = new WebMobileEntities();
        public ActionResult Index()
        {
            var count_user = (from pro in db.Users select pro.UserID).Count();

            ViewBag.count_user = count_user;

            var count_product = (from pro in db.Products select pro.ProductID).Count();

            ViewBag.count_product = count_product;
            var count_order = (from or in db.Orders where or.ViewStatus == false select or.orderID).Count();

            if (count_order > 0)
            {
                Session["countnewcart"] = count_order;
            }


            var newproduct = db.Products.Where(x => x.Status == true).OrderByDescending(x => x.CreatedAt).Take(10);
            ViewBag.newproduct = newproduct.ToList();


            return View();
        }

        public ActionResult LogoutUser()
        {
            Session["UserID"] = null;
            Session["Username"] = null;
            Session["Password"] = null;
            Session["Image"] = null;
            Session["role"] = null;
            return Redirect("~/Login/LoginAccount");
        }

        public ActionResult HomeAuthorPermission()
        {
            return View();
        }


    }
}