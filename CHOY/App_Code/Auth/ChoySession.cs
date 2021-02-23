using System.Web;

namespace CHOY.App_Code.Auth
{
  public class ChoySession
  {
    // private constructor 
    private ChoySession() 
    {
      // 可以在這邊設定預設值
    }
    /* add your session properties here. */
    public string LoginId { get; set; }
    public long LoginAt { get; set; }
    public Permissions PerCode { get; set; }

        /* Gets the current session. */
        public static ChoySession Current 
    {
      get
      {
        var session = (ChoySession)HttpContext.Current.Session["__CHOY__"];
        if (session == null)
        {
          session = new ChoySession();
          HttpContext.Current.Session["__CHOY__"] = session;
        }
        return session;
      }
    }
    // 清除 session 所有儲存的內容
    public static void Clear() 
    {
      HttpContext.Current.Session.Clear();
      HttpContext.Current.Session.Abandon();
    }
  }
}