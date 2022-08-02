using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using WebBanDienThoai.Others;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Controllers
{
    public class checkoutsController : Controller
    {
        private WebMobileEntities db = new WebMobileEntities();
        // GET: checkouts

        public ActionResult Index(int? promoCode)
        {
            if (promoCode != null)

            {
                Voucher vc = db.Voucher.Where(x => x.IdVoucher == promoCode).SingleOrDefault();

                Session["promoCode"] = vc.Code;
                Session["Discount"] = vc.Price;


            }
            var session_cart = Session["CartItem"];
            var list = new List<CartModel>();
            if (session_cart != null)
            {
                list = (List<CartModel>)session_cart;
            }

            if (Session["customerID"] == null)
            {
                return RedirectToAction("LoginAccount", "Login");
            }
            else
            {


                int id = Convert.ToInt32(Session["customerID"].ToString());
                var customer = db.Customers.Where(x => x.customerID == id).ToList().Distinct();
                var payment = db.Payments.Where(x => x.Status == true).ToList().Distinct();
                ViewBag.customer = customer;
                ViewBag.payment = payment;
            }


            return View(list);
        }
        public ActionResult AddOrderDetails(int PaymentID, decimal TotalProduct)
        {

            var order = new Order();
            decimal dis = 0;
            if (Session["Discount"] != null)
            {
                dis = (decimal)Session["Discount"];
            }

            order.CreatedAt = DateTime.Now;
            int id = Convert.ToInt32(Session["customerID"].ToString());
            order.customerID = id;
            order.PaymentID = PaymentID;
            order.Status = 1;
            order.TotalMoney = TotalProduct - dis;
            order.ViewStatus = false;


            db.Orders.Add(order);

            var session_cart = (List<CartModel>)Session["CartItem"];
            foreach (var item in session_cart)
            {
                var orderdl = new OrderDetail();
                orderdl.ProductID = item.Product.ProductID;
                orderdl.orderID = order.orderID;
                orderdl.Price = (decimal)(item.Product.Price);
                orderdl.Quanlity = item.Quanlity;
                orderdl.TotalProduct = (decimal)(orderdl.Quanlity * orderdl.Price);
                db.OrderDetails.Add(orderdl);
                orderdl.Status = true;
            }

            db.SaveChanges();
            Session["Discount"] = null;
            Session["CartItem"] = null;
            return RedirectToAction("thankyou");
        }

        public ActionResult AddOrderDetails2()
        {

            var order = new Order();
            decimal dis = 0;
            if (Session["Discount"] != null)
            {
                dis = (decimal)Session["Discount"];
            }

            order.CreatedAt = DateTime.Now;
            int id = Convert.ToInt32(Session["customerID"].ToString());
            order.customerID = id;
            order.PaymentID = 1;
            order.Status = 3;
            order.TotalMoney = decimal.Parse(getAmount()) / 100;
            order.ViewStatus = false;


            db.Orders.Add(order);

            var session_cart = (List<CartModel>)Session["CartItem"];
            foreach (var item in session_cart)
            {
                var orderdl = new OrderDetail();
                orderdl.ProductID = item.Product.ProductID;
                orderdl.orderID = order.orderID;
                orderdl.Price = (decimal)(item.Product.Price);
                orderdl.Quanlity = item.Quanlity;
                orderdl.TotalProduct = (decimal)(orderdl.Quanlity * orderdl.Price);
                db.OrderDetails.Add(orderdl);
                orderdl.Status = true;
            }
            db.SaveChanges();
            //Session["Discount"] = null;
            //Session["CartItem"] = null;

            return RedirectToAction("Payment");
            // return RedirectToAction("thankyou");
        }

        public ActionResult thankyou()
        {
            return View();
        }
        public string getAmount()
        {
            decimal dis = 0;
            var session_cart = Session["CartItem"];
            if (Session["Discount"] != null)
            {
                dis = (decimal)Session["Discount"];
            }
            var list = new List<CartModel>();
            if (session_cart != null)
            {
                decimal sum = 0;
                list = (List<CartModel>)session_cart;
                foreach (var item in list)

                {
                    sum += (decimal)(item.Product.Price * item.Quanlity);
                }
                sum = sum - dis;
                string temp = ((Int64)sum).ToString();
                return temp + "00"; // tỷ hay sao ấy
            }
            else
            {
                return "0";
            }
            // thử đi 

        }
        public ActionResult Payment()
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.0.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.0.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", getAmount()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {

                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?


                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {

                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;

                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;

                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }

    }
}













