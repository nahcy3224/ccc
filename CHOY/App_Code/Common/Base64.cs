using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CHOY.App_Code.Common
{
  public class Base64
  {
    public string Encode(string str)
    {
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }
    public string Decode(string str_base64)
    {      
      return Encoding.UTF8.GetString(Convert.FromBase64String(str_base64));
    }
    public string UrlEncode(string str)
    {
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(str))
        .Replace("=", "")
        .Replace("+", "-")
        .Replace("/", "_");
    }
    public string UrlDecode(string str_base64)
    {
      if (str_base64.Length % 4 != 0)
      {
        for (int i = 0; i < str_base64.Length % 4; i++)
        {
          str_base64 += "=";
        }
      }
      str_base64.Replace("-", "+").Replace("_", "/");
      
      return Encoding.UTF8.GetString(Convert.FromBase64String(str_base64));
    }
  }
}