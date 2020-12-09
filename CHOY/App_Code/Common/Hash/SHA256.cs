using System.Security.Cryptography;
using System.Text;

namespace CHOY.App_Code.Common.Hash
{
    public class SHA256 : HashAlg
  {
    public override string Hash(string data)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(data);
      byte[] hash = SHA256Managed.Create().ComputeHash(bytes);
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < hash.Length; i++)
      {
        builder.Append(hash[i].ToString("x2"));
      }

      return builder.ToString();
    }
  }
}