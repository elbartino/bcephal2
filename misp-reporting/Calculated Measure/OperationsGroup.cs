using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reporting.Calculated_Measure
{
    public class OperationsGroup : SideBarExpander
    {
        private System.Collections.ObjectModel.ObservableCollection<string> operationItems = new System.Collections.ObjectModel.ObservableCollection<string>();
       #region Properties

        public OperationTreeview OperationTreeview { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public OperationsGroup() : base() { }

        public OperationsGroup(string header) : base(header) { }

        public OperationsGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            operationItems.Add("+"); operationItems.Add("-"); operationItems.Add("*"); operationItems.Add("/"); operationItems.Add("^"); operationItems.Add("("); operationItems.Add(")"); operationItems.Add("=");

            base.InitComponents();

            this.OperationTreeview= new OperationTreeview();
            this.OperationTreeview.fillTree(operationItems);

            this.ContentPanel.Children.Add(this.OperationTreeview);
        }

        #endregion   
    }
}
