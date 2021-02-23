using CHOY.App_Code.Auth;
using CHOY.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHOY.Controllers
{   
    public class BoardController : Controller
    {
        private ChoyContext db = new ChoyContext();
        // GET: Board
        public ActionResult Index()
        {

            var session = ChoySession.Current;
            ViewBag.Who = session.PerCode;
               ViewBag.MemberID = session.LoginId;
            return View();
        }
    }
}