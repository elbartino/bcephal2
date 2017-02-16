using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.GridViews
{
    public class GridItem
    {
        public static String LEFT_SIDE = "L";
        public static String RIGHT_SIDE = "R";

        public String Side { get; set; }
        public bool IsSelected { get; set; }
        public Object[] Datas { get; set; }

        public GridItem(String side = null) { IsSelected = false; Side = side; }

        public GridItem(Object[] datas, String side = null) : this(side) { IsSelected = false; Datas = datas; }

        public int? GetOid()
        {
            if (Datas == null || this.Datas.Length <= 0) return null;
            object obj = this.Datas[this.Datas.Length - 1];
            if (obj == null) return null;
            int oid;
            if (int.TryParse(obj.ToString(), out oid)) return oid;
            return null;
        }

        public bool IsLeftSide()
        {
            return !String.IsNullOrWhiteSpace(this.Side) && this.Side == LEFT_SIDE;
        }

        public bool IsRightSide()
        {
            return !String.IsNullOrWhiteSpace(this.Side) && this.Side == RIGHT_SIDE;
        }

    }
}
