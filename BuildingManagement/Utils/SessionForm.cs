using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuildingManagement.Models;

namespace BuildingManagement.Utils
{
    public static class SessionForm
    {
        public static User User
        {
            get
            {
                return (User) HttpContext.Current.Session["_User"];
            }
            set
            {
                HttpContext.Current.Session.Add("_User", value);
            }
        }
    }
}