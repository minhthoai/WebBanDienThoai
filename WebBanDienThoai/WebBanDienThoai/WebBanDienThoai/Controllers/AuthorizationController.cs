using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Controllers
{
    public class AuthorizationController : AuthorizeAttribute
    {
        public int?[] CurrentRole { get; set; }
        public AuthorizationController(int?[] role) : base()
        {
            CurrentRole = role;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["TaiKhoan"] == null)
                return base.AuthorizeCore(httpContext);
            User user = (User)httpContext.Session["TaiKhoan"];
            if (CurrentRole.Contains(user.Role))
            {
                return true;
            }
            return base.AuthorizeCore(httpContext);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                                    new RouteValueDictionary
                                    {
                                        {"action","Index" },
                                        {"controller","Home" }
                                    });
        }
    }
}