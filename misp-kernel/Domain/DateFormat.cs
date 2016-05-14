using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class DateFormat
    {

        private static List<string> formats;

        public static string[] FormatsArray
        {
            get 
            {
                return Formats.ToArray();
            }
        }

        public static List<string> Formats
        {
            get 
            {
                if(formats == null)
                { 
                    formats = new List<string>(0);
                    formats.Add("");
                    formats.Add("dd-MM-yyyy");
                    formats.Add("dd/MM/yyyy");
                    formats.Add("MM-yyyy");
                    formats.Add("MM/yyyy");
                    formats.Add("yyyy");
                }                
                return formats;
            }
        }



    }
}
