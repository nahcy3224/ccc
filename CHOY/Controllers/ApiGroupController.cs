using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CHOY.Models;
using CHOY.DAL;
using System.Data.Entity;
using CHOY.App_Code.Common;
using CHOY.App_Code.Auth;

namespace CHOY.Controllers
{
  [RoutePrefix("api")]
  public class ApiGroupController : ApiController
  {
    ChoyContext db = new ChoyContext();

    [HttpPatch]
    [Route("group/{GroupID}/name/{name}")]
    public HttpResponseMessage Update(string GroupID, string name)
    {
      var response = new JsonResponse();

      var session = ChoySession.Current;
      var MemberID = session.LoginId;

      var data = db.Groups
        .Where(g => g.GroupID == GroupID && g.MemberID == MemberID)
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
        data.GroupName = name;
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
        Message = "Update completed !!"
      }, HttpStatusCode.OK); // Http Status Code: 200

      return response.Get();



    }
  }
}
