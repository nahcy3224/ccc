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

namespace CHOY.Controllers
{
    public class ProjectController : Controller
    {
        private ChoyContext db = new ChoyContext();

        // GET: Project
        public ActionResult Index()
        {  
            var session = ChoySession.Current;
            var MemberID = session.LoginId;
 ViewBag.Who = session.PerCode;
            //var MemberID = "M0002";
            ViewBag.MemberID = MemberID;
            var projects = db.Projects.Include(p => p.Member).Where(m=>m.MemberID==MemberID && m.DeleteAt==null);
            return View(projects.ToList());
        }

        // GET: Project/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Project/Create
        public ActionResult Create()
        {
            ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Email");
            return View();
        }

        // POST: Project/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
         //   if (ModelState.IsValid)
           // {
                var session =  ChoySession.Current;
                var MemberID = session.LoginId;
                project.ProjectID = "P0000";
                project.CreateAt = DateTime.Now;
                project.MemberID = MemberID;
                db.Projects.Add(project);
                db.SaveChanges();
                var lastProjects = db.Projects.OrderByDescending(p=>p.ProjectID).FirstOrDefault();
                db.Boards.Add(new Board{
                    ProjectID = lastProjects.ProjectID,
                    MemberIDOwner = lastProjects.MemberID,
                });
                db.SaveChanges();
            //}
                return RedirectToAction("Index");
            

            //ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Email", project.MemberID);
            //return View(project);
        }

        // GET: Project/Edit/5
        public ActionResult Edit(string pid)
        {
            if (pid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Where(m=>m.ProjectID==pid).FirstOrDefault();
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Email", project.MemberID);
            return View(project);
        }

        // POST: Project/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Email", project.MemberID);
            return View(project);
        }



        // POST: Groups/Delete/
        public ActionResult Delete(Project project, string pid)
        {
            var pro = db.Projects.Where(m => m.ProjectID == pid).FirstOrDefault();
            pro.DeleteAt = DateTime.Now;
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
