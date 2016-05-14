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

namespace Misp.Sourcing.AutomaticSourcingViews
{
    /// <summary>
    /// Interaction logic for AutomaticTablePropertiesPanel.xaml
    /// </summary>
    public partial class AutomaticTablePropertiesPanel : ScrollViewer
    {
        #region Events

        public event OnNewPeriodClickedEventHandler OnNewPeriodClicked;
        public delegate void OnNewPeriodClickedEventHandler();

        #endregion

        #region Constructor
        public AutomaticTablePropertiesPanel()
        {
            InitializeComponent();
            InitializeHandlers();
        }
        #endregion

        public BGroup tableGroup;
        
        private void InitializeHandlers() 
        {
          periodPanel.CustomizeForAutomaticSourcing();
          periodPanel.CustomizeForReport();
          periodPanel.OnNewPeriodClicked +=periodPanel_OnNewPeriodClicked;
        }

       

        private void periodPanel_OnNewPeriodClicked()
        {
            if (OnNewPeriodClicked != null) OnNewPeriodClicked();   
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void fillAutomaticSourcing(AutomaticSourcing automaticSourcing, bool isTarget = false)
        {
            if (automaticSourcing == null) return;
            automaticSourcing.filter = filterScopePanel.Scope;
            automaticSourcing.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
            if (!isTarget)
            {
                automaticSourcing.period = periodPanel.Period;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void displayAutomaticSourcing(AutomaticSourcing automaticSourcing,bool isTarget = false)
        {
            if (automaticSourcing == null) return;
            
            filterScopePanel.DisplayScope(automaticSourcing.filter);
            visibleInShortcutCheckBox.IsChecked = automaticSourcing.visibleInShortcut;
            if (!isTarget)
            {
                if (automaticSourcing.tableGroup == null) automaticSourcing.tableGroup = tableGroup;
                groupGroupField.Group = automaticSourcing.tableGroup;
                periodPanel.DisplayPeriod(automaticSourcing.period);
            }
        }

     

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<System.Windows.UIElement> getEditableControls()
        {
            List<UIElement> controls = new List<UIElement>(0);
            controls.Add(this.periodGroupBox);
            controls.Add(this.filterGroupBox);
            controls.Add(this.visibleInShortcutCheckBox);
            return controls;
        }
    }
}
