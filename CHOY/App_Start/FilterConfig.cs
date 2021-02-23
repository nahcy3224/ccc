using CHOY.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHOY.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute() { View = "Error" });

            filters.Add(new checkLoginStatus());

            filters.Add(new checkPerCodeBulletin());

            filters.Add(new checkPerCodeManager());

            filters.Add(new checkPerCodeSuspension());

            //filters.Add(new checkPerCodeDownload());
        }
    }
}