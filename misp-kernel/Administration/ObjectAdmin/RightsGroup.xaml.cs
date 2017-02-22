using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Kernel.Administration.ObjectAdmin
{
    /// <summary>
    /// Interaction logic for RightsGroup.xaml
    /// </summary>
    public partial class RightsGroup : Expander
    {

        
        #region Properties

        public String ObjectType { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public RightsGroup()
        {
            InitializeComponent();
        }

        public RightsGroup(String ObjectType) : this()
        {
            this.ObjectType = ObjectType;
            Customize();
        }

        #endregion


        #region Operations


        #endregion


        #region Initializations

        private void Customize()
        {
            if (!string.IsNullOrWhiteSpace(ObjectType))
            {                
                if (ObjectType.Equals(SubjectType.INPUT_TABLE.label))
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                }
                else if (ObjectType.Equals(SubjectType.REPORT.label))
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                }
                else if (ObjectType.Equals(SubjectType.INPUT_GRID.label))
                {
                    AddCheckBox(new RightCheckBox("View", RightType.VIEW));
                }
            }
        }

        protected void AddCheckBox(RightCheckBox checkBox)
        {
            //checkBox.Checked += ;
            //checkBox.Unchecked += ;
            RightsPanel.Children.Add(checkBox);
        }

        #endregion


        #region Handlers


        #endregion

    }
}
