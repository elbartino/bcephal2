using Misp.Kernel.Ui.Base;
using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridSideBar : SideBar
    {

        #region Constructor


        #endregion


        #region Properties

        public GrilleGroup GrilleGroup { get; set; }
        public EntityGroup EntityGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public MeasureGroup MeasureGroup { get; set; }
        public PeriodNameGroup PeriodNameGroup { get; set; }
        //public CalculatedMeasureGroup CalculateMeasureGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeGroups()
        {
            base.InitializeGroups();
            this.GrilleGroup = new GrilleGroup("Grids", true);
            this.EntityGroup = new EntityGroup("Entities", true);
            this.TargetGroup = new TargetGroup("Targets", true);
            this.MeasureGroup = new MeasureGroup("Measure", true);
            this.PeriodNameGroup = new PeriodNameGroup("Period", true);
            //this.CalculateMeasureGroup = new CalculatedMeasureGroup("Calculated Measure", true);

            this.GrilleGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;
            //this.CalculateMeasureGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.GrilleGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.PeriodNameGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.EntityGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.MeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            //this.CalculateMeasureGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;


            this.AddGroup(this.GrilleGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.TargetGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.PeriodNameGroup);
            //this.AddGroup(this.CalculateMeasureGroup);
        }


        #endregion

    }
}
