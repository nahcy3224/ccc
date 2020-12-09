namespace CHOY.App_Code.Common.Hash
{
    public abstract class HmacAlg
    {
    public abstract string Hash(string data, string secretKey);
    public bool Verify(string data, string hash, string secretKey)
    {
      return Hash(data, secretKey) == hash;
    }
  }
}