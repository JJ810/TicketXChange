using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketsXchange.Helper
{
    public static class SessionHelper
    {
        public static int? UserID
        {
            get
            {
                if (HttpContext.Current.Session == null)
                {
                    return new int?();
                }
                if ((HttpContext.Current.Session["UserID"] == null))
                    HttpContext.Current.Session.Add("UserID", new int?());
                return HttpContext.Current.Session["UserID"] as int?;
            }
            set {
                if (HttpContext.Current.Session == null)
                {
                    HttpContext.Current.Session.Add("UserID", new int?());
                }
                HttpContext.Current.Session["UserID"] = value;
               
            }
                
        }

    }
}