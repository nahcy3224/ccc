using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using CHOY.App_Code;
using CHOY.App_Code.Auth;
using CHOY.App_Code.Auth.Jwt;
using CHOY.App_Code.Common;
using CHOY.DAL;
using CHOY.Models;
using CHOY.Models.ModelBinders;
using Newtonsoft.Json;


namespace CHOY.Controllers
{
    public class MemberJoinProjectsController : Controller
    {
        private ChoyContext db = new ChoyContext();

        // GET: MemberJoinProjects
        //public ActionResult Index()
        //{
        //    var session = ChoySession.Current;
        //    var memberJoinProjects = db.MemberJoinProjects.Include(m => m.Member).Include(m => m.Project).Where(m=>m.MemberIDOwner==session.LoginId );
        //    return View(memberJoinProjects.ToList());
        //}

        [ChildActionOnly]
        public ActionResult _MemInProject(string ProjectID ,string MemberID )
        {
            var memberJoinProjects = db.MemberJoinProjects.Include(m => m.Member).Include(m => m.Project).Where(m => m.MemberIDOwner == MemberID && m.ProjectID == ProjectID); 
            return View(memberJoinProjects.ToList());
        }

        //// GET: MemberJoinProjects/Create
        //public ActionResult Create(string mid,string pid)
        //{
        //    //Project project = db.Projects.Where(m => m.ProjectID == pid && m.MemberID == mid).FirstOrDefault();
        //    //ViewBag.MemberIDJoin = new SelectList(db.Members, "MemberID", "Email");
        //    //ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName");
        //    return View();
        //}

        //// POST: MemberJoinProjects/Create
        //// 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        //// 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create( MemberJoinProject memberJoinProject, string mid, string pid,string email)
        //{
        //        memberJoinProject.ProjectID = pid;
        //        memberJoinProject.MemberIDOwner = mid;
        //        memberJoinProject.SharePerID =Share.Share;
        //        var gro = db.Members.First(x => x.Email == email);
        //        memberJoinProject.MemberIDJoin = gro.MemberID;
        //        bool exists = db.MemberJoinProjects.Any(m => m.MemberIDOwner == mid && m.ProjectID == pid && m.MemberIDJoin == gro.MemberID);
        //                            db.MemberJoinProjects.Add(memberJoinProject);

        //    if (exists == true)
        //        {
        //             return RedirectToAction("Create");
        //        }
        //        else
        //            db.SaveChanges();
        //            return RedirectToAction("Index", "Project");

        //    //ViewBag.MemberIDJoin = new SelectList(db.Members, "MemberID", "Email", memberJoinProject.MemberIDJoin);
        //    //ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName", memberJoinProject.ProjectID);
        //    //return View(memberJoinProject);
        //}

        //// GET: MemberJoinProjects/Create
        //public ActionResult CreateWGroup(Group group,GroupMember groupmmber,string mid, string pid)
        //{

        //    //ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", group.GroupName);
        //    //ViewBag.GroupMember= new SelectList(;
        //    //Project project = db.Projects.Where(m => m.ProjectID == pid && m.MemberID == mid).FirstOrDefault();
        //    //ViewBag.MemberIDJoin = new SelectList(db.Members, "MemberID", "Email");
        //    //ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName");
        //    return View();
        //}

        //// POST: MemberJoinProjects/Create
        //// 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        //// 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateWGroup(MemberJoinProject memberJoinProject, string mid, string pid, string groupname)
        //{

        //    //IEnumerable<Group> groupList= db.Groups.Where(x => x.GroupName == groupname);
        //    var gro = db.Groups.First(x => x.GroupName == groupname);
        //    IEnumerable<GroupMember> groupmemberList = db.GroupMembers.Where(m => m.MemberIDOwner == mid && m.GroupID == gro.GroupID);
        //    //ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName", memberJoinProject.ProjectID);
        //    //ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Email", group.MemberID);
        //    bool exists = db.MemberJoinProjects.Any(m => m.MemberIDOwner == mid && m.ProjectID == pid && m.MemberIDJoin == gro.MemberID);
        //    if (exists == true)
        //    {
        //        return RedirectToAction("Create");
        //    }
        //    else

        //    foreach (var item in groupmemberList)
        //    { 
        //              memberJoinProject.MemberIDJoin = item.MemberIDInGroup;
        //              memberJoinProject.ProjectID = pid;
        //               memberJoinProject.MemberIDOwner = mid;
        //              memberJoinProject.SharePerID = Share.Share;
        //              db.MemberJoinProjects.Add(memberJoinProject);

        //    }
        //    db.SaveChanges();
        //    return RedirectToAction("Index", "Project");

        //    //ViewBag.MemberIDJoin = new SelectList(db.Members, "MemberID", "Email", memberJoinProject.MemberIDJoin);
        //    //ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName", memberJoinProject.ProjectID);
        //    //return View(memberJoinProject);
        //}
        
    // GET: MemberJoinProjects/Create
    public ActionResult CreateW(string MemberID, string ProjectID)
        {
            var session = ChoySession.Current;
            ViewBag.Who = session.PerCode;

            ViewBag.MID = MemberID;
            ViewBag.PID = ProjectID;
            return View();
        }

