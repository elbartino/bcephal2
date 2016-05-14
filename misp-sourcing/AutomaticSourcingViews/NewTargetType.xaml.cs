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
    /// Interaction logic for NewTargetType.xaml
    /// </summary>
    public partial class NewTargetType : UserControl
    {
        #region Events
        public event OnNewColumnEventHandler OnNewColumn;
        public delegate void OnNewColumnEventHandler();

        public event OnRemoveColumnEventHandler OnRemoveColumn;
        public delegate void OnRemoveColumnEventHandler(object item);

        public event OnRemoveColumnItemEventHandler OnRemoveColumnItem;
        public delegate void OnRemoveColumnItemEventHandler(object item);

        public event OnSelectionChangeEventHandler OnSelectColumnItem;
        public delegate void OnSelectionChangeEventHandler(object item);

        public event OnSetTargetGroupEventHandler OnSetTargetGroup;
        public delegate void OnSetTargetGroupEventHandler(string groupName);

        public event OnSelectionTargetChangeEventHandler OnSelectTarget;
        public delegate void OnSelectionTargetChangeEventHandler(object item, object operatorValue);

        #endregion

        #region Contructor
        public NewTargetType()
        {
            InitializeComponent();
            this.columnsItemsPanel.OnNewColumn +=columnsItemsPanel_OnNewColumn;
            this.columnsItemsPanel.OnRemoveColumn +=columnsItemsPanel_OnRemoveColumn;
            this.columnsItemsPanel.OnSelectColumnItem +=columnsItemsPanel_OnSelectColumnItem;
            this.columnsItemsPanel.OnRemoveColumnItem +=columnsItemsPanel_OnRemoveColumnItem;
            this.columnsItemsPanel.OnSelectTarget += columnsItemsPanel_OnSelectTarget;
            GroupTextBox.KeyUp +=GroupTextBox_KeyUp;
        }

        private void columnsItemsPanel_OnSelectTarget(object item, object operatorValue)
        {
            if (OnSelectTarget != null) OnSelectTarget(item,operatorValue);   
        }

        private void GroupTextBox_KeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                GroupTextBox.Text = "";
            }
            else if (args.Key == Key.Enter)
            {
                if (this.OnSetTargetGroup != null)
                {
                    if (GroupTextBox.Text.Length > 0)
                    {
                        string targetName = GroupTextBox.Text;
                        OnSetTargetGroup(targetName);
                    }
                }
            }
        }

        private void GroupTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (OnSetTargetGroup != null)
            {
                if(GroupTextBox.Text != "")
                OnSetTargetGroup(GroupTextBox.Text);
            }
        }
        #endregion

        public void customizeForAutomaticTarget() 
        {
            this.columnsItemsPanel.customizeForAutomaticTarget();
        }

        #region Handlers
        /// <summary>
        /// Event called when removing a  ColumnTargetItem
        /// </summary>
        /// <param name="item"></param>
        private void columnsItemsPanel_OnRemoveColumnItem(object item)
        {
            if (OnRemoveColumnItem != null)
                OnRemoveColumnItem(item);
        }

        /// <summary>
        /// Event called when selecting a ColumnTargetItem in list.
        /// </summary>
        /// <param name="item"></param>
        private void columnsItemsPanel_OnSelectColumnItem(object item)
        {
            if (OnSelectColumnItem != null) OnSelectColumnItem(item);
        }

        /// <summary>
        /// Event called when removing a new ColumnTargetItem
        /// </summary>
        /// <param name="item"></param>
        private void columnsItemsPanel_OnRemoveColumn(object item)
        {
            if (OnRemoveColumn != null)
                OnRemoveColumn(item);
        }

        /// <summary>
        /// Event called when adding a new ColumnTargetItem
        /// </summary>
        /// <param name="item"></param>
        private void columnsItemsPanel_OnNewColumn()
        {
            if (OnNewColumn != null)
                OnNewColumn();
        }
        #endregion

        public string getGroupTargetName()
        {
            return !string.IsNullOrEmpty(this.GroupTextBox.Text) ? this.GroupTextBox.Text : null;
        }
    }
}
