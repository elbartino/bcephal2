using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Misp.Kernel.Domain;


namespace Misp.Kernel.Ui.Base
{
    public class TestBrowser : Browser<Persistent>
    {

        public TestBrowser()
        {
            Datas.Add(new Persistent());
            Datas.Add(new Persistent());
            Datas.Add(new Persistent());
            Datas.Add(new Persistent());
            Datas.Add(new Persistent());
            DisplayDatas(Datas);
        }

        protected override string getTitle() { return "Tests"; }

        protected override int getColumnCount()
        {
            return 4;
        }

        /// <summary>
        /// Build and returns the column at index position
        /// </summary>
        /// <param name="index">The position of the column</param>
        /// <returns></returns>
        protected override DataGridColumn getColumnAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridTextColumn();
                case 1: return new DataGridTextColumn();
                case 2: return new DataGridTextColumn();
                case 3: return new DataGridTextColumn();
                default: return new DataGridTextColumn();
            }
        }

        /// <summary>
        /// Column Label
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getColumnHeaderAt(int index)
        {
            switch (index)
            {
                case 0: return "Oid";
                case 1: return "State";
                case 2: return "Creation Date";
                case 3: return "Modification Date";
                default: return "Column";
            }
        }

        /// <summary>
        /// Column Width
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override DataGridLength getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return 50;
                case 1: return new DataGridLength(0.2, DataGridLengthUnitType.Star);
                case 2: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 3: return new DataGridLength(1, DataGridLengthUnitType.Star);
                default: return 90;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getBindingNameAt(int index)
        {
            switch (index)
            {
                case 0: return "Oid";
                case 1: return "PersistentState";
                case 2: return "CreationDate";
                case 3: return "ModificationDate";
                default: return "Oid";
            }
        }


    }
}
