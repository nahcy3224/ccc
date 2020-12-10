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

namespace CHOY.Controllers
{
    public class GroupMembersController : Controller
    {
        private ChoyContext db = new ChoyContext();

        // GET: GroupMembers
        public ActionResult Index()
        {
            var groupMembers = db.GroupMembers.Include(g => g.Group);
            return View(groupMembers.ToList());
        }

        // GET: GroupMembers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMember groupMember = db.GroupMembers.Find(id);
            if (groupMember == null)
            {
                return HttpNotFound();
            }
            return View(groupMember);
        }

        // GET: GroupMembers/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName");
            return View();
        }

        // POST: GroupMembers/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,MemberIDOwner,MemberIDInGroup")] GroupMember groupMember)
        {
            if (ModelState.IsValid)
            {
                db.GroupMembers.Add(groupMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", groupMember.GroupID);
            return View(groupMember);
        }

        // GET: GroupMembers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMember groupMember = db.GroupMembers.Find(id);
            if (groupMember == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", groupMember.GroupID);
            return View(groupMember);
        }

        // POST: GroupMembers/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,MemberIDOwner,MemberIDInGroup")] GroupMember groupMember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", groupMember.GroupID);
            return View(groupMember);
        }

        // GET: GroupMembers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMember groupMember = db.GroupMembers.Find(id);
            if (groupMember == null)
            {
                return HttpNotFound();
            }
            return View(groupMember);
        }

        // POST: GroupMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            GroupMember groupMember = db.GroupMembers.Find(id);
            db.GroupMembers.Remove(groupMember);
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
    }
}