        // POST: MemberJoinProjects/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateW(ApiAuthRegister data, MemberJoinProject memberJoinProject, string MemberID, string ProjectID, string emailorgname)
        {
            var project= db.Projects.First(x => x.ProjectID == ProjectID);
            ViewBag.ProjectName = project.ProjectName;

            if (Validator.IsValidEmail(emailorgname))
            {
                var gro = db.Members.Where(x => x.Email == emailorgname).FirstOrDefault();
                if (gro == null )
                {
                    //寄送email
                    //data=
                    ViewBag.Message = "此使用者未註冊會員";
                    return RedirectToAction("CreateW", new { ProjectID, MemberID });
                }
                else {
                    memberJoinProject.ProjectID = ProjectID;
                    memberJoinProject.MemberIDOwner = MemberID;
                    memberJoinProject.SharePerID = Share.Share;
                    memberJoinProject.MemberIDJoin = gro.MemberID;

                    bool exists = db.MemberJoinProjects.Any(m => m.MemberIDOwner == MemberID && m.ProjectID == ProjectID && m.MemberIDJoin == gro.MemberID);
                    db.MemberJoinProjects.Add(memberJoinProject);

                    if (exists == true)
                    {
                        return RedirectToAction("CreateW", new { ProjectID, MemberID });
                    }
                }
            }

            else
            {
                bool groupexist = db.Groups.Any(x => x.GroupName == emailorgname);
                if (groupexist == false)
                {
                    ViewBag.Message = "此群組不存在";
                }
                else
                {
                    var gro = db.Groups.First(x => x.GroupName == emailorgname);
                    IEnumerable<GroupMember> groupmemberList = db.GroupMembers.Where(m => m.MemberIDOwner == MemberID && m.GroupID == gro.GroupID);
                    //var groupmemberList = db.GroupMembers.Where(m => m.MemberIDOwner == MemberID && m.GroupID == gro.GroupID);
                    bool exists = db.MemberJoinProjects.Any(m => m.MemberIDOwner == MemberID && m.ProjectID == ProjectID && m.MemberIDJoin == gro.MemberID);
                    if (exists == true)
                    {
                        return RedirectToAction("CreateW", new { ProjectID, MemberID });
                    }
                    else
                    { 
                        foreach (var item in groupmemberList)
                        {
                            MemberJoinProject m = new MemberJoinProject()
                            { 
                            MemberIDJoin = item.MemberIDInGroup,
                            ProjectID = ProjectID,
                            MemberIDOwner = MemberID,
                           SharePerID = Share.Share
                           };
                         db.MemberJoinProjects.Add(m);
                        }
                         db.SaveChanges();
                    }
                    //db.SaveChanges();
                }
            }
            db.SaveChanges();

            var memberJoinProjects = db.MemberJoinProjects.Include(m => m.Member).Include(m => m.Project).Where(m => m.MemberIDOwner == MemberID && m.ProjectID == ProjectID);
            return RedirectToAction("CreateW", new {  ProjectID, MemberID });
        }

        //ViewBag.MemberIDJoin = new SelectList(db.Members, "MemberID", "Email", memberJoinProject.MemberIDJoin);
        //ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName", memberJoinProject.ProjectID);
        //return View(memberJoinProject);

        //// GET: MemberJoinProjects/Edit/5
        //public ActionResult Edit(string mid,string pid)
        //{
        //    //if (id == null)
        //    //{
        //    //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    //}
        //    //MemberJoinProject memberJoinProject = db.MemberJoinProjects.Where(m=>m.ProjectID==pid &&m.MemberIDOwner==mid).FirstOrDefault();
        //    var memberJoinProject= db.MemberJoinProjects.Where(m => m.ProjectID == pid && m.MemberIDOwner == mid).ToList();
        //    if (memberJoinProject == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    //ViewBag.MemberIDJoin = new SelectList(db.Members, "MemberID", "Email", memberJoinProject.MemberIDJoin);
        //    //ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName", memberJoinProject.ProjectID);
        //    return View(memberJoinProject);
        //}

        //// POST: MemberJoinProjects/Edit/5
        //// 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        //// 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ProjectID,MemberIDOwner,MemberIDJoin,SharePerID")] MemberJoinProject memberJoinProject)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(memberJoinProject).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MemberIDJoin = new SelectList(db.Members, "MemberID", "Email", memberJoinProject.MemberIDJoin);
        //    ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName", memberJoinProject.ProjectID);
        //    return View(memberJoinProject);
        //}

        // GET: MemberJoinProjects/Delete/5
        public ActionResult Delete(string id,string pid)
        {
            var memberJoinProjects = db.MemberJoinProjects.Where(m => m.ProjectID == pid && m.MemberIDJoin == id ).AsEnumerable();
            foreach (var item in memberJoinProjects) 
            { 
            db.MemberJoinProjects.Remove(item);
             }
            db.SaveChanges();

            return RedirectToAction("Index", "Project");
        }

        // POST: MemberJoinProjects/Delete/5
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteMember(MemberJoinProject memberJoinProject ,string id,string oid,string pid)
        {
            var mem = db.MemberJoinProjects.Where(m => m.ProjectID == pid && m.MemberIDJoin == id && m.MemberIDOwner==oid).FirstOrDefault();
            db.MemberJoinProjects.Remove(mem);
            db.SaveChanges();
            var member = db.MemberJoinProjects.Where(m => m.ProjectID == pid  && m.MemberIDOwner == oid).FirstOrDefault();

            return RedirectToAction("CreateW",new { ProjectID=pid,MemberID=oid});
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
