using System.Security.Cryptography;
using System.Text;

namespace CHOY.App_Code.Common.Hash
{ 
  public class HS256 : HmacAlg
  {
    public override string Hash(string data, string secretKey)
    {
      byte[] bytes_secretKey = Encoding.UTF8.GetBytes(secretKey);
      byte[] bytes_data = Encoding.UTF8.GetBytes(data);
      StringBuilder builder = new StringBuilder();
      using (HMACSHA256 hmacSHA256 = new HMACSHA256(bytes_secretKey))
      {
        byte[] hash = hmacSHA256.ComputeHash(bytes_data);
        for (int i = 0; i < hash.Length; i++)
        {
          builder.Append(hash[i].ToString("x2"));
        }
      }
      return builder.ToString();
    }
  }
}