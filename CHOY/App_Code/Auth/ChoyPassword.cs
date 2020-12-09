using CHOY.App_Code.Common.Hash;

namespace CHOY.App_Code.Auth
{
  public class ChoyPassword
  {
    public static string Hash(string password, long salt)
    {
      Env env = new Env();
      SHA256 sha256 = new SHA256();
      HS256 hs256 = new HS256();
      return hs256.Hash($"{sha256.Hash(salt.ToString())}.{password}", env.SecretKey);
    }
    public static bool Validate(string password, long salt, string hashPassword)
    {
      return Hash(password, salt) == hashPassword;
    }
  }
}