using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class NumberUtil
    {
        /// <summary>
        /// dot as thousand separator, 
        /// coma as decimal separator
        /// </summary>
        /// <param name="numberValue"></param>
        public static string ToGermanFormat(decimal? numberValue){
            if (numberValue == null) numberValue= 00;
            string formatedDecimal =  numberValue.Value.ToString("N", CultureInfo.CreateSpecificCulture("de-DE"));
            return formatedDecimal;
        }
    }
}
