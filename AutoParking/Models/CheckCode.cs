using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoParking.Models
{
    public class CheckCode
    {
       static List<string> _code = new List<string> { "M1", "M2" };

        public static bool checkcode(string code)
        {
            var check = _code.Find(x => x == code);
            if (check != null)
            {
                return true;
            }
            else
                return false;
        }
    }
}