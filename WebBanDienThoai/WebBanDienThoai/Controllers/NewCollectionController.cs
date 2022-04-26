using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Controllers
{
    public class NewCollectionController : Controller
    {
        private WebMobileEntities db = new WebMobileEntities();

        // GET: Category
        public ActionResult News(int? page)
        {

            var model = from p in db.News where p.Status == true orderby p.CreatedAt descending select p;
            int pagesize = 20;
            int pageNumber = (page ?? 1);

            var count_news = db.News.Where(x => x.Status == true).Count();
            ViewBag.count_news = count_news;
            return View(model.ToPagedList(pageNumber, pagesize));
        }


        

    }
}