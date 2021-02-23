using CHOY.App_Code;
using CHOY.App_Code.Auth;
using CHOY.App_Code.Auth.Jwt;
using CHOY.App_Code.Common;
using CHOY.DAL;
using CHOY.Models;
using CHOY.Models.ModelBinders;
using Newtonsoft.Json;
using RestSharp;
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
    public HttpResponseMessage Login(ApiAuthLogin user)
    {
      HttpResponseMessage response = new HttpResponseMessage();
      MemberSystem memberSystem = new MemberSystem();
      string messages = "";

      bool isLoginSuccessful = memberSystem.Login(user.Account, user.Password);
      if (isLoginSuccessful)
      {
        ChoySession session = ChoySession.Current;
        messages = $"Member({session.LoginId})have successfully logged in !!";
        response.StatusCode = HttpStatusCode.OK; // http status code 200
      }
      else
      {
        messages = "Incorrect account or password";
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
    public HttpResponseMessage Register(ApiAuthRegister data)
    {
      Env env = new Env();
      SimpleJws jws = new SimpleJws();
      HttpResponseMessage response = new HttpResponseMessage();
      string token = data.Token;
      bool isSuccess = true;
      string messages = "";
      if (token == null) // 缺乏 token
      {
        isSuccess = false;
        messages = "You don't have permission to access this server.";
        response.StatusCode = HttpStatusCode.Unauthorized; // 401
      }
      else if (!jws.Validate(token, env.SecretKey)) // 缺乏 token
      {
        isSuccess = false;
        messages = "註冊連結已失效!!!";
        response.StatusCode = HttpStatusCode.Unauthorized; // 401
      }
      else if (data.Password.Length < 6 || data.Password.Length > 18)
      {
        isSuccess = false;
        messages = "Your password must be between 6 and 18 characters";
        response.StatusCode = HttpStatusCode.BadRequest; // 400 
      }
      else
      {
        MemberSystem memberSystem = new MemberSystem();
        Dictionary<string, object> jwt = jws.Decode(token);
        if (!jwt.ContainsKey("Email") || !jwt.ContainsKey("Gender") || !jwt.ContainsKey("Birthday"))
        {
          isSuccess = false;
          messages = "Invalid operation.";
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
            Psw = ChoyPassword.Hash(data.Password, TimeConverter.ToTimestamp(now)),
            NickName = name,
            Gender = (bool)jwt["Gender"],
            Bday = TimeConverter.ToDateTime((long)jwt["Birthday"]),
            ContactEmail = (string)jwt["Email"],
            CreateAt = now,
            ProfilePic = memberSystem.GetFileBytes("\\Images\\carot.png"),
            ImageMimeType = "image/png",
            PerCode = 0,
            IsSuspended = false,
            LastLogInTime = now
          };
          if (!memberSystem.Register(member))
          {
            isSuccess = false;
            messages = "Registration failed";
            response.StatusCode = HttpStatusCode.InternalServerError; // 500
          }
          else
          {
            isSuccess = true;
            messages = "Registration success";
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
    public HttpResponseMessage VerifyEmailAddress(ApiAuthVerifyEmailAddress user)
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
          messages = "The verification letter has been sent.";
          response.StatusCode = HttpStatusCode.OK; // 200
          break;
        case 1:
          isSuccess = false;
          messages = "Failed to send verification letter.Email is required or email format is wrong.";
          response.StatusCode = HttpStatusCode.BadRequest; // 400
          break;
        case 2:
          isSuccess = false;
          messages = "Failed to send verification letter.This email is already registered.";
          response.StatusCode = HttpStatusCode.BadRequest; // 400
          break;
        case 3:
          isSuccess = false;
          messages = "Failed to send verification letter.Your username cannot be longer than 15 characters.";
          response.StatusCode = HttpStatusCode.BadRequest; // 400
          break;
        case 4:
          isSuccess = false;
          messages = "Failed to send verification letter.Birthday is required and no later than today. ";
          response.StatusCode = HttpStatusCode.BadRequest; // 400
          break;
        default:
          isSuccess = false;
          messages = "Failed to send verification letter.Unknown mistake";
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

    [HttpPost]
    [Route("forgetPassword")]
    public HttpResponseMessage ForgetPassword(ApiAuthForgetPassword request)
    {
      var response = new JsonResponse();

      if (string.IsNullOrWhiteSpace(request.Email) || !Validator.IsValidEmail(request.Email))
      {
        response.Set(new
        {
          Success = false,
          Message = "信箱格式錯誤，請重新輸入 !!"
        }, HttpStatusCode.BadRequest); // Http Status Code: 400

        return response.Get();
      }

      var db = new ChoyContext();
      var data = db.Members.Where(m => m.Email == request.Email).FirstOrDefault();

      if (data != null)
      {
        // 產生暫時密碼
        string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        int passwordLength = 18;//密碼長度
        char[] chars = new char[passwordLength];
        Random rd = new Random();
        for (int i = 0; i < passwordLength; i++)
        {
          chars[i] = allowedChars[rd.Next(0, allowedChars.Length)]; // 隨機從 allowedChars 取得一個字
        }
        string password = new string(chars);

        var env = new Env();
        var jws = new SimpleJws();
        var payload = new Dictionary<string, object>();
        var exp = DateTime.Now.AddMinutes(45);

        payload.Add("MemberID", data.MemberID);
        payload.Add("Password", password);
        payload.Add("exp", TimeConverter.ToTimestamp(exp)); // 時效 45

        string token = jws.Encode(payload, env.SecretKey);
        string link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/Home/SetRandenPassword");

        var client = new RestClient(env.SendForgetPassworadEmailAPI);
        client.Timeout = -1;
        var req = new RestRequest(Method.POST);
        req.AddHeader("Content-Type", "application/json");

        var content = new
        {
          Recipient = data.ContactEmail,
          NickName = data.NickName,
          Link = link + $"?Token={token}",
          Exp = exp.ToString("MM/dd/yyyy HH:mm:ss"),
          Password = password,
        };

        req.AddParameter("application/json", JsonConvert.SerializeObject(content), ParameterType.RequestBody);
        IRestResponse res = client.Execute(req);
        // var dict_response = JsonConvert.DeserializeObject<Dictionary<string, object>>(res.Content);
        // HttpStatusCode statusCode = res.StatusCode;
        // int numericStatusCode = (int)statusCode;

        // if (!(bool)dict_response["Success"])
        // {
        //   response.Set(new
        //   {
        //     Success = false,
        //     Message = dict_response["Messages"]
        //   }, HttpStatusCode.InternalServerError); // Http Status Code: 500

        //   return response.Get();
        // }
      }
      // else
      // {
      //   response.Set(new
      //   {
      //     Success = false,
      //     Message = "此信箱尚未被註冊"
      //   }, HttpStatusCode.NotFound); // Http Status Code: 404

      //   return response.Get();
      // }

      response.Set(new
      {
        Success = true,
        Message = "如果此註冊信箱存在，暫時密碼已透過 Email 寄送，請至您於本站設定的聯絡 Email 收信 !!"
      }, HttpStatusCode.BadRequest); // Http Status Code: 400

      return response.Get();
    }


    [HttpGet]
    [Route("getSessionInfo")]
    public HttpResponseMessage GetSessionInfo() // 測試用功能
    {
      HttpResponseMessage response = new HttpResponseMessage();
      var session = ChoySession.Current;

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
