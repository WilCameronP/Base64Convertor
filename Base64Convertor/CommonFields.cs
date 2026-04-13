using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Base64Convertor
{
    public static class CommonFields
    {
        static CommonFields()
        {
            var ass = Assembly.GetExecutingAssembly();
            AppVersion = ass.GetName().Version.ToString(3);
            AppRootPath = Path.GetDirectoryName(ass.Location);
#if DEBUG 
            var appPathMatcher = new System.Text.RegularExpressions.Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            AppRootPath = appPathMatcher.Match(AppRootPath).Value;
#endif            
        }

        public static string AppVersion { get; }

        public static string AppRootPath { get; }
    }
}
