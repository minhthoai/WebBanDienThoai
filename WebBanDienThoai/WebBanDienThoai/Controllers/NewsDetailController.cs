using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Controllers
{
    public class NewsDetailController : Controller
    {
        private WebMobileEntities db = new WebMobileEntities();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewsHome(string Alias, int IdNew)
        {
            var model = from p in db.News
                        where p.Alias == Alias && p.Status == true
                        select new New
                        {
                            IdNew = p.IdNew,
                            NameNew = p.NameNew,
                            Alias = p.Alias,
                            Content = p.Content,
                            Author = p.Author,
                            MetaTitle = p.MetaTitle,
                            MetaKeyword = p.MetaKeyword,

                            MetaDescription = p.MetaDescription,
                            CreatedAt = p.CreatedAt,
                            Status = p.Status,
                            DescriptShort = p.DescriptShort,



                        };


            var news_related = from p in db.News
                               join c in db.News
                               on p.IdNew equals c.IdNew
                               where p.IdNew == IdNew
                               orderby Guid.NewGuid()
                               select new New
                               {
                                   IdNew = p.IdNew,
                                   NameNew = p.NameNew,
                                   Alias = p.Alias,
                                   Content = p.Content,
                                   Author = p.Author,
                                   MetaTitle = p.MetaTitle,
                                   MetaKeyword = p.MetaKeyword,

                                   MetaDescription = p.MetaDescription,
                                   CreatedAt = p.CreatedAt,
                                   Status = p.Status,
                                   DescriptShort = p.DescriptShort,


                               };

            ViewBag.news_related = news_related.ToList();


            var getbread = from p in db.News
                           where p.IdNew == IdNew
                           select new New
                           {
                               IdNew = p.IdNew,
                               NameNew = p.NameNew
                           };

            ViewBag.getbread = getbread.ToList();


            return View(model.ToList());
        }
    }
}