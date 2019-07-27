using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildingManagement.Utils
{
    public class Constants
    {
        public enum RoleTypes
        {
            User = 1,
            Admin = 2,
            Sysadmin = 3
        }
    }
}