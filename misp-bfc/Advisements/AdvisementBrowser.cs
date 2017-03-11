using Misp.Bfc.Model;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Bfc.Advisements
{
    public class AdvisementBrowser : Browser<AdvisementBrowserData>
    {
       public AdvisementBrowser(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }


       public AdvisementType AdvisementType { get; set; }
       public AdvisementBrowser(Kernel.Domain.SubjectType subjectType, String functionality, AdvisementType advisementType) : base(subjectType,functionality)
       {
           this.AdvisementType = advisementType;
       }

        protected override string getTitle()
        {
            if (this.AdvisementType.ToString().Equals(AdvisementType.MEMBER))
                return "Member Advisement";

            if (this.AdvisementType.ToString().Equals(AdvisementType.EXCEPTIONAL))
                return "Exception Advisement";

            if (this.AdvisementType.ToString().Equals(AdvisementType.SETTLEMENT))
                return "Settlement Advisement";

            return "Prefunding Advisement"; 
        }

        protected override int getColumnCount()
        {
            if (this.AdvisementType.ToString().Equals(AdvisementType.MEMBER))
                return 7;

            if (this.AdvisementType.ToString().Equals(AdvisementType.EXCEPTIONAL))
                return 6;

            if (this.AdvisementType.ToString().Equals(AdvisementType.SETTLEMENT))
                return 6;

            return 7;
        }

        protected override System.Windows.Controls.DataGridColumn getColumnAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridTextColumn();
                case 1: return new DataGridTextColumn();
                case 2: return new DataGridTextColumn();
                case 3: return new DataGridTextColumn();
                case 4: return new DataGridCheckBoxColumn();
                case 5: return new DataGridCheckBoxColumn();
                default: return new DataGridTextColumn();
            }
        }

        protected override string getColumnHeaderAt(int index)
        {
            switch (index)
            {
                case 0: return "Name";
                case 1: return "Group";
                case 2: return "Creation Date";
                case 3: return "Modification Date";
                case 4: return "Active";
                case 5: return "Template";
                default: return "";
            }
        }

        protected override System.Windows.Controls.DataGridLength getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 1: return 150;
                case 2: return 120;
                case 3: return 120;
                case 4: return 70;
                case 5: return 70;
                default: return 100;
            }
        }

        protected override string getBindingNameAt(int index)
        {
            switch (index)
            {
                case 0: return "name";
                case 1: return "group";
                case 2: return "creationDateTime";
                case 3: return "modificationDateTime";
                case 4: return "active";
                case 5: return "template";
                case 6: return "visibleInShortcut";
                default: return "oid";
            }
        }
    }
}
