
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Models
{
    public class ProductCart
    {
        WebMobileEntities db = null;

        public ProductCart()
        {
            db = new WebMobileEntities();
        }
        public Product ViewDetail(int ProductID)
        {
            return db.Products.Find(ProductID);
        }
    }
}