using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ExcelFileExtension
    {

        public static ExcelFileExtension XLS = new ExcelFileExtension(".xls");
	
        public static ExcelFileExtension XLSX = new ExcelFileExtension(".xslx");
	
	    public String Extension{get; private set;}
	
	    public ExcelFileExtension(String extension) {
		    this.Extension = extension;
	    }
		
	    public override String ToString() {
		    return Extension;
	    }

        public bool equals(String ext)
        {
            if (string.IsNullOrEmpty(ext)) return false;
            return ext.Trim().ToUpper().Equals(this.Extension.ToUpper());
        }

        public static ExcelFileExtension getByExtension(String ext)
        {
            if (string.IsNullOrEmpty(ext)) return null;
            if (XLS.equals(ext)) return XLS;
            if (XLSX.equals(ext)) return XLSX;
            return null;
        }

    }
}
