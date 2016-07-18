using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections;
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
    /// Interaction logic for AutomaticSourcingSheetPanel.xaml
    /// </summary>
    public partial class AutomaticSourcingSheetPanel : UserControl
    {
        #region Properties
        public AutomaticSourcingSheet AutomaticSourcingSheet { get; private set; }
        private bool throwChange = true;
        AutomaticSourcingColumn activeColumn { get; set; }
        private ContextMenu columnContextMenu { get; set; }
        private MenuItem removeColumnMenuItem { get; set; }
        private int currentIndex;

        #endregion

        #region events
        public event ChangeEventHandler Changed;

        public event OnSetFirstRowAsHeaderEventHandler SetFirstRowAsHeader;
        public delegate void OnSetFirstRowAsHeaderEventHandler(bool set);

        public event OnSelectRangeOptionChangeEventHandler SelectRangeOption;
        public delegate void OnSelectRangeOptionChangeEventHandler(bool set);

        public event OnSelectColumnChangeEventHandler SelectColumn;
        public delegate void OnSelectColumnChangeEventHandler(object item,int index);

        public event OnFocusRangeTextBoxEventHandler FocusTextBox;
        public delegate void OnFocusRangeTextBoxEventHandler(bool set);


        public event OnNewColumnEventHandler OnNewColumn;
        public delegate void OnNewColumnEventHandler();

        public event OnRemoveColumnEventHandler OnRemoveColumn;
        public delegate void OnRemoveColumnEventHandler(object item,int index);

        public event OnRemoveExcelColumnEventHandler OnRemoveExcelColumn;
        public delegate void OnRemoveExcelColumnEventHandler(object item, int index);

        public event OnRemoveColumnItemEventHandler OnRemoveColumnItem;
        public delegate void OnRemoveColumnItemEventHandler(object item);

        public event OnSelectionChangeEventHandler OnSelectColumnItem;
        public delegate void OnSelectionChangeEventHandler(object item);

        public event OnSelectionTargetChangeEventHandler OnSelectTarget;
        public delegate void OnSelectionTargetChangeEventHandler(object item, object operatorValue);

        public event OnTypeChangeEventHandler OnTypeChange;
        public delegate void OnTypeChangeEventHandler(Kernel.Application.ParameterType typeValue);

        public event OnSetPeriodEventHandler OnSetPeriod;
        public delegate void OnSetPeriodEventHandler(object parameters,bool isDateFormat);

        public event OnChooseTargetEventHandler OnChooseTarget;
        public delegate void OnChooseTargetEventHandler(object parameters);

        public event OnChooseTargetTypeEventHandler OnChooseTargetType;
        public delegate void OnChooseTargetTypeEventHandler(object parameters);

        public event OnSetTagNameEventHandler OnSetTagName;
        public delegate void OnSetTagNameEventHandler(string tagName);

        public event OnAllocationPanelChangeEventHandler OnAllocationPanelChange;
        public delegate void OnAllocationPanelChangeEventHandler();

        public event OnSelectedRangeChangeEventHandler OnSelectedRangeChange;
        public delegate void OnSelectedRangeChangeEventHandler(object range);

        public event OnSetTargetGroupEventHandler OnSetTargetGroup;
        public delegate void OnSetTargetGroupEventHandler(string groupName);



        #endregion
              
        #region Constructor
        public AutomaticSourcingSheetPanel()
        {
            InitializeComponent();
            InitializeContextMenu();
            InitializeHandlers();
        }

        #endregion

     
        #region Public Methods
    

        /// <summary>
        /// Display current AutomaticSourcingSheet elements to Ui.
        /// </summary>
        /// <param name="automaticSourcingSheet"></param>
        public void Display(AutomaticSourcingSheet automaticSourcingSheet)
        {
            throwChange = false;
            this.AutomaticSourcingSheet = automaticSourcingSheet != null ? automaticSourcingSheet : this.AutomaticSourcingSheet;
            nameSheetTextBox.Text = this.AutomaticSourcingSheet != null ? this.AutomaticSourcingSheet.Name : "";

                FirstRowNameCheckBox.IsChecked = automaticSourcingSheet != null ? this.AutomaticSourcingSheet.firstRowColumn: false;
                RangeTextBox.Text = this.AutomaticSourcingSheet != null && this.AutomaticSourcingSheet.rangeSelected != null && RangeCheckBox.IsChecked.HasValue ?
                    this.AutomaticSourcingSheet.rangeSelected.Name : "" ;

                List<AutomaticSourcingColumn> listeToSend = automaticSourcingSheet != null && this.AutomaticSourcingSheet.listColumnToDisplay != null ? this.AutomaticSourcingSheet.listColumnToDisplay : null;
                FillListColumns(listeToSend);

                if (this.AutomaticSourcingSheet != null && this.AutomaticSourcingSheet.ActiveColumn == null)
                  if (listeToSend != null && listeToSend.Count > 0) this.AutomaticSourcingSheet.ActiveColumn = this.AutomaticSourcingSheet.getFirstInList();

                ColumnPanel.Display(this.AutomaticSourcingSheet != null ? this.AutomaticSourcingSheet.ActiveColumn : null, listeToSend);
            
            throwChange = true;
        }


        public void DisplayColumn(AutomaticSourcingColumn col, List<AutomaticSourcingColumn> liste) 
        {
            this.ColumnPanel.Display(col,liste);
        }

        public void DisplayMeasure() 
        {
            this.ColumnPanel.DisplayMeasure();
        }

        public void DisplayScope()
        {
            this.ColumnPanel.DisplayScope();
        }

        public void DisplayPeriod()
        {
            this.ColumnPanel.DisplayPeriod();
        }

        public void DisplayTag()
        {
            this.ColumnPanel.DisplayTag();
        }

        public List<int> getListBoxItems()
        {
            List<int> items = new List<int>(0);
            foreach (object objet in this.ColumnListBox.ItemsSource) 
            {
                items.Add(((AutomaticSourcingColumn)objet).columnIndex);
            }
            return items;
        }
   
        public void SetSelectedIndex(int index) 
            
        {
            if (index > -1 && this.ColumnListBox.Items.Count > 0) this.ColumnListBox.SelectedItem = this.ColumnListBox.Items.GetItemAt(index);
        }

        public int getColumnInListBox(int colunmIndex) 
        {
            List<AutomaticSourcingColumn> liste = (List<AutomaticSourcingColumn>)this.ColumnListBox.ItemsSource;
            if (liste == null) return -1;
            if (liste.Count == 0) return -1;

            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);

                if (((AutomaticSourcingColumn)liste[mil]).columnIndex == colunmIndex)
                {
                    found = true;
                    return mil;
                }
                else
                {
                    if (((AutomaticSourcingColumn)liste[mil]).columnIndex > colunmIndex)
                    {
                        fin = mil - 1;

                    }
                    else
                    {
                        debut = mil + 1;

                    }
                }
            }
            while (!found && debut <= fin);
            return -1;
        }

        public CellPropertyAllocationData GetAllocationData()
        {
            return this.ColumnPanel.GetAllocationData();
        }

        /// <summary>
        /// Set Measure to the current AutomaticSourcingSheet
        /// </summary>
        /// <param name="measure"></param>
        public void SetSelectedMeasure(Kernel.Domain.Measure measure)
        {
            this.ColumnPanel.SetMeasure(measure);
        }

        /// <summary>
        /// Set Attribute to the current AutomaticSourcingSheet
        /// </summary>
        /// <param name="attribute"></param>
        public void SetSelectedAttribute(Kernel.Domain.Attribute attribute)
        {
            this.ColumnPanel.SetAttribute(attribute);
        }

        public void SetSelecteColumn(int columnIndex) 
        {
            this.ColumnListBox.SelectedIndex = columnIndex;
        }

        public bool GetSelectionRangeState() 
        {
            return this.RangeCheckBox.IsChecked.Value;
        }

        public bool GetFirstRowSelectionState() 
        {
            return this.FirstRowNameCheckBox.IsChecked.Value;
        }

        public AutomaticSourcingColumn GetSelectedColumn() 
        {
            return this.ColumnListBox.SelectedItem != null ? this.ColumnListBox.SelectedItem as AutomaticSourcingColumn : null;
        }

        public int GetSelectedListIndex() 
        {
            return this.ColumnListBox.SelectedIndex;
        }
    
        public AutomaticSourcingSheet GetSelectedSheet()
        {
            return this.AutomaticSourcingSheet;
        }

        public void DisplayAllocationData(CellPropertyAllocationData CellPropertyAllocationData) 
        {
            this.ColumnPanel.DisplayAllocationData(CellPropertyAllocationData);
        }

        
        #endregion

        #region Handlers

        private void InitializeHandlers()
        {
            this.ColumnListBox.KeyUp +=ColumnListBox_KeyUp;
            this.PreviewMouseRightButtonDown +=OnPreviewMouseRightButtonDown;
            this.FirstRowNameCheckBox.Checked += OnSetFirstRowAsHeaderChange;
            this.FirstRowNameCheckBox.Unchecked += OnSetFirstRowAsHeaderChange;
           
            this.RangeCheckBox.Checked += OnSelectRangeOptionChange;
            this.RangeCheckBox.Unchecked += OnSelectRangeOptionChange;
            this.RangeTextBox.KeyUp +=RangeTextBox_KeyUp;
            this.ColumnListBox.SelectionChanged +=ColumnListBox_SelectionChanged;
         
            this.ColumnPanel.OnNewColumn +=AutomaticSourcingColumnPanel_OnNewColumn;

            this.ColumnPanel.OnRemoveColumn +=AutomaticSourcingColumnPanel_OnRemoveColumn;
            this.ColumnPanel.OnSelectColumnItem +=AutomaticSourcingColumnPanel_OnSelectColumnItem;
            this.ColumnPanel.OnSelectTarget += AutomaticSourcingColumnPanel_OnSelectTarget;
            this.ColumnPanel.OnTypeChange +=AutomaticSourcingColumnPanel_OnTypeChange;
            this.ColumnPanel.OnSetTagName +=AutomaticSourcingColumnPanel_OnSetTagName;
            this.ColumnPanel.OnSetPeriod +=AutomaticSourcingColumnPanel_OnSetPeriod;
            this.ColumnPanel.OnRemoveColumnItem +=AutomaticSourcingColumnPanel_OnRemoveColumnItem;
            this.ColumnPanel.OnChooseTargetType +=AutomaticSourcingColumnPanel_OnChooseTargetType;
            this.ColumnPanel.OnAllocationPanelChange +=AutomaticSourcingColumnPanel_OnAllocationPanelChange;
            this.ColumnPanel.OnSetTargetGroup += ColumnPanel_OnSetTargetGroup;

            this.ColumnPanel.Changed += OnChanged;

            Style TextBoxStyle = new Style(typeof(TextBox));
            TextBoxStyle.Setters.Add(new EventSetter(UIElement.GotFocusEvent,new RoutedEventHandler(RangeTextBox_GotFocus)));
            RangeTextBox.Style = TextBoxStyle;

        }

        private void ColumnPanel_OnSetTargetGroup(string groupName)
        {
            if (OnSetTargetGroup != null) OnSetTargetGroup(groupName);
        }

        private void RangeTextBox_KeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter) 
            {
                if(OnSelectedRangeChange != null)
                {
                    Kernel.Ui.Office.Range range = new Kernel.Ui.Office.Range();
                    AutomaticSourcingSheet sourcingSheet = new Kernel.Domain.AutomaticSourcingSheet();
                    range = sourcingSheet.buildRange(RangeTextBox.Text.TrimEnd());
                    if (range != null && range.CellCount > 0)
                    {
                        OnSelectedRangeChange(range);
                    }
                }
            }
        }

     
        private void AutomaticSourcingColumnPanel_OnAllocationPanelChange()
        {
            if (OnAllocationPanelChange != null)
                OnAllocationPanelChange();
        }

        private void ColumnListBox_KeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Delete)
            {
                if (OnRemoveColumn != null)
                {
                    if(ColumnListBox.SelectedItem != null)
                        OnRemoveColumn(ColumnListBox.SelectedItem as AutomaticSourcingColumn, ColumnListBox.SelectedIndex);
                }
            }
        }


        protected virtual void InitializeContextMenu()
        {
            BuildColumnContextMenu();
            this.ColumnListBox.ContextMenu = this.columnContextMenu;
            this.removeColumnMenuItem.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Delete.png", UriKind.Relative)) };
            this.removeColumnMenuItem.Click += new RoutedEventHandler(OnRemoveColumnInList);
        }

        private void OnRemoveColumnInList(object sender, RoutedEventArgs e)
        {
            if (OnRemoveColumn != null)
                OnRemoveColumn(activeColumn, ColumnListBox.SelectedIndex);
        }

        private static object VisualUpwardSearch(DependencyObject source)
        {
            List<object> listToReturn = new List<object>(0);
            while (source != null && !(source is ListBoxItem))
                source = VisualTreeHelper.GetParent(source);

            if (source is ListBoxItem)
            {
                ListBoxItem item = source as ListBoxItem;
                ListBox view = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
                int index = view.ItemContainerGenerator.IndexFromContainer(item);

                listToReturn.Add(item.Content);
                listToReturn.Add(index);

                return listToReturn;
            }
            return null;
        }

        private void BuildColumnContextMenu()
        {
            this.columnContextMenu = new ContextMenu();
            removeColumnMenuItem = new MenuItem();
            removeColumnMenuItem.Header = "Remove";
            this.columnContextMenu.Items.Add(removeColumnMenuItem);
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            object resultColumn = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (resultColumn != null && resultColumn is IList)
            {
                var liste = resultColumn as IList;
                if (liste.Count == 2)
                { 
                  activeColumn = liste[0] as  AutomaticSourcingColumn;
                  currentIndex =int.Parse(liste[1].ToString());
                }
            }
        }
          
        private void AutomaticSourcingColumnPanel_OnChooseTargetType(object parameters)
        {
            if (OnChooseTargetType != null)
                OnChooseTargetType(parameters);
        }

        private void AutomaticSourcingColumnPanel_OnRemoveColumnItem(object item)
        {
            if (OnRemoveColumnItem != null)
                OnRemoveColumnItem(item);
        }

        private void AutomaticSourcingColumnPanel_OnChooseTarget(object parameters)
        {
            if (OnChooseTarget != null)
                OnChooseTarget(parameters);
        }

        private void AutomaticSourcingColumnPanel_OnSetPeriod(object parameters,bool isDateFormat)
        {
            if (OnSetPeriod != null) OnSetPeriod(parameters,isDateFormat);           

        }

        private void AutomaticSourcingColumnPanel_OnSetTagName(string tagName)
        {
            if (OnSetTagName != null) OnSetTagName(tagName);
        }

        private void AutomaticSourcingColumnPanel_OnTypeChange(Kernel.Application.ParameterType typeValue)
        {
            if (OnTypeChange != null)
                OnTypeChange(typeValue);

        }

        private void AutomaticSourcingColumnPanel_OnSelectColumnItem(object item)
        {
            if (OnSelectColumnItem != null) OnSelectColumnItem(item);
        }

        private void AutomaticSourcingColumnPanel_OnSelectTarget(object item, object operatorValue)
        {
            if (OnSelectTarget != null) OnSelectTarget(item, operatorValue);
        }

        private void AutomaticSourcingColumnPanel_OnRemoveColumn(object item)
        {
            if (OnRemoveColumn != null)
                OnRemoveColumn(item,ColumnListBox.SelectedIndex);
        }

        private void AutomaticSourcingColumnPanel_OnNewColumn()
        {
            if (OnNewColumn != null)
                OnNewColumn();
        }

        private void RangeTextBox_GotFocus(object sender, RoutedEventArgs e) 
        {
            if (FocusTextBox != null)
            {
                FocusTextBox(RangeTextBox.IsFocused);
               
            }
        }
           
        private void ColumnListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColumnListBox.SelectedItem is AutomaticSourcingColumn)
            {
                activeColumn = ColumnListBox.SelectedItem as AutomaticSourcingColumn;
                if (SelectColumn != null && ColumnListBox.SelectedIndex >=0) SelectColumn(activeColumn, ColumnListBox.SelectedIndex);
                if (this.AutomaticSourcingSheet == null) this.AutomaticSourcingSheet = activeColumn.parent;
                List<object> listeToSend = this.AutomaticSourcingSheet.listColumnToDisplay.ToList<object>();
            }
        }

        private void OnSelectRangeOptionChange(object sender, RoutedEventArgs e)
        {
            if(SelectRangeOption != null)
            {
                var state = ((CheckBox)sender).IsChecked.Value;
                SelectRangeOption(state);
                if (state)
                {
                 
                    RangeTextBox.Focus();
                }
            }
        }

        private void OnSetFirstRowAsHeaderChange(object sender, RoutedEventArgs e)
        {
            if (SetFirstRowAsHeader != null) 
            {
                var state = ((CheckBox)sender).IsChecked.Value;
                SetFirstRowAsHeader(state);
            }
        }

        private void OnChanged()
        {
            if (Changed != null && throwChange) Changed();
        }
        #endregion

        #region Utils

        public void FillListColumns(List<AutomaticSourcingColumn> items)
        {
            if (items is IList)
            {
                ColumnListBox.ItemsSource = null;
                ColumnListBox.ItemsSource = items;
            }
        }
       
        #endregion




        public void SetPeriod(PeriodName periodName)
        {
            this.ColumnPanel.setPeriodName(periodName);
        }

        public void customizeForTarget()
        {
            this.ColumnPanel.customizeForTarget();
        }

        public  string getTargetGroupName()
        {
            return this.ColumnPanel.getGroupTargetName();
        }
    }
}
