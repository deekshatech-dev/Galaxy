using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPSMap.Data
{
    public static class ExtensionMethods
    {
        public static System.Web.Mvc.SelectList ToSelectList<TEnum>(this TEnum obj, string selectedValues = null)
        where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return new SelectList(Enum.GetValues(typeof(TEnum))
            .OfType<Enum>()
            .Select(x => new SelectListItem
            {
                Text = Enum.GetName(typeof(TEnum), x),
                Value = (Convert.ToString(x))
            }), "Value", "Text");
        }

    }
}