using CHOY.App_Code.Auth;
using CHOY.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHOY.Controllers
{
    public class checkPerCodeBulletin : ActionFilterAttribute
    {
        public bool flag = false;

        ChoyContext db = new ChoyContext();
        
        void PerCode(string MemberID)
        {
            HttpContext context = HttpContext.Current;

            var mem = db.Members.Where(m => m.MemberID == MemberID).FirstOrDefault();

            if ((mem.PerCode & Permissions.Bulletin) != Permissions.Bulletin)
                context.Response.Redirect("/Project/Index");
            //context.Response.Write("<script language='javascript'>alert('發表成功!');window.open('WebForm2.aspx') </ script > ");
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