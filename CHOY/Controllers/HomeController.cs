using CHOY.App_Code;
using CHOY.App_Code.Auth;
using CHOY.App_Code.Auth.Jwt;
using CHOY.App_Code.Common;
using CHOY.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHOY.Controllers
{
    [checkLoginStatus(flag = false)]
    public class HomeController : Controller
  {
    ChoyContext db = new ChoyContext();
    // GET: Home
    public ActionResult Index()
    {
      if (TempData["Error"] != null)
        ViewBag.Error = TempData["Error"];

      return View();
    }
        public ActionResult Logout()
        {
            MemberSystem memberSystem = new MemberSystem();
            memberSystem.Logout();

            return RedirectToAction("Index");
        }
        public ActionResult SetRandenPassword(string Token = null)
    {
      if (string.IsNullOrWhiteSpace(Token))
      {
        return RedirectToAction("Index");
      }
      var env = new Env();
      var jws = new SimpleJws();
      if (jws.Validate(Token, env.SecretKey))
      {
        var payload = jws.Decode(Token);
        var MemberID = (string)payload["MemberID"];
        var data = db.Members.Where(m => m.MemberID == MemberID).FirstOrDefault();
        var password = ChoyPassword.Hash((string)payload["Password"], TimeConverter.ToTimestamp(data.CreateAt));
        if (data.Psw != password)
        {
          try
          {
            data.Psw = password;
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
          }
          catch
          {
            ViewBag.Error = "Sorry, the server is busy. Please try again later.";
            return View();
          }
        }
      } else {
        ViewBag.Error = "The apply has expired";
        return View();
      }
      
      var url = Url.Action("Index") + "#/login";
      return Redirect(url);
    }
  }
}