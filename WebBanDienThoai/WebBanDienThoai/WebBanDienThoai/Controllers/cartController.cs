using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Controllers
{
    public class cartController : Controller
    {
        // GET: cart
        private WebMobileEntities db = new WebMobileEntities();

        public ActionResult Index()
        {
            var session_cart = Session["CartItem"];
            var list = new List<CartModel>();
            if (session_cart != null)
            {
                list = (List<CartModel>)session_cart;
            }
            return View(list);
        }

        public JsonResult AddCart(int ProductID, int Quanlity)
        {
            var product = new ProductCart().ViewDetail(ProductID);
            var Pd = db.Products.Where(x => x.ProductID == ProductID).SingleOrDefault();
            var session_cart = Session["CartItem"];
            if (Pd.Quanlity > Quanlity)
            {

                if (session_cart != null)
                {
                    var list = (List<CartModel>)session_cart;

                    if (list.Exists(x => x.Product.ProductID == ProductID))
                    {
                        foreach (var item in list)
                        {
                            if (item.Product.ProductID == ProductID)
                            {
                                item.Quanlity += Quanlity;
                            }
                        }
                    }
                    else
                    {
                        var item = new CartModel();
                        item.Product = product;
                        item.Quanlity = Quanlity;
                        list.Add(item);
                    }
                    Session["CartItem"] = list;
                }
                else
                {
                    var item = new CartModel();
                    item.Product = product;
                    item.Quanlity = Quanlity;
                    var list = new List<CartModel>();

                    list.Add(item);

                    Session["CartItem"] = list;
                }
            }
            else
            {
                TempData["msg1"] = "<script>alert('San pham het hang');</script>";
            }


            return null;

        }

        public JsonResult UpdateCart(string CartModels)
        {
            var JsonCart = new JavaScriptSerializer().Deserialize<List<CartModel>>(CartModels);
            var sessionCart = (List<CartModel>)Session["CartItem"];

            foreach (var item in sessionCart)
            {
                var itemjson = JsonCart.SingleOrDefault(x => x.Product.ProductID == item.Product.ProductID);
                var product = db.Products.Where(x => x.ProductID == item.Product.ProductID).SingleOrDefault();
                if (itemjson != null)
                {
                    if (itemjson.Quanlity >= 1 && itemjson.Quanlity <= product.Quanlity)
                    {
                        item.Quanlity = itemjson.Quanlity;

                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('Số lượng sản phẩm không đủ');</script>";
                    }


                }
            }
            Session["CartItem"] = sessionCart;

            return Json(new
            {
                status = true
            });
        }


        public JsonResult DeleteOneCart(int id)
        {
            var session_cart = Session["CartItem"];
            var list = (List<CartModel>)session_cart;

            list.RemoveAll(x => x.Product.ProductID == id);

            return Json(new
            {
                status = true
            });
        }


        public JsonResult DeleteCartAll()
        {
            Session["CartItem"] = null;

            return Json(new
            {
                status = true
            });
        }
    }
}