/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using WebBanDienThoai.Others;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Controllers
{
    public class checkoutsController : Controller
    {
        private WebMobileEntities db = new WebMobileEntities();
        // GET: checkouts

        public ActionResult Index(int? promoCode)
        {
            if(promoCode != null)

            {
                Voucher vc = db.Voucher.Where(x=> x.IdVoucher == promoCode).SingleOrDefault();

                Session["promoCode"] = vc.Code;
                Session["Discount"] = vc.Price;


            }    
            var session_cart = Session["CartItem"];
            var list = new List<CartModel>();
            if (session_cart != null)
            {
                list = (List<CartModel>)session_cart;
            }

            if (Session["customerID"] == null)
            {
                return RedirectToAction("LoginAccount", "Login");
            }
            else
            {
           

                int id = Convert.ToInt32(Session["customerID"].ToString());
                var customer = db.Customers.Where(x => x.customerID == id).ToList().Distinct();
                var payment = db.Payments.Where(x => x.Status == true).ToList().Distinct();
                ViewBag.customer = customer;
                ViewBag.payment = payment;
            }

           
            return View(list);
        }
        public ActionResult AddOrderDetails( int PaymentID, decimal TotalProduct)
        {
           
            var order = new Order();
            decimal dis = 0;
            if (Session["Discount"] != null)
            {
                dis = (decimal)Session["Discount"];
            }

            order.CreatedAt = DateTime.Now;
            int id = Convert.ToInt32(Session["customerID"].ToString());
            order.customerID = id;
            order.PaymentID = PaymentID;
            order.Status = 1; 
            order.TotalMoney = TotalProduct - dis;
            order.ViewStatus = false;

   
            db.Orders.Add(order);
           
            var session_cart = (List<CartModel>)Session["CartItem"];
            foreach (var item in session_cart)
            {
                var orderdl = new OrderDetail();
                orderdl.ProductID = item.Product.ProductID;
                orderdl.orderID = order.orderID;
                orderdl.Price =(decimal)( item.Product.Price);
                orderdl.Quanlity = item.Quanlity;
                orderdl.TotalProduct = (decimal)(orderdl.Quanlity * orderdl.Price );
                db.OrderDetails.Add(orderdl);
                orderdl.Status = true;
            }

            db.SaveChanges();
            Session["Discount"] = null;
            Session["CartItem"] = null;
            return RedirectToAction("thankyou");
        }


        public ActionResult thankyou()
        {
            return View();
        }
        public string getAmount()
        {
            decimal dis = 0;
            var session_cart = Session["CartItem"];
            if(Session["Discount"]!=null)
            {
                dis = (decimal)Session["Discount"];
                    }    
            var list = new List<CartModel>();
            if (session_cart != null)
            {
                decimal sum = 0;
                list = (List<CartModel>)session_cart;
                foreach (var item in list)

                {
                    sum += (decimal)(item.Product.Price * item.Quanlity);
                }
                sum = sum - dis;
                string temp = ((Int64)sum).ToString();
                return temp+"00"; // tỷ hay sao ấy
            }
            else {
                return "0";
            }
            // thử đi 
            
        }
        public ActionResult Payment()
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();
         
            pay.AddRequestData("vnp_Version", "2.0.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.0.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", getAmount() ); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm(int PaymentID, decimal TotalProduct)
        {
            if (Request.QueryString.Count > 0)
            {

                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                // có cái mã lỗi xem thử, chả lq
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?


                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {                      
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                      
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }

    }
}*/