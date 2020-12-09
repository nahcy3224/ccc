using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHOY.App_Code.Common
{
  public class TimeConverter
  {
    // DateTime 轉 Timestamp
    public static long ToTimestamp(DateTime dateTime)
    {
      // 當地時區的 1970-01-01 00:00:00
      DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
      return (long)(dateTime - startTime).TotalSeconds; // 相差秒數
    }
    // Timestamp 轉 DateTime
    public static DateTime ToDateTime(long seconds)
    {
      // 當地時區的 1970-01-01 00:00:00
      DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
      return startTime.AddSeconds((double)seconds);
    }
  }
}