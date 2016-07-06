using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackUsersList_Windows.Extensions
{
    public static class StringExtension
    {
        public static string CapitalizeFirstLetter(this string s) 
        { 
            if (String.IsNullOrEmpty(s)) return s; 
            if (s.Length == 1) return s.ToUpper(); 
            return s[0].ToString().ToUpper() + s.Substring(1); 
        } 
    }
}
