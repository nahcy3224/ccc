namespace CHOY.App_Code.Common.Hash
{
  public abstract class HashAlg
  {
    public abstract string Hash(string data);
    public bool Verify(string data, string hash)
    {
      return Hash(data) == hash;
    }
  }
}