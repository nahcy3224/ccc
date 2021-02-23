using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CHOY.App_Code.Auth;
using CHOY.DAL;
using CHOY.Models;
using CHOY.ViewModels;

namespace CHOY.Controllers
{
    public class GroupsController : Controller
    {
        private ChoyContext db = new ChoyContext();

        //var me 記得加入Session
        // GET: Groups
        public ActionResult Index()
        {
            var session = ChoySession.Current;
            ViewBag.Who = session.PerCode;

            var groups = db.Groups.Include(g => g.Member).Where(m=>m.MemberID==session.LoginId);
            return View(groups.ToList());
        }

        // GET: Groups/Create
        // GET: Groups/Create
        public ActionResult Create()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string GroupName)
        {
            var session = ChoySession.Current;
            var MemberID = session.LoginId;

            var group = new Group();
            group.GroupName = GroupName;
            group.MemberID = MemberID;

            db.Groups.Add(group);
            db.SaveChanges();

            ViewBag.test = GroupName;
            
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Create(Group group, string GroupName)
        {
            var session = ChoySession.Current;
            var MemberID = session.LoginId;

            var groupname = GroupName;

            group.GroupID = "G0000";
            group.GroupName = groupname;
            group.MemberID = MemberID;

            db.Groups.Add(group);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(string mid,string gid)
        {
            if (mid  == null| gid==null)///////////////////////////////////////////////////////////////////////////
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Where(m => m.MemberID == mid && m.GroupID == gid).FirstOrDefault();
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Email", group.MemberID);
            return View(group);
        }

        // POST: Groups/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Email", group.MemberID);
            return View(group);
        }

        // POST: Groups/Delete/5
        //[HttpPost, ActionName("Delete")]
       // [ValidateAntiForgeryToken]
        public ActionResult Delete(GroupMember groupmember,string mid, string gid)
        {
            IEnumerable < GroupMember > groupmemberList= db.GroupMembers.Where(m => m.MemberIDOwner == mid && m.GroupID == gid);
            if (groupmember != null) 
            {
                foreach (var item in groupmemberList)
                { 
                      db.GroupMembers.Remove(item);
                }
            }
            Group group = db.Groups.Where(m => m.MemberID == mid && m.GroupID == gid).FirstOrDefault();
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [ChildActionOnly]
        public ActionResult _MemberInGroups(string gid, string mid)
        {            
            var members = db.GroupMembers.Include(g => g.Member).Where(m => m.GroupID == gid && m.MemberIDOwner == mid).ToList();
            return PartialView(members);
        }


        public ActionResult DeleteMember(string id, string gid)
        {
            GroupMember groupmember = db.GroupMembers.Where(m => m.GroupID == gid && m.MemberIDInGroup == id).FirstOrDefault();
            db.GroupMembers.Remove(groupmember);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //[ChildActionOnly]
        public ActionResult CreateMemberInGroups(string gid, string mid)
        {
            if (mid == null | gid == null)
            {
                Group group = db.Groups.Where(m => m.MemberID == mid && m.GroupID == gid).FirstOrDefault();
                return View(group);
            }
            GroupMember groupmember = db.GroupMembers.Where(m => m.MemberIDOwner == mid && m.GroupID == gid).FirstOrDefault();
            //Group group= db.Groups.Where( m => m.MemberID == mid && m.GroupID == gid).FirstOrDefault();

            //if (groupmember == null)
            //{
            //    return HttpNotFound();
            //}
            return View(groupmember);

        }

        // POST: Groups/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ChildActionOnly]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateMemberInGroups(GroupMember groupmember, string gid, string mid,  string Email)
        {

            groupmember.GroupID = gid;
            groupmember.MemberIDOwner = mid;
            if(Email == null || Email == "")
            {
                return RedirectToAction("Index");
            }
            var mem = db.Members.Where(x => x.Email == Email).FirstOrDefault();
            if (mem == null )
            {
                return RedirectToAction("Index");
            }
            groupmember.MemberIDInGroup = mem.MemberID;
            bool exists = db.GroupMembers.Any(m => m.MemberIDOwner == mid && m.GroupID == gid && m.MemberIDInGroup == mem.MemberID);
            if (exists == true)
            {
                return RedirectToAction("Index");
            }
            else
                db.GroupMembers.Add(groupmember);
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
