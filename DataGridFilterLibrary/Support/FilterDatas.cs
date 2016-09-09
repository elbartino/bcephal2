using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridFilterLibrary.Support
{
    public class FilterDatas 
    {
        public HashSet<FilterData> Datas { get; set; }

        public FilterDatas()
        {
            Datas = new HashSet<FilterData>();
        }

        public void AddOrUpdateData(FilterData data)
        {
            Datas.Add(data);
        }

    }
}
