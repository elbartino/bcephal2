using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Office
{
    public class ExcelEventArg : EventArgs
    {

        /// <summary>
        /// Gets or sets a value that indicates the Range.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the Sheet.
        /// </summary>
        public Sheet Sheet { get; set; }


        /// <summary>
        /// Initializes a new instance of the ExcelEventArg class.
        /// </summary>
        public ExcelEventArg(){}
        
        /// <summary>
        /// Initializes a new instance of the ExcelEventArg class.
        /// </summary>
        /// <param name="sheet">The sheet</param>
        /// <param name="range">The range</param>
        public ExcelEventArg(Sheet sheet, Range range) : this() 
        {
            this.Sheet = sheet;
            this.Range = range;
        }
    }
}
