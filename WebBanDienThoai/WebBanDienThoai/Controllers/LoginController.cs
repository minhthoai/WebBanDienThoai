using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private WebMobileEntities db = new WebMobileEntities();
        public ActionResult LoginAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAccount(string email, string password)
        {

            var loginKh = db.Customers.SingleOrDefault(x => x.Email == email && x.Password == password);
            //Md5Encode md5 = new Md5Encode();
            //var passmd5 = md5.EncodeMd5Encrypt(password);

            var login = db.Users.SingleOrDefault(x => x.Email == email && x.Password == password);




            if (login != null)
            {
                if (login.Status == true)
                {

                    Session["UserID"] = login.UserID;
                    Session["Username"] = login.Username;
                    Session["Email"] = login.Email;
                    Session["Password"] = login.Password;
                    Session["Image"] = login.Image;
                    Session["role"] = login.Role;
                    return Redirect("~/AdminHome/Index");
                }
                else
                {
                    ViewBag.error1 = "Tài khoản bị khóa";
                }






            }
            else if (loginKh != null)
            {
                if (loginKh.Status == true)
                {

                    Session["customerID"] = loginKh.customerID;
                    Session["customerName"] = loginKh.customerName;
                    Session["Email"] = loginKh.Email;
                    Session["Password"] = loginKh.Password;

                    return Redirect("~/Home/Index");
                }
                else
                {
                    ViewBag.error1 = "Tài khoản bị khóa";
                }
            }

            else
            {

                ViewBag.error = "Tài khoản hoặc mật khẩu sai";
            }


            return View();
        }
        public ActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterUser(Customer _user)
        {
            if (ModelState.IsValid)
            {
                var check_id = db.Customers.Where(s => s.Email == _user.Email).FirstOrDefault();
                if (check_id == null)
                {
                    if (_user.Password == _user.ComfirmPass)
                    {

                        _user.CreatedAt = DateTime.Now;
                        _user.Status = true;
                        db.Configuration.ValidateOnSaveEnabled = false;

                        db.Customers.Add(_user);
                        db.SaveChanges();
                        return RedirectToAction("LoginAccount");
                    }
                    else
                    {
                        ViewBag.ErrorRegister1 = "Sai mật khẩu ";
                    }
                }
                else
                {
                    ViewBag.ErrorRegister = "This Email is exist";
                    return View();

                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}