using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public  class TableActionData
    {
        public List<int> oids { get; set; }
        public List<InputTableBrowserData> datas { get; set; }
        public Kernel.Ui.Office.Range range { get; set; }
        public bool writeInExcel { get; set; }

        public bool saveBeforePerformAction { get; set; }
        public bool clearBeforePerformAction { get; set; }
        public string name { get; set; }

        public TableActionData()
        {
            this.oids = new List<int>(0);
            this.datas = new List<InputTableBrowserData>(0);
            saveBeforePerformAction = false;
            clearBeforePerformAction = false;
            writeInExcel = false;
        }

        public TableActionData(List<int> oids) : this()
        {
            this.oids = oids;
        }
        
        public TableActionData(System.Collections.IList objects) : this()
        {
            foreach (object obj in objects)
            {
                if (obj is InputTableBrowserData)
                {
                    this.oids.Add(((InputTableBrowserData)obj).oid);
                    this.datas.Add((InputTableBrowserData)obj);
                }
            }
        }

        public TableActionData(int oid, Ui.Office.Range range) : this()
        {
            this.oids.Add(oid);
            this.range = range;
        }

        List<TransformationTreeBrowserData> treeData;
        public TableActionData(List<TransformationTreeBrowserData> objects) 
        {
            treeData = objects;
        }
    }
}
