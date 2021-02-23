using CHOY.App_Code.Auth.Jwt;
using CHOY.App_Code.Common;
using CHOY.DAL;
using CHOY.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using System.IO;
using System.Web;

namespace CHOY.App_Code.Auth
{
  public class MemberSystem
  {
    ChoyContext db = new ChoyContext();
    // 登入
    public bool Login(string account, string password)
    {

      Member user = Validator.IsValidEmail(account)
        ? db.Members.Where(member => member.Email == account).FirstOrDefault()
        : db.Members.Where(member => member.MemberID == account).FirstOrDefault();



      if (user == null)
      {
        return false;
      }

      long salt = TimeConverter.ToTimestamp(user.CreateAt);
      
      if (!ChoyPassword.Validate(password, salt, user.Psw))
      {
        return false;
      }

      ChoySession session = ChoySession.Current;
      session.LoginId = user.MemberID;
      session.LoginAt = TimeConverter.ToTimestamp(DateTime.Now);
      session.PerCode = user.PerCode;

      return true;
    }
    // 登出
    public void Logout()
    {
      ChoySession.Clear(); // 清除 session
    }
    // 註冊
    public bool Register(Member member)
    {
      try
      {
        db.Members.Add(member);
        db.SaveChanges();
      }
      catch
      {
        return false;
      }

      return true;
    }
    public byte[] GetFileBytes(string path)
    {
      FileStream fileOnDisk = new FileStream(HttpRuntime.AppDomainAppPath + path, FileMode.Open);
      byte[] fileBytes;

      using (BinaryReader br = new BinaryReader(fileOnDisk))
      {
        fileBytes = br.ReadBytes((int)fileOnDisk.Length);
      }
      return fileBytes;

    }
    /**
     * memberSystem.SendValidationEmailByAPI() 回復所代表意義
     * return 0 : 表示 Email 寄送成功
     * return 1 : 表示 Email 不能為空 || Email 格式錯誤
     * return 2 : 表示 Email 已被註冊
     * return 3 : 表示 UserName 不能超過 15 個字
     * return 4 : 表示 Birthday 不能為空 || Birthday 不能晚於今日
     */
    public int SendValidationEmailByAPI(string Email, string UserName, bool Gender, DateTime Birthday, string link)
    {
      DateTime now = DateTime.Now;
      // Email 不能為空 || Email 格式錯誤
      if (Email == null || !Validator.IsValidEmail(Email))
      {
        return 1;
      }
      // Email 已被註冊
      Member user = db.Members.Where(member => member.Email == Email).FirstOrDefault();
      if (user != null)
      {
        return 2;
      }
      // UserName 不能超過 15 個字
      if (UserName != null && UserName.Length > 15)
      {
        return 3;
      }
      // Birthday 不能為空 || Birthday 不能晚於今日
      if (Birthday == null || DateTime.Compare(now, Birthday) < 0)
      {
        return 4;
      }

      Env env = new Env();
      SimpleJws jws = new SimpleJws();
      Dictionary<string, object> payload = new Dictionary<string, object>();
      payload.Add("Email", Email);

      if (UserName != null)
      {
        payload.Add("UserName", UserName);
      }
      payload.Add("Gender", Gender);
      payload.Add("Birthday", TimeConverter.ToTimestamp(Birthday));
      payload.Add("exp", TimeConverter.ToTimestamp(now.AddMinutes(30)));
      string token = jws.Encode(payload, env.SecretKey);

      var client = new RestClient(env.SendEmailAPI);
      client.Timeout = -1;
      var request = new RestRequest(Method.POST);
      request.AddHeader("Content-Type", "application/json");


      var content = new
      {
        Recipient = Email,
        Link = link.Split('#').Length > 1 ? link.Replace("/#", $"?token={token}#") : $"{link}?token={token}",
        Name = UserName ?? null
      };
      request.AddParameter("application/json", JsonConvert.SerializeObject(content), ParameterType.RequestBody);
      IRestResponse response = client.Execute(request);
      Dictionary<string, object> dict_response = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

      return 0;
    }
  }
}