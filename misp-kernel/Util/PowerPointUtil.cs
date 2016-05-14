using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class PowerPointUtil
    {
        public static PowerPointExtension GetDefaultPowerPointExtension()
        {
            Microsoft.Office.Interop.PowerPoint.Application appl = new Microsoft.Office.Interop.PowerPoint.Application();
            
            string sVersion = appl.Version;
            switch (sVersion.ToString())
            {
                case "7.0": return PowerPointExtension.PPT;
                case "8.0": return PowerPointExtension.PPT;
                case "9.0": return PowerPointExtension.PPT;
                case "10.0": return PowerPointExtension.PPT;
                case "11.0": return PowerPointExtension.PPT;
                case "12.0": return PowerPointExtension.PPTX;
                case "14.0": return PowerPointExtension.PPTX;
                case "15.0": return PowerPointExtension.PPTX;
                default: return null;
            }
        }

        public static PowerPointExtension GetFileExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            String ext = Path.GetExtension(filePath);
            return PowerPointExtension.Get(ext);
        }

        public static String GetFileName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            String name = Path.GetFileName(filePath);
            return name;
        }
    }
}
