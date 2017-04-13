using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class NumberUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static decimal ToDecimal(String text)
        {
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            decimal amount = 0;
            try 
            {
                if (!string.IsNullOrWhiteSpace(text)) amount = decimal.Parse(text.Trim());
            }
            catch(Exception e)
            {
                
            }            
            return amount;
        }

        /// <summary>
        /// dot as thousand separator, 
        /// coma as decimal separator
        /// </summary>
        /// <param name="numberValue"></param>
        public static string ToGermanFormat(decimal? numberValue){
            if (numberValue == null) numberValue= 00;
           string formatedDecimal =  numberValue.Value.ToString("N2");
           //string formatedDecimal =  numberValue.Value.ToString("N", CultureInfo.CreateSpecificCulture("de-DE"));
          return formatedDecimal;
        }

        //public static string ToStandardFormat(decimal? numberValue){
        //    if (numberValue == null) numberValue= 00;
        //    string formatedDecimal =  numberValue.Value.ToString("N2", CultureInfo.InvariantCulture);
        //    return formatedDecimal;
        //}

        public static string ToNormalString(string formatedString)
        {
            if (formatedString == null) return "";
            char[] thousandSeperator = new char[] { '.', ',', ' ' };
            char thSeparator = ' ';
            char decimalSeparator = ' ';
            foreach (char s in thousandSeperator)
            {
                if (Regex.Matches(formatedString, ""+s, RegexOptions.IgnoreCase).Count == 1)
                {
                    decimalSeparator = s;
                }

                if (Regex.Matches(formatedString, ""+s, RegexOptions.IgnoreCase).Count > 1 && formatedString.Length > 7)
                {
                    thSeparator = s;
                }
                
            }
             formatedString = formatedString.Replace(""+thSeparator,string.Empty);
             return formatedString;
        }
    }
}
