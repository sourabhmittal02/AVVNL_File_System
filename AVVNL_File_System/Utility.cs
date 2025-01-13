using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AVVNL_File_System
{
    public class Utility
    {
        public static string Connection
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;
            }
        }
    }
}