using DevExpress.Xpf.Grid;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.Profil
{
    public class ProfilBrowser : Browser<ProfilBrowserData>
    {

        public ProfilBrowser(Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected override int getColumnCount()
        {
            return 3;
        }

        protected override string getTitle() { return "Profils"; }



        /// <summary>
        /// Column Label
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getColumnHeaderAt(int index)
        {
            switch (index)
            {
                case 0: return "Name";
                case 1: return "Creation Date";
                case 2: return "Active";
                default: return "";
            }
        }

        /// <summary>
        /// Column Width
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override GridColumnWidth getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return new GridColumnWidth(1, GridColumnUnitType.Star);
                case 1: return 120;
                case 2: return 100;
                default: return 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getFieldNameAt(int index)
        {
            switch (index)
            {
                case 0: return "name";
                case 1: return "creationDateTime";
                case 2: return "active";
                default: return "oid";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getBindingStringFormatAt(int index)
        {
            switch (index)
            {
                case 1: return "{0:dd/MM/yyyy HH:mm:ss}";
                default: return null;
            }
        }

        protected override bool isReadOnly(int index)
        {
            switch (index)
            {
                case 0: return false;
                case 1: return true;
                case 2: return false;
                default: return true;
            }
        }
    }
}
