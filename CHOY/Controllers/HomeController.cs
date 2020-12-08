using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHOY.Controllers
{
  public class HomeController : Controller
  {
    // GET: Home
    public ActionResult Index()
    {
      if (TempData["Error"] != null)
        ViewBag.Error = TempData["Error"];

      return View();
    }
  }
}