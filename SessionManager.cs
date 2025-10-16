using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_B_isTovar
{
    public static class SessionManager
    {
        public static int CurrentUserId { get; set; }
        public static int CurrentUserRole { get; set; } // 0 = dispatcher, 1 = driver
        public static string CurrentUsername { get; set; }
    }
}

