using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Office
{
    [Serializable]
    public class Sheet
    {

        public int Index { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Sheet() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        public Sheet(int index, string name) 
        {
            this.Index = index;
            this.Name = name;
        }

    }

}
