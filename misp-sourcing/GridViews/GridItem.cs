using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.GridViews
{
    public class GridItem
    {

        public bool IsSelected { get; set; }
        public Object[] Datas { get; set; }

        public GridItem() { IsSelected = false; }

        public GridItem(Object[] datas) { IsSelected = false; Datas = datas; }

        public int? GetOid()
        {
            if (Datas == null || this.Datas.Length <= 0) return null;
            object obj = this.Datas[this.Datas.Length - 1];
            if (obj == null) return null;
            int oid;
            if (int.TryParse(obj.ToString(), out oid)) return oid;
            return null;
        }
    }
}
