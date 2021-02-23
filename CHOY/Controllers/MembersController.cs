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
using System.Data.Entity;
using CHOY.App_Code.Auth;
using CHOY.App_Code.Common;

namespace CHOY.Controllers
{
    public class MembersController : Controller
    {
        ChoyContext context = new ChoyContext();

        [checkPerCodeManager(flag = true)]
        public ActionResult Index(int page=1)
        {

            var session = ChoySession.Current;
            var MemberID = session.LoginId;
            ViewBag.Who = session.PerCode;

            Member member = new Member();
           // Member mem = context.Members.Where(m => m.MemberID == MemberID).FirstOrDefault();
           


            ViewBag.Date = DateTime.Now;            
            
            var data = context.Members.Where(m => m.MemberID != MemberID).ToList();
           
            int pagesize = 10;
            int pagecurrent = page < 1 ? 1 : page;    
            var pagedlist = data.ToPagedList(pagecurrent, pagesize);
            
            return View(pagedlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string id, Permissions[] PerCode)
        {

            Member mem = context.Members.Where(m => m.MemberID == id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                mem.PerCode = (Permissions)(PerCode.Sum(i => (int)i));

                context.Entry(mem).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mem);
        }

        [checkPerCodeSuspension(flag = true)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Suspend(string id)
        {
            Member mem = context.Members.Where(m => m.MemberID == id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                mem.IsSuspended = !mem.IsSuspended;

                context.Entry(mem).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mem);
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

            return File(member.ProfilePic, member.ImageMimeType ?? "Image/png");
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
            var session = ChoySession.Current;
            ViewBag.Who = session.PerCode;

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
            //var session = ChoySession.Current;
            // var MemberID = session.LoginId;
            var session = ChoySession.Current;
            ViewBag.Who = session.PerCode;
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
            var session = ChoySession.Current;
            var MemberID = session.LoginId;
            var mem = context.Members.Where(m => m.MemberID == MemberID).FirstOrDefault();
            if ((mem.PerCode & Permissions.Download) != Permissions.Download)
            {
                return RedirectToAction("Index");
            }
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


        //public ActionResult EditMember(string id)
        //{

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Member member = context.Members.Find(id);
        //    if (member == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(member);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditMember(string id, Member member, Permissions[] PerCode)
        //{
        //    var mem = context.Members.Where(m => m.MemberID == id).FirstOrDefault();

        //    if (ModelState.IsValid)
        //    {
        //        member.PerCode = (Permissions)PerCode.Sum(i => (int)i);

        //        member.ProfilePic = mem.ProfilePic;
        //        member.Psw = mem.Psw;
        //        member.Bday = mem.Bday;
        //        member.CreateAt = mem.CreateAt;
        //        member.ImageMimeType = mem.ImageMimeType;
        //        mem.IsSuspended = member.IsSuspended;
        //        mem.PerCode = member.PerCode;
        //        context.SaveChanges();
        //        return RedirectToAction("Edit");
        //    }
        //    return View(member);
        //}

        public ActionResult EditMember(string MemberID)
        {
            var session = ChoySession.Current;
            MemberID = session.LoginId;
            ViewBag.Who = session.PerCode;
            ViewBag.Password = TempData["Password"];
            if (MemberID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = context.Members.Find(MemberID);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMember(string MemberID, Member member, Permissions[] PerCode)
        {
            var session = ChoySession.Current;
            ViewBag.Who = session.PerCode;
            var mem = context.Members.Where(m => m.MemberID == MemberID).FirstOrDefault();

            mem.NickName = member.NickName;
            mem.ContactEmail = member.ContactEmail;
            context.SaveChanges();

            return View(mem);
        }

        public ActionResult _ChangePassword(string MemberID)
        {
            var session = ChoySession.Current;
            MemberID = session.LoginId;
            if (MemberID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = context.Members.Find(MemberID);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ChangePassword(string opsw, string psw2)
        {
            var session = ChoySession.Current;
            var MemberID = session.LoginId;
            var member = context.Members.Where(m => m.MemberID == MemberID).FirstOrDefault();
            var currentpsw = opsw;
            var newpsw = psw2;
            long salt = TimeConverter.ToTimestamp(member.CreateAt);
            var now = DateTime.Now;

            if (ChoyPassword.Validate(currentpsw, salt, member.Psw))
            {
                member.Psw = ChoyPassword.Hash(newpsw, salt);
                context.Entry(member).State = EntityState.Modified;
                context.SaveChanges();
            }
            else
                ViewBag.password = "Current Password is not matched";

            return RedirectToAction("EditMember");
        }

        //public ActionResult editHeadshot(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    Member member = context.Members.Find(id);

        //    if (member == null)
        //        return HttpNotFound();

        //    return View(member);
        //}
        public ActionResult editHeadshot(string MemberID)
        {
            var session = ChoySession.Current;
            MemberID = session.LoginId;
            ViewBag.Who = session.PerCode;
            if (MemberID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = context.Members.Find(MemberID);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editHeadshot(string MemberID, string images)
        {
            var session = ChoySession.Current;
            ViewBag.Who = session.PerCode;
            if (images == null)
                return View();


            if (ModelState.IsValid)
            {
                MemberID = session.LoginId;

                var mem = context.Members.Where(m => m.MemberID == MemberID).FirstOrDefault();

                var t = images.Substring(22);  // remove data:image/png;base64,

                byte[] bytes = Convert.FromBase64String(t);

                System.Drawing.Image image;
                mem.ProfilePic = bytes;
                //member.ProfilePic = mem.ProfilePic;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = System.Drawing.Image.FromStream(ms);
                    ms.Read(mem.ProfilePic, 0, bytes.Length);
                }
                mem.ImageMimeType = "image/png";

                //member.ProfilePic = mem.ProfilePic;
                //member.ImageMimeType = mem.ImageMimeType;
                
                context.SaveChanges();
            }

            return RedirectToAction("EditMember");
        }



        public ActionResult ChangePassword(string MemberID)
        {
            var session = ChoySession.Current;
            MemberID = session.LoginId;
            if (MemberID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = context.Members.Find(MemberID);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string opsw, string psw2)
        {
            var session = ChoySession.Current;
            var MemberID = session.LoginId;
            var member = context.Members.Where(m => m.MemberID == MemberID).FirstOrDefault();
            var currentpsw = opsw;
            var newpsw = psw2;
            long salt = TimeConverter.ToTimestamp(member.CreateAt);

            if (ChoyPassword.Validate(currentpsw, salt, member.Psw) && ModelState.IsValid)
            {
                member.Psw = ChoyPassword.Hash(newpsw, salt);
                context.Entry(member).State = EntityState.Modified;
                context.SaveChanges();
                TempData["Password"] = "Password changed successfully.";
            }
            else
                TempData["Password"] = "Current Password is not matched";
            //ViewBag.password = "Current Password is not matched";

            return RedirectToAction("EditMember");
        }
    }
}


