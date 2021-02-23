using CHOY.App_Code.Common;
using CHOY.DAL;
using CHOY.Models;
using CHOY.Models.ModelBinders;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CHOY.Controllers
{
  [RoutePrefix("api")]
  public class ApiVoteController : ApiController
  {
    ChoyContext db = new ChoyContext();

    [HttpGet]
    [Route("project/{ProjectID}/votes")]
    public HttpResponseMessage GetAllVotasOfProject(string ProjectID)
    {
      var response = new JsonResponse();

      var data = (from Vote in db.Votes.Where(v => v.ProjectID == ProjectID)
                  select new
                  {
                    VoteID = Vote.VoteID,
                    VoteName = Vote.VoteName,
                    Result = Vote.Result,
                    Choices = (from VoteRecord in db.VoteRecords.Where(vr => vr.VoteID == Vote.VoteID)
                               select new
                               {
                                 ChoiceID = VoteRecord.ChoiceID,
                                 Choice = VoteRecord.Choice,
                                 VoteCounts = VoteRecord.VoteCounts
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
        Message = "資源已成功取得!!",
        Data = data
      }, HttpStatusCode.OK); // Http Status Code: 200

      return response.Get();
    }

    [HttpPost]
    [Route("project/{ProjectID}/vote")]
    public HttpResponseMessage Create(string ProjectID, ApiVoteCreate request)
    {
      var response = new JsonResponse();

      if (request.VoteName == null || request.Choices.Length < 2 || request.VoteCount == null)
      {
        response.Set(new
        {
          Success = false,
          Message = "缺少執行所需參數 !!"
        }, HttpStatusCode.BadRequest); // Http Status Code: 400

        return response.Get();
      }

      var data = db.Projects.Where(p => p.ProjectID == ProjectID).FirstOrDefault();

      if (data == null)
      {
         response.Set(new
        {
          Success = false,
          Message = "Resource not found !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 404

        return response.Get();
      }

      var vote = new Vote
      {
        VoteName = request.VoteName,
        ProjectID = data.ProjectID,
        MemberIDOwner = data.MemberID,
        VoteCount = request.VoteCount
      };

      db.Votes.Add(vote);

      for (int i = 0; i < request.Choices.Length; i++)
      {
        VoteRecords voteRecords = new VoteRecords
        {
          Choice = request.Choices[i]
        };
        db.VoteRecords.Add(voteRecords);
      }

      try
      {
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
        Message = "Create completed !!",
        Data = vote.VoteID
      }, HttpStatusCode.OK); // Http Status Code: 200

      return response.Get();
    }
    [HttpPatch]
    [Route("vote/{VoteID}/choice/{ChoiceID}")]
    public HttpResponseMessage Update(int VoteID, int ChoiceID)
    {
      var response = new JsonResponse();
      var data = db.VoteRecords.Where(vr => vr.VoteID == VoteID && vr.ChoiceID == ChoiceID).FirstOrDefault();
      var result = db.Votes.Where(v => v.VoteID == VoteID).FirstOrDefault().Result;
      if (data == null)
      { // not found
        response.Set(new
        {
          Success = false,
          Message = "Resource not found !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 404

        return response.Get();
      }

      if (result != null)
      {
        response.Set(new
        {
          Success = true,
          Message = "此投票已結束"
        }, HttpStatusCode.Accepted); // Http Status Code: 202

        return response.Get();
      }

      if (ModelState.IsValid)
      { // 更新成功
        data.VoteCounts = data.VoteCounts + 1;
        db.Entry(data).State = EntityState.Modified;
        db.SaveChanges();

        response.Set(new
        {
          Success = true,
          Message = "Update completed !!"
        }, HttpStatusCode.OK); // Http Status Code: 200

        return response.Get();
      }
      // 更新失敗
      response.Set(new
      {
        Success = false,
        Message = "Update failed !!"
      }, HttpStatusCode.NotFound); // Http Status Code: 200

      return response.Get();
    }
    [HttpPatch]
    [Route("vote/{VoteID}/choice")]
    public HttpResponseMessage Update(int VoteID)
    {
      var response = new JsonResponse();
      var data = db.Votes.Where(v => v.VoteID == VoteID).FirstOrDefault();
      var result = db.Votes.Where(v => v.VoteID == VoteID).FirstOrDefault().Result;
      if (data == null)
      { // not found
        response.Set(new
        {
          Success = false,
          Message = "Resource not found !!"
        }, HttpStatusCode.NotFound); // Http Status Code: 404

        return response.Get();
      }

      if (result != null)
      {
        response.Set(new
        {
          Success = true,
          Message = "此投票已結束"
        }, HttpStatusCode.Accepted); // Http Status Code: 202

        return response.Get();
      }

      if (ModelState.IsValid)
      { // 更新成功
        data.VoteCount = data.VoteCount - 1;
        db.Entry(data).State = EntityState.Modified;
        db.SaveChanges();

        response.Set(new
        {
          Success = true,
          Message = "Update completed !!"
        }, HttpStatusCode.OK); // Http Status Code: 200

        return response.Get();
      }
      // 更新失敗
      response.Set(new
      {
        Success = false,
        Message = "Update failed !!"
      }, HttpStatusCode.NotFound); // Http Status Code: 200

      return response.Get();
    }
  }
}
