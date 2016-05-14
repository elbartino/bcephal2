using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class CombinedTransformationTreeItem : Persistent
    {
        public int position { get; set; }

        public TransformationTree tree { get; set; }

        [ScriptIgnore]
        public CombinedTransformationTree parent { get; set; }

        public CombinedTransformationTreeItem() { }

        public CombinedTransformationTreeItem(int position) : this()
        {
            this.position = position;
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is CombinedTransformationTreeItem)) return 1;
            return position.CompareTo(((CombinedTransformationTreeItem)obj).position);
        }

    }
}
