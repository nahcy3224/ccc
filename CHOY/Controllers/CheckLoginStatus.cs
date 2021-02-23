using CHOY.App_Code.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHOY.Controllers
{
    public class checkLoginStatus : ActionFilterAttribute
    {
        public bool flag = true;

        void LoginStatus(string MemberID)
        {
            HttpContext context = HttpContext.Current;

            if (MemberID == null)
                context.Response.Redirect("/Home/Index");
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (flag)
            {
                var session = ChoySession.Current;
                var MemberID = session.LoginId;

                LoginStatus(MemberID);
            }
        }
    }
}