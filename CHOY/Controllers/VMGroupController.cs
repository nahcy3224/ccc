using CHOY.App_Code.Auth;
using CHOY.DAL;
using CHOY.Models;
using CHOY.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHOY.Controllers
{
  public class VMGroupController : Controller
  {
    private ChoyContext db = new ChoyContext();

    VMGroup vmgroup = new VMGroup();
    // GET: VMGroup
    //public ActionResult Create()
    //{
    //    VMGroup vmgroup = new VMGroup();

    //    return View();
    //}

    public ActionResult Create()
    {
      return View();
    }
    [HttpPost]
    public ActionResult Create(GroupMember groupMember, Group group, string Id, string GroupName)
    {
      //group.GroupID = "G0000";
      group.GroupName = GroupName;
      var session = ChoySession.Current;
      var MemberID = session.LoginId;
      group.MemberID = MemberID;
            //groupMember.MemberIDOwner="M0001";
            //group.MemberID = groupMember.MemberIDOwner;
 
      groupMember.MemberIDOwner = MemberID;
      groupMember.MemberIDInGroup = Id;

      db.Groups.Add(group);


    db.SaveChanges();

    var GroupID =db.Groups.OrderByDescending(m=>m.GroupID).FirstOrDefault().GroupID;
groupMember.GroupID =GroupID;

            //db.SaveChanges();
            db.GroupMembers.Add(groupMember);
            //try
            //{
            //  db.SaveChanges();
            //}
            //catch (DbEntityValidationException ex)
            //{
            //  var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
            //  var getFullMessage = string.Join("; ", entityError);
            //  var exceptionMessage = string.Concat(ex.Message, "errors are: ", getFullMessage);
            // }
            db.SaveChanges();

            return RedirectToAction("Index", "Groups");

      // return RedirectToAction("Index", "Groups",new { Id = groupMember.MemberIDInGroup });
    }

    public ActionResult Edit()
    {
      return View();
    }
    [HttpPost]
    public ActionResult Edit(GroupMember groupMember, Group group, string GroupName, string mid, string gid)
    {
      group.GroupID = "G0000";
      group.GroupName = GroupName;
      group.MemberID = "M0001";
      //groupMember.MemberIDOwner="M0001";
      //group.MemberID = groupMember.MemberIDOwner;
      groupMember.MemberIDOwner = group.MemberID;
      groupMember.MemberIDInGroup = mid;

      db.Groups.Add(group);
      db.SaveChanges();
      db.GroupMembers.Add(groupMember);
      db.SaveChanges();

      return RedirectToAction("Index", "Groups");

    }

  }
}