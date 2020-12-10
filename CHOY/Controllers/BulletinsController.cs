using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CHOY.DAL;
using CHOY.Models;
using PagedList;

namespace CHOY.Controllers
{
    public class BulletinsController : Controller
    {
        private ChoyContext db = new ChoyContext();

        public ActionResult Index(int page=1)
        {
            var data = db.Bulletins.OrderByDescending(p=>p.BulletinID).ToList();

            int pagesize = 5;
            int pagecurrent = page < 1 ? 1 : page;

            var pagedlist = data.ToPagedList(pagecurrent, pagesize);


            return View("Index", pagedlist);
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bulletin bulletin = db.Bulletins.Find(id);
            if (bulletin == null)
            {
                return HttpNotFound();
            }
            return View(bulletin);
        }
        //public ActionResult Create()
        //{
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Bulletin bulletin)
        {
            if (ModelState.IsValid)
            {
                bulletin.EditTime = DateTime.Now;
                db.Bulletins.Add(bulletin);
                db.SaveChanges();
                //return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }


        //public ActionResult Create()
        //{
        //    Bulletin newbulletin = new Bulletin();

        //    return View(newbulletin);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Bulletin bulletin)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //bulletin.EditTime = DateTime.Now;
        //        db.Bulletins.Add(bulletin);
        //        db.SaveChanges();
        //        //return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("Index");
        //    //return View(bulletin);
        //}

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bulletin bulletin = db.Bulletins.Find(id);
            if (bulletin == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;

            return View(bulletin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BulletinID,EditTime,PublishStart,PublishEnd,Content")] Bulletin bulletin)
        {
            if (ModelState.IsValid)
            {
                bulletin.EditTime = DateTime.Now;
                db.Entry(bulletin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bulletin);
        }

        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Bulletin bulletin = db.Bulletins.Find(id);
        //    if (bulletin == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bulletin);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Bulletin bulletin = db.Bulletins.Find(id);
        //    db.Bulletins.Remove(bulletin);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        public ActionResult Delete(string id)
        {
            var blt = db.Bulletins.Where(m => m.BulletinID == id).FirstOrDefault();

            db.Bulletins.Remove(blt);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult _addBul(Bulletin bulletin)
        {
            if (ModelState.IsValid)
            {
                //bulletin.EditTime = DateTime.Now;
                db.Bulletins.Add(bulletin);
                db.SaveChanges();
                //return RedirectToAction("Index");
            }

            return PartialView(bulletin);
        }

        public ActionResult _ShowBul(Bulletin bulletin)
        {
            var now = DateTime.Now;
            var data = db.Bulletins.Where(m=>m.PublishEnd>now && m.PublishStart<now).OrderByDescending(p => p.PublishStart).ToList();

            return PartialView(data);
        }
    }
}
