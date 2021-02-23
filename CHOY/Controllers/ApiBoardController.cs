using CHOY.App_Code.Auth;
using CHOY.App_Code.Common;
using CHOY.DAL;
using CHOY.Models;
using CHOY.Models.ModelBinders;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CHOY.Controllers
{
  [RoutePrefix("api")]
  public class ApiBoardController : ApiController
  {
    ChoyContext db = new ChoyContext();

    [HttpGet]
    [Route("project/{ProjectID}/boards")]
    public HttpResponseMessage GetAllBoardsOfProject(string ProjectID) // 抓指定專案的所有白板資料
    {
      var response = new JsonResponse();

      var data = (from Board in db.Boards.Where(b => b.ProjectID == ProjectID && b.DeleteAt == null)
                  select new
                  {
                    ProjectID = Board.ProjectID,
                    BoardID = Board.BoardID,
                    Code = Board.Code
                  }).ToList();

      if (data.Count == 0)
      {
        response.Set(new
        {
          Success = false,
          Message = "Resource not found !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 404

        return response.Get();
      }

      response.Set(new
      {
        Success = true,
        Message = "資源已成功取得!!",
        Data = data
      }, HttpStatusCode.OK); // Http Status Code: 200

      return response.Get();
    }

    [HttpGet]
    [Route("project/{ProjectID}/board/{BoardID}")]
    public HttpResponseMessage GetBoardOfProject(string ProjectID, string BoardID) // 抓指定白板資料
    {
      var response = new JsonResponse();
      var data = (from Board in db.Boards.Where(b => b.ProjectID == ProjectID && b.BoardID == BoardID && b.DeleteAt == null)
                  select new
                  {
                    ProjectID = Board.ProjectID,
                    BoardID = Board.BoardID,
                    Code = Board.Code
                  }).FirstOrDefault();

      if (data == null)
      {
        response.Set(new
        {
          Success = false,
          Message = "Resource not found !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 404

        return response.Get();
      }

      response.Set(new
      {
        Success = true,
        Message = "資源已取得!!",
        Data = data
      }, HttpStatusCode.OK); // Http Status Code: 200

      return response.Get();
    }

    [HttpGet]
    [Route("member/boards/own")]
    public HttpResponseMessage GetAllBoardsOfOwnProjects()
    {
      var response = new JsonResponse();
      var session = ChoySession.Current;
      var MemberID = session.LoginId;
      var data = (from Project in db.Projects.Where(p => p.MemberID == MemberID && p.DeleteAt == null)
                  select new
                  {
                    ProjectID = Project.ProjectID,
                    ProjectName = Project.ProjectName,
                    Data = (
                      from Board in db.Boards.Where(b => b.ProjectID == Project.ProjectID && b.DeleteAt == null)
                      select new
                      {
                        ProjectID = Board.ProjectID,
                        BoardID = Board.BoardID,
                        Code = Board.Code
                      }).ToList()
                  }).ToList();

      if (data.Count == 0)
      {
        response.Set(new
        {
          Success = false,
          Message = "Resource not found !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 404

        return response.Get();
      }

      response.Set(new
      {
        Success = true,
        Message = "資源已取得!!",
        Data = data
      }, HttpStatusCode.OK); // Http Status Code: 200

      return response.Get();
    }
    [HttpGet]
    [Route("member/boards/shared")]
    public HttpResponseMessage GetAllBoardsOfSharedProjects()
    {
      var response = new JsonResponse();
      var session = ChoySession.Current;
      var MemberID = session.LoginId;

      var data = (
        from jp in db.MemberJoinProjects
        from p in db.Projects
        where
          jp.MemberIDJoin == MemberID &&
          jp.ProjectID == p.ProjectID &&
          p.DeleteAt == null &&
          jp.MemberIDOwner != MemberID
        select new
        {
          ProjectID = p.ProjectID,
          ProjectName = p.ProjectName,
          Data = (
            from Board in db.Boards.Where(b => b.ProjectID == p.ProjectID)
            select new
            {
              ProjectID = Board.ProjectID,
              BoardID = Board.BoardID,
              Code = Board.Code
            }).ToList()
        }).ToList();

      if (data.Count == 0)
      {
        response.Set(new
        {
          Success = false,
          Message = "Resource not found !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 404

        return response.Get();
      }

      response.Set(new
      {
        Success = true,
        Message = "資源已取得!!",
        Data = data
      }, HttpStatusCode.OK); // Http Status Code: 200

      return response.Get();
    }
    [HttpPost]
    [Route("project/{ProjectID}/board")]
    public HttpResponseMessage Create(string ProjectID)
    {
      var response = new JsonResponse();
      var session = ChoySession.Current;
      var MemberID = session.LoginId;
      var data = db.Projects.Where(p => p.ProjectID == ProjectID && p.DeleteAt == null).FirstOrDefault();
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
        db.Boards.Add(new Board
        {
          ProjectID = data.ProjectID,
          MemberIDOwner = data.MemberID
        });
        db.SaveChanges();
      }
      catch
      {
        response.Set(new
        {
          Success = false,
          Message = "Create failed !!"
        }, HttpStatusCode.InternalServerError); // Http Status Code: 500

        return response.Get();
      }

      response.Set(new
      {
        Success = true,
        Message = "Create completed !!"
      }, HttpStatusCode.OK); // Http Status Code: 200


      return response.Get();
    }
    [HttpPatch]
    [Route("project/{ProjectID}/board/{BoardID}")]
    // public HttpResponseMessage Patch(ApiBoardPatch request)
    public HttpResponseMessage Update(string ProjectID, string BoardID, ApiBoardUpdate request)
    {
      var response = new JsonResponse();

      if (request.Canvas == null)
      {
        response.Set(new
        {
          Success = false,
          Message = "缺少執行所需參數 !!"
        }, HttpStatusCode.BadRequest); // Http Status Code: 400 

        return response.Get();
      }

      var data = db.Boards
        .Where(b => b.ProjectID == ProjectID && b.BoardID == BoardID)
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

      if (ModelState.IsValid)
      {
        data.Code = request.Canvas;
        db.Entry(data).State = EntityState.Modified;
        db.SaveChanges();

        response.Set(new
        {
          Success = true,
          Message = "Update completed !!"
        }, HttpStatusCode.OK); // Http Status Code: 200

        return response.Get();
      }

      response.Set(new
      {
        Success = false,
        Message = "Update failed !!"
      }, HttpStatusCode.NotFound); // Http Status Code: 200

      return response.Get();
    }
    [HttpDelete]
    [Route("project/{ProjectID}/board/{BoardID}")]
    // public HttpResponseMessage Patch(ApiBoardPatch request)
    public HttpResponseMessage Delete(string ProjectID, string BoardID)
    {
      var response = new JsonResponse();
      var data = db.Boards
        .Where(b => b.ProjectID == ProjectID && b.BoardID == BoardID && b.DeleteAt == null)
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
        data.DeleteAt = DateTime.Now;
        db.Entry(data).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch
      {
        response.Set(new
        {
          Success = false,
          Message = "Delete failed !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 200

        return response.Get();
      }

      response.Set(new
      {
        Success = true,
        Message = "Delete completed !!"
      }, HttpStatusCode.OK); // Http Status Code: 200

      return response.Get();
    }
  }
}
