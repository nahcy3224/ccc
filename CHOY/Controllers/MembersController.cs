using CHOY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CHOY.DAL;
using System.Net;
using PagedList;
using System.Data;
using ClosedXML.Excel;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace CHOY.Controllers
{
    public class MembersController : Controller
    {
        ChoyContext context = new ChoyContext();

        // GET: Members
        public ActionResult Index(int page=1)
        {
   
            Member member = new Member();
     
            ViewBag.Date = DateTime.Now;            
            
            var data = context.Members.ToList();
           
            int pagesize = 2;
            int pagecurrent = page < 1 ? 1 : page;    
            var pagedlist = data.ToPagedList(pagecurrent, pagesize);
            
            return View(pagedlist);
        }
            public ActionResult Display(string id)
        {
            ViewData["Date"] = DateTime.Now;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = context.Members.Find(id);

            if (member == null)
                return HttpNotFound();
    
            return View(member);
        }


        public FileContentResult GetImage(string id) 
        {
            Member member = context.Members.Find(id);

            return File(member.ProfilePic, member.ImageMimeType);
        }


        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = context.Members.Find(id);
            if (member == null)
                return HttpNotFound();

            return View(member);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Member member = context.Members.Find(id);
            context.Members.Remove(member);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            Member newmember = new Member();
            return View(newmember);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Member member)
        {
            //if (images == null)
            //    return View();
            if (ModelState.IsValid)
            {
                //member.ImageMimeType = images.ContentType;
                //member.ProfilePic = new byte[0];
                // member.ProfilePic = new byte[images.ContentLength];

                //images.InputStream.Read(member.ProfilePic, 0,images.ContentLength);
                //member.MemberID = "M0000";
                //member.CreateAt = DateTime.Now;
                //member.PerCode = 0;
                //member.IsSuspended = false;
                context.Members.Add(member);
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
  
        public ActionResult Edit(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = context.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
                  return View(member);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id,Member member, Permissions[] PerCode)
        {
            var mem = context.Members.Where(m => m.MemberID == id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                member.PerCode = (Permissions)PerCode.Sum(i => (int)i);

                member.ProfilePic = mem.ProfilePic;
                member.Psw = mem.Psw;
                member.Bday = mem.Bday;
                member.CreateAt = mem.CreateAt;
                member.ImageMimeType = mem.ImageMimeType;
                mem.IsSuspended = member.IsSuspended;
                mem.PerCode = member.PerCode;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
          return View(member);
        }

        public IList<MemberExcel> GetMemberList()
        {
            var employeeList = (from e in context.Members
                                select new MemberExcel
                                {
                                    MemberID = e.MemberID,
                                    Email = e.Email,
                                    Gender = e.Gender ? "男":"女",
                                    NickName=e.NickName,
                                    Birthday=e.Bday,
                                    ContactEmail =e.ContactEmail,
                                    CreateAt=e.CreateAt,
                                    PerCode=e.PerCode,
                                    IsSuspended=e.IsSuspended? "停權":"未停權",

                                }).ToList();


            return employeeList;
        }
        //GET: Employee
        //public ActionResult Index()
        //{
        //    return View(this.GetEmployeeList());
        //}
        public ActionResult ExportToExcel()
        {
            var gv = new GridView();
            gv.DataSource = this.GetMemberList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/vnd.ms-excel";
            //Response.ContentType = "application/pdf";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            
            return View("Index");
        }


        public ActionResult EditMember(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = context.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMember(string id, Member member, Permissions[] PerCode)
        {
            var mem = context.Members.Where(m => m.MemberID == id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                member.PerCode = (Permissions)PerCode.Sum(i => (int)i);

                member.ProfilePic = mem.ProfilePic;
                member.Psw = mem.Psw;
                member.Bday = mem.Bday;
                member.CreateAt = mem.CreateAt;
                member.ImageMimeType = mem.ImageMimeType;
                mem.IsSuspended = member.IsSuspended;
                mem.PerCode = member.PerCode;
                context.SaveChanges();
                return RedirectToAction("Edit");
            }
            return View(member);
        }







    }
}


