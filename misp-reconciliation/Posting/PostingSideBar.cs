using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Posting
{
    public class PostingSideBar : BrowserSideBar
    {

        #region Properties
        public EntityGroup EntityGroup { get; set; }
        public PeriodNameGroup PeriodNameGroup { get; set; }
        
        #endregion
        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.GroupCatagoryGroup.Visibility = System.Windows.Visibility.Collapsed;
            this.GroupGroup.Visibility = System.Windows.Visibility.Collapsed;
            this.EntityGroup = new EntityGroup("Entities", true);
            this.PeriodNameGroup = new PeriodNameGroup("Period", true);
            
            this.EntityGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.Background = System.Windows.Media.Brushes.LightBlue;


            this.EntityGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.PeriodNameGroup);
        }

        #endregion
    }
}
