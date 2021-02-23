using CHOY.App_Code.Auth;
using CHOY.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHOY.Controllers
{
    public class checkPerCodeManager : ActionFilterAttribute
    {
        public bool flag = false;

        ChoyContext db = new ChoyContext();

        void PerCode(string MemberID)
        {
            HttpContext context = HttpContext.Current;

            var mem = db.Members.Where(m => m.MemberID == MemberID).FirstOrDefault();

            if ((mem.PerCode & Permissions.Manager) != Permissions.Manager)
                context.Response.Redirect("/Project/Index");
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (flag)
            {
                var session = ChoySession.Current;
                var MemberID = session.LoginId;

                PerCode(MemberID);
            }
        }
    }
}