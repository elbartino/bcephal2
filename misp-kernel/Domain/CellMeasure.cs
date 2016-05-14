using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class CellMeasure : Persistent
    {

        public string name { get; set; }

        public Measure measure { get; set; }

        public bool calculated { get; set; }

        public bool ascRanking { get; set; }

        public bool groupByObject { get; set; }

        public bool repeatMeasure { get; set; }

        public string formula { get; set; }

        public string sheet { get; set; }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is CellMeasure)) return 1;
            return this.name.CompareTo(((CellMeasure)obj).name);
        }

        public CellMeasure GetCopy()
        {
            CellMeasure cellMeasure = new CellMeasure();
            cellMeasure.ascRanking = this.ascRanking;
            cellMeasure.calculated = this.calculated;
            cellMeasure.groupByObject = this.groupByObject;
            cellMeasure.repeatMeasure = this.repeatMeasure;
            cellMeasure.measure = this.measure;
            cellMeasure.name = this.name;
            return cellMeasure;
        }

    }
}
