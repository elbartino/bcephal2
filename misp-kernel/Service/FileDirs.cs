using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class FileDirs
    {

        public string InputTableDir { get; set; }

        public string ReportDir { get; set; }

        public string AutomaticSourcingDir { get; set; }

        public string PresentationDir { get; set; }

        public static string getTableTempFolder()
        {
            String tempPath = System.IO.Path.GetTempPath() + "bcephal\\tables\\";
            if (!System.IO.Directory.Exists(tempPath))
                System.IO.Directory.CreateDirectory(tempPath);
            return tempPath ;
        }

        public string TempTableFolder 
        {
            get { return getTableTempFolder(); }
        }
    }
}
