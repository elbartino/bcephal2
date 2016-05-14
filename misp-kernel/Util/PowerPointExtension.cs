using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class PowerPointExtension
    {
        public static PowerPointExtension PPT = new PowerPointExtension(".ppt");
        public static PowerPointExtension PPTX = new PowerPointExtension(".pptx");

        public string Extension { get; set; }

        public PowerPointExtension(string extension) 
        {
            this.Extension = extension;
        }

        public override  string ToString() 
        {
            return this.Extension;
        }


        public static PowerPointExtension Get(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return null;
            if (ext.Trim().ToUpper().Equals(PPTX.Extension.ToUpper())) return PPTX;
            else if (ext.Trim().ToUpper().Equals(PPT.Extension.ToUpper())) return PPT;
            return null;
        }
    }
}
