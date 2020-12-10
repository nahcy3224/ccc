using CHOY.App_Code;
using CHOY.App_Code.Auth;
using CHOY.App_Code.Auth.Jwt;
using CHOY.App_Code.Common;
using CHOY.Models;
using CHOY.Models.ModelBinders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace CHOY.Controllers
{
  [RoutePrefix("api/auth")]
  public class ApiAuthController : ApiController
  {
    [HttpPost]
    [Route("login")]
    public HttpResponseMessage Login(User user)
    {
      HttpResponseMessage response = new HttpResponseMessage();
      MemberSystem memberSystem = new MemberSystem();
      string messages = "";

      bool isLoginSuccessful = memberSystem.Login(user.Account, user.Password);
      if (isLoginSuccessful)
      {
        ChoySession session = ChoySession.Current;
        messages = $"會員({session.LoginId})，登入成功 !!";
        response.StatusCode = HttpStatusCode.OK; // http status code 200
      }
      else
      {
        messages = "帳號或密碼錯誤!!";
        response.StatusCode = HttpStatusCode.NotFound; // http status code 404
      }

      var result = new
      {
        Success = isLoginSuccessful,
        Messages = messages
      };
      response.Content = new StringContent(JsonConvert.SerializeObject(result));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

      return response;
    }
    [HttpGet]
    [Route("logout")]
    public void Logout()
    {
      MemberSystem memberSystem = new MemberSystem();
      memberSystem.Logout();
    }
    [HttpPost]
    [Route("register")]
    public HttpResponseMessage Register(RegisterData registerData)
        {
      Env env = new Env();
      SimpleJws jws = new SimpleJws();
      HttpResponseMessage response = new HttpResponseMessage();
      string token = registerData.Token;
      bool isSuccess = true;
      string messages = "";
      if (token == null) // 缺乏 token
      {
        isSuccess = false;
        messages = "無權限執行該請求!!!";
        response.StatusCode = HttpStatusCode.Unauthorized; // 401
      }
      else if (!jws.Validate(token, env.SecretKey)) // 缺乏 token
      {
        isSuccess = false;
        messages = token;
        // messages = "註冊連結已失效!!!";
        response.StatusCode = HttpStatusCode.Unauthorized; // 401
      }
      else if (registerData.Password.Length < 6 || registerData.Password.Length > 18)
      {
        isSuccess = false;
        messages = "密碼長度必須界在 6 ~ 18 個字元!!!";
        response.StatusCode = HttpStatusCode.BadRequest; // 400 
      }
      else
      {
        MemberSystem memberSystem = new MemberSystem();
        Dictionary<string, object> jwt = jws.Decode(token);
        if (!jwt.ContainsKey("Email") || !jwt.ContainsKey("Gender") || !jwt.ContainsKey("Birthday"))
        {
          isSuccess = false;
          messages = "無效操作，缺乏必要資訊!!!";
          response.StatusCode = HttpStatusCode.BadRequest; // 400 
        }
        else
        {
          string name = jwt.ContainsKey("UserName")
            ? (string)jwt["UserName"]
            : null;
          DateTime now = DateTime.Now;
          Member member = new Member
          {
            Email = (string)jwt["Email"],
            Psw = ChoyPassword.Hash(registerData.Password, TimeConverter.ToTimestamp(now)),
            NickName = name,
            Gender = (bool)jwt["Gender"],
            Bday = TimeConverter.ToDateTime((long)jwt["Birthday"]),
            ContactEmail = (string)jwt["Email"],
            CreateAt = now,
            PerCode = 0,
            IsSuspended = false,
            LastLogInTime = now
          };
          if (!memberSystem.Register(member))
          {
            isSuccess = false;
            messages = "註冊失敗";
            response.StatusCode = HttpStatusCode.InternalServerError; // 500
          }
          else
          {
            isSuccess = true;
            messages = "註冊成功";
            response.StatusCode = HttpStatusCode.OK; // 200
          }
        }
      }

      var result = new
      {
        Success = isSuccess,
        Messages = messages
      };
      response.Content = new StringContent(JsonConvert.SerializeObject(result));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

      return response;
    }
    [HttpPost]
    [Route("verifyEmailAddress")]
    public HttpResponseMessage VerifyEmailAddress(RegisterUser user)
    {
      MemberSystem memberSystem = new MemberSystem();
      HttpResponseMessage response = new HttpResponseMessage();
      string link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, $"/Home/Index/#/check");
      /**
       * memberSystem.SendValidationEmailByAPI() 回復所代表意義
       * return 0 : 表示 Email 寄送成功
       * return 1 : 表示 Email 不能為空 || Email 格式錯誤
       * return 2 : 表示 Email 已被註冊
       * return 3 : 表示 UserName 不能超過 15 個字
       * return 4 : 表示 Birthday 不能為空 || Birthday 不能晚於今日
       */
      int statusCode = memberSystem.SendValidationEmailByAPI(user.Email, user.UserName, user.Gender, user.Birthday, link);
      bool isSuccess = true;
      string messages = "";
      switch (statusCode)
      {
        case 0:
          messages = "驗證信寄送成功";
          response.StatusCode = HttpStatusCode.OK; // 200
          break;
        case 1:
          isSuccess = false;
          messages = "驗證信寄送失敗，Email不能為空或是格式錯誤";
          response.StatusCode = HttpStatusCode.BadRequest; // 400
          break;
        case 2:
          isSuccess = false;
          messages = "驗證信寄送失敗，Email已被註冊";
          response.StatusCode = HttpStatusCode.BadRequest; // 400
          break;
        case 3:
          isSuccess = false;
          messages = "驗證信寄送失敗，Name不能超過15個字";
          response.StatusCode = HttpStatusCode.BadRequest; // 400
          break;
        case 4:
          isSuccess = false;
          messages = "驗證信寄送失敗，Birthday不能為空或是不能晚於今日";
          response.StatusCode = HttpStatusCode.BadRequest; // 400
          break;
        default:
          isSuccess = false;
          messages = "驗證信寄送失敗，未知錯誤";
          response.StatusCode = HttpStatusCode.InternalServerError; // 500
          break;
      }

      var result = new
      {
        Success = isSuccess,
        Messages = messages
      };

      response.Content = new StringContent(JsonConvert.SerializeObject(result));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
      return response;
    }
    [HttpGet]
    [Route("getSessionInfo")]
    public HttpResponseMessage GetSessionInfo() // 測試用功能
    {
      HttpResponseMessage response = new HttpResponseMessage();
      ChoySession session = ChoySession.Current;

      var result = new
      {
        LoginId = session.LoginId,
        LoginAt = session.LoginAt
      };

      response.StatusCode = HttpStatusCode.OK;
      response.Content = new StringContent(JsonConvert.SerializeObject(result));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
      return response;
    }
  }
}
