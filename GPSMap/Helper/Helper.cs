using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GPSMap.Helper
{
    public static class Helper
    {
        public static decimal? ToNullableDecimal(object val)
        {
            if (val is DBNull ||
                val == null)
            {
                return null;
            }
            if (val is string &&
                ((string)val).Length == 0)
            {
                return null;
            }
            return Convert.ToDecimal(val);
        }
        public static void LogFile(Exception ex, string errorFolderPath)
        {
            var fileName = DateTime.Now.Ticks + ".txt";
            if (!Directory.Exists(Path.Combine(errorFolderPath)))
            {
                Directory.CreateDirectory(Path.Combine(errorFolderPath));
            }
            var path = Path.Combine(errorFolderPath, fileName);
            try
            {
                using (var file = new System.IO.StreamWriter(path))
                {
                    file.WriteLine(ex);
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
}