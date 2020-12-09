using CHOY.App_Code.Common;
using CHOY.App_Code.Common.Hash;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CHOY.App_Code.Auth.Jwt
{
  public class SimpleJws
  {
    public string Encode(Dictionary<string, object> payload, string secretKey)
    {
      DateTime now = DateTime.Now;
      Base64 base64 = new Base64();
      

      string tokenHeader = base64.UrlEncode(JsonConvert.SerializeObject(new { alg = "HS256", typ = "JWT" }));
      payload.Add("iat", TimeConverter.ToTimestamp(now)); // 發行時間
      if (!payload.ContainsKey("exp"))
      {
        payload.Add("exp", TimeConverter.ToTimestamp(now.AddHours(3))); // 到期時間
      }

      string tokenPayload = base64.UrlEncode(JsonConvert.SerializeObject(payload));
      string tokenSignature = new JwsAlgorithm().Hash("HS256", tokenHeader, tokenPayload, secretKey);

      return $"{tokenHeader}.{tokenPayload}.{tokenSignature}";
    }
    public Dictionary<string, object> Decode(string token)
    {
      Base64 base64 = new Base64();
      Regex regex = new Regex(@"^[a-zA-Z0-9_\-]+.[a-zA-Z0-9_\-]+.[a-zA-Z0-9_\-]+$");
      if (regex.IsMatch(token))
      {
        try
        {
          string tokenPayload = token.Split('.')[1];
          string jsonPayload = base64.UrlDecode(tokenPayload);
          Dictionary<string, object> dictPayload = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonPayload);
          return dictPayload;
        }
        catch
        {
          return null;
        }
      }
      return null;
    }
    public bool Validate(string token, string secretKey)
    { // 實作自己的檢查方法
      Regex regex = new Regex(@"^[a-zA-Z0-9_\-]+.[a-zA-Z0-9_\-]+.[a-zA-Z0-9_\-]+$");
      if (!regex.IsMatch(token))
      {
        return false;
      }

      long now = TimeConverter.ToTimestamp(DateTime.Now);
      string tokenHeader = token.Split('.')[0];
      string tokenPayload = token.Split('.')[1];
      string tokenSignature = token.Split('.')[2];
      Dictionary<string, object> dictHeader, dictPayload;

      try
      {
        Base64 base64 = new Base64();
        string jsonHeader = base64.UrlDecode(tokenHeader);
        string jsonPayload = base64.UrlDecode(tokenPayload);
        dictHeader = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonHeader);
        dictPayload = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonPayload);
      }
      catch
      {
        return false;
      }

      string type = dictHeader.ContainsKey("typ") ? ((string)dictHeader["typ"]).ToUpper() : null;
      string algorithm = dictHeader.ContainsKey("alg") ? ((string)dictHeader["alg"]).ToUpper() : null;

      if (type != null && type != "JWT") // 可以沒有 typ 宣告，但若有則必須為 JWT
      {
        return false;
      }

      if (algorithm != "HS256") // alg 為必要欄位。本實作只接受 HS256
      {
        return false;
      }

      /***
       * 與開放規範不同的地方，本次實作將 exp 與 iat 視為必要欄位
       */
      if (dictPayload.ContainsKey("exp") && dictPayload.ContainsKey("iat"))
      {
        long exp = (long)dictPayload["exp"];
        long iat = (long)dictPayload["iat"];

        // 必須滿足 exp > now > nbf > iat
        if (!((exp > iat) && (exp > now)))
        {
          return false;
        }
        if (dictPayload.ContainsKey("nbf"))
        {
          long nbf = (long)dictPayload["nbf"];
          if (!((exp > nbf) && (nbf > iat) && (now > nbf)))
          {
            return false;
          }
        }
      }
      else
      {
        return false;
      }

      
      string signature = new JwsAlgorithm().Hash("HS256", tokenHeader, tokenPayload, secretKey);

      if (signature != tokenSignature)
      {
        return false;
      }

      return true;
    }
  }
}