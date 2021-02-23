using CHOY.App_Code.Auth;
using CHOY.App_Code.Common;
using CHOY.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CHOY.Controllers
{

  [RoutePrefix("api")]
  public class ApiProjectController : ApiController
  {
    ChoyContext db = new ChoyContext();
    [HttpPatch]
    [Route("project/{ProjectID}/name/{name}")]
    public HttpResponseMessage ChangeName(string ProjectID, string name)
    {

      var response = new JsonResponse();
      var session = ChoySession.Current;
      var MemberID = session.LoginId;

      if (MemberID == null)
      {
        response.Set(new
        {
          Success = false,
          Message = "您無權限進行此操作 !!"
        }, HttpStatusCode.Unauthorized); // Http Status Code: 401

        return response.Get();
      }

      var data = db.Projects
        .Where(p => p.ProjectID == ProjectID && p.MemberID == MemberID && p.DeleteAt == null)
        .FirstOrDefault();
      if (data == null)
      {
        response.Set(new
        {
          Success = false,
          Message = "Resource not found !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 404

        return response.Get();
      }

      try
      {
        data.ProjectName = name;
        db.Entry(data).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (System.Exception)
      {

        response.Set(new
        {
          Success = false,
          Message = "Update failed !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 200

        return response.Get();
      }

      response.Set(new
      {
        Success = true,
        Message = "Update completed !!",
      }, HttpStatusCode.OK); // Http Status Code: 200

      return response.Get();

    }
    // [HttpPost]
    // [Route("project/{ProjectID}/join")]
    // public HttpResponseMessage Join(string ProjectID)
    // {
    //   var response = new JsonResponse();
    //   var session = ChoySession.Current;
    //   var MemberID = session.LoginId;
    //   if (MemberID == null)
    //   {
    //     response.Set(new
    //     {
    //       Success = false,
    //       Message = "您無權限進行此操作，請先登入 !!"
    //     }, HttpStatusCode.Unauthorized); // Http Status Code: 401

    //     return response.Get();
    //   }

    //   if (db.Projects.Find(ProjectID) == null) 
    //   {
    //     response.Set(new
    //     {
    //       Success = false,
    //       Message = "Resource not found !!"
    //     }, HttpStatusCode.NotFound); // Http Status Code: 404

    //     return response.Get();
    //   }


    //   return response.Get();
    // }




    // [HttpGet]
    // [Route("project/board")]
    // public HttpResponseMessage Get(string MemberID) // 抓指定專案的所有白板資料
    // {
    //   var response = new JsonResponse();
    //   if (MemberID == null)
    //   {
    //     response.Set(new
    //     {
    //       Success = false,
    //       Message = "參數 member Id 缺失!!"
    //     }, HttpStatusCode.BadRequest); // Http Status Code: 400

    //     return response.Get();
    //   }

    //   var data = (
    //     from project in db.Projects.Where(Project => Project.MemberID == MemberID)
    //     select new
    //     {
    //       ProjectID = project.ProjectID,
    //       ProjectName = project.ProjectName,
    //       data = (
    //         from board in db.Boards.Where(Board => Board.ProjectID == project.ProjectID)
    //         select new
    //         {
    //           BoardID = board.BoardID,
    //           Code = board.Code
    //         }
    //       ).ToList()
    //     }
    //   ).ToList();

    //   if (data.Count == 0)
    //   {
    //     response.Set(new
    //     {
    //       Success = false,
    //       Message = "找不到資源!!"
    //     }, HttpStatusCode.NotFound); // Http Status Code: 404

    //     return response.Get();
    //   }

    //   response.Set(new
    //   {
    //     Success = true,
    //     Message = "資源已取得!!",
    //     Data = data
    //   }, HttpStatusCode.OK); // Http Status Code: 200

    //   return response.Get();
    // }
  }
}
