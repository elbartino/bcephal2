using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.Base;
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
    /// Interaction logic for AutomaticSourcingColumnPanel.xaml
    /// </summary>
    public partial class AutomaticSourcingColumnPanel : UserControl
    {

        #region events
        public event OnNewColumnEventHandler OnNewColumn;
        public delegate void OnNewColumnEventHandler();

        public event OnRemoveColumnEventHandler OnRemoveColumn;
        public delegate void OnRemoveColumnEventHandler(object item);

        public event OnSelectionChangeEventHandler OnSelectColumnItem;
        public delegate void OnSelectionChangeEventHandler(object item);

        public event OnSelectionTargetChangeEventHandler OnSelectTarget;
        public delegate void OnSelectionTargetChangeEventHandler(object item, object operatorValue);

        public event OnTypeChangeEventHandler OnTypeChange;
        public delegate void OnTypeChangeEventHandler(ParameterType typeValue);

        public event OnSetPeriodEventHandler OnSetPeriod;
        public delegate void OnSetPeriodEventHandler(object parameters,bool isDateFormat=false);

        public event OnChooseTargetTypeEventHandler OnChooseTargetType;
        public delegate void OnChooseTargetTypeEventHandler(object parameters);

        public event OnRemoveColumnItemEventHandler OnRemoveColumnItem;
        public delegate void OnRemoveColumnItemEventHandler(object item);

        public event OnSetTagNameEventHandler OnSetTagName;
        public delegate void OnSetTagNameEventHandler(string tagName);

        public event OnAllocationPanelChangeEventHandler OnAllocationPanelChange;
        public delegate void OnAllocationPanelChangeEventHandler();

        public event OnSetTargetGroupEventHandler OnSetTargetGroup;
        public delegate void OnSetTargetGroupEventHandler(string groupName);

        public event ChangeEventHandler Changed;
        #endregion

        #region Properties
      
        public AutomaticSourcingColumn AutomaticSourcingColumn { get; private set; }
        ColumnTargetItem columnTargetItem { get; set; }
        List<AutomaticSourcingColumn> ListeAutomaticSourcingColumn { get; set; }
        
        List<object> periodTerms { get; set; }
        List<string> listeDateType;

        Dictionary<TargetType, string> targetList;  
      
        string[] formatToDisplay = 
        {
            "",
            "MM-dd-yy",
            "MMMM dd, yyyy",
            "MM/dd/yy",
            "yyyy-MM-dd",
            "dddd, MMMM dd yyyy",
            "M/yy",
            "dd-MM-yy",
        };
        public List<string> DataFormatType
        {
            get 
            {
                listeDateType = new List<string>(0);
                listeDateType.Add("");
                listeDateType.Add("jj-MM-AA");
                listeDateType.Add("jj-MM-AAAA");
                listeDateType.Add("jj-AA-MM");
                listeDateType.Add("AA-MM-jj");
                listeDateType.Add("AAAA-MM-jj");
                listeDateType.Add("AA-jj-MM");
                listeDateType.Add("jj-AAAA-MM");
                listeDateType.Add("MM-AA");
                listeDateType.Add("MM-AAAA");
                listeDateType.Add("MM-AAAA-jj");
                listeDateType.Add("MM-AA-jj");
                listeDateType.Add("MM-jj-AA");
                listeDateType.Add("MM-jj-AAAA");
                listeDateType.Add("jj/MM/AA");
                listeDateType.Add("jj/MM/AAAA");
                listeDateType.Add("jj/AA/MM");
                listeDateType.Add("jj/AAAA/MM");
                listeDateType.Add("MM/AA");
                listeDateType.Add("MM/AAAA");
                return this.listeDateType;
            }
        }

        List<object> localList;
        List<object> filterList
        {
            get 
            {
                this.localList = new List<object>(0);
                foreach (object element in this.ListeAutomaticSourcingColumn)
                {
                    var ascolumn = element as AutomaticSourcingColumn;
                    if (ascolumn != this.AutomaticSourcingColumn && (ascolumn.parameterType == ParameterType.TARGET) || (ascolumn.parameterType == ParameterType.SCOPE && ascolumn.attribute != null))
                    {
                        this.localList.Add(ascolumn);
                    }
                    ColumnTargetItem targetItem = this.AutomaticSourcingColumn.getColumnTargetItem(ascolumn.columnIndex);
                    if (targetItem != null && ascolumn.attribute == null)
                        this.AutomaticSourcingColumn.RemoveColumnTargetItem(targetItem);
                }
                return this.localList.Distinct().ToList();
            }
            set 
            {
                this.localList = value;
            } 
        }
     
        private bool throwChange = true;
        
        #endregion

        #region Constructor
    
        public AutomaticSourcingColumnPanel()
        {
            InitializeComponent();
            InitializeData();
            InitializeHandlers();
        }

        #endregion
    
        #region Handlers

        private void InitializeHandlers()
        {
            TypeComboBox.SelectionChanged += OnTypeChanged;
            TargetTypeComboBox.SelectionChanged += OnTargetTypeChanged;
            this.PeriodNameTextBox.KeyUp += PeriodNameTextBox_KeyUp;
            this.newTargetElement.OnNewColumn += newTargetElement_OnNewColumn;
            this.newTargetElement.OnRemoveColumn += newTargetElement_OnRemoveColumn;
            this.newTargetElement.OnSelectColumnItem += newTargetElement_OnSelectColumnItem;
            this.newTargetElement.OnSelectTarget +=newTargetElement_OnSelectTarget;
            this.TagNameTextBox.KeyUp +=TagNameTextBox_KeyUp;
            this.newTargetElement.OnRemoveColumnItem += newTargetElement_OnRemoveColumnItem;
            this.FormatComboBox.SelectionChanged +=OnFormatDateChanged;
            this.newTargetElement.OnSetTargetGroup += OnSetTargetGroupHandler;
            this.allocationPanel.Change += new ChangeEventHandler(OnChangePanel);

            this.DefaultValuePanel.Changed += OnDefaultValueChanged;
            this.ExcludedValuePanel.Changed += OnChanged;
        }
        
        private void OnDefaultValueChanged()
        {
            this.AutomaticSourcingColumn.defaultValue = this.DefaultValuePanel.Item;
            OnChanged();
        }

        private void OnSetTargetGroupHandler(string groupName)
        {
            if (OnSetTargetGroup != null) OnSetTargetGroup(groupName);
        }

        private void OnChangePanel()
        {
            if (OnAllocationPanelChange != null)
                OnAllocationPanelChange();
        }

        private void TagNameTextBox_KeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                TagNameTextBox.Text = "";
            }
            else if (args.Key == Key.Enter)
            {
                if (this.OnSetTagName != null)
                {
                    if (TagNameTextBox.Text.Length > 0 && TypeComboBox.SelectedItem.ToString() == ParameterType.TAG.ToString())
                    {
                        string tagName = TagNameTextBox.Text;
                        OnSetTagName(tagName);
                    }
                }
            }
        }

        private void PeriodNameTextBox_KeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                PeriodNameTextBox.Text = "";
            }
            else if (args.Key == Key.Enter)
            {
                if (this.OnSetPeriod != null)
                {
                    if (PeriodNameTextBox.Text.Length > 0 && TypeComboBox.SelectedItem.ToString() == ParameterType.PERIOD.ToString())
                    {
                        string periodName = PeriodNameTextBox.Text;
                        OnSetPeriod(periodName);
                    }
                }
            }
        }

        private void OnFormatDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnSetPeriod != null)
            OnSetPeriod(FormatComboBox.SelectedItem != null ? FormatComboBox.SelectedItem.ToString() : "",true);
        }
                    
        public void OnTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            HideControls();
            if (sender is Kernel.Application.ParameterType)
                TypeComboBox.SelectedItem = sender.ToString();

            string columnType = TypeComboBox.SelectedItem.ToString();
            if (columnType != null && columnType != "")
            {
                if (OnTypeChange != null)
                {
                    if (columnType == Kernel.Application.ParameterType.MEASURE.ToString())
                    {
                        DisplayMeasureControls();
                    }
                    else if (columnType == Kernel.Application.ParameterType.PERIOD.ToString())
                    {
                        DisplayPeriodControls();
                    }
                    else if (columnType == Kernel.Application.ParameterType.SCOPE.ToString())
                    {
                        DisplayScopeControls();
                        
                    }
                    else if (columnType == Kernel.Application.ParameterType.TAG.ToString())
                    {
                        DisplayTagControls();
                    }
                    else if (columnType == Kernel.Application.ParameterType.TARGET.ToString())
                    {
                        InitializeTargetItems();
                        DisplayTargetControls();
                    }
                }
            }
            else
            {
                columnType = Kernel.Application.ParameterType.NULL.ToString();
                HideControls();
                OnTypeChange(Kernel.Application.ParameterType.NULL);
             
            }
       
        }

        private void OnTargetTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            string TargetType = TargetTypeComboBox.SelectedItem.ToString();

            string targetTypeSelected = targetList.Keys.ElementAt(TargetTypeComboBox.SelectedIndex).ToString();
            object targetParams = getTargetType(targetTypeSelected);

            if (targetParams != null)
            {
                if (OnChooseTargetType != null)
                {
                    OnChooseTargetType(targetParams);
                    HideControls(NewTargetGrid, (TargetType)targetParams != Kernel.Application.TargetType.CREATE);
                    DisplayTargetColumnItems(this.AutomaticSourcingColumn, this.filterList);
                }
            }
       }

        private void OnChanged()
        {
            if (this.AutomaticSourcingColumn.parent != null)
            {
                this.AutomaticSourcingColumn.parent.UpdateColumn(this.AutomaticSourcingColumn);
                if (this.AutomaticSourcingColumn.parent.parent != null)
                {
                    this.AutomaticSourcingColumn.parent.parent.UpdateSheet(this.AutomaticSourcingColumn.parent);
                }
            }
            if (Changed != null && throwChange) Changed();
        }

        private void newTargetElement_OnRemoveColumnItem(object item)
        {
            if (OnRemoveColumnItem != null)
                OnRemoveColumnItem(item);
        }

        private void TagNameTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.OnSetTagName != null)
            {
                if (TagNameTextBox.Text.Length > 0 && TypeComboBox.SelectedItem.ToString() == ParameterType.TAG.ToString())
                {
                    string tagName = TagNameTextBox.Text;
                    OnSetTagName(tagName);
                }
            }
        }

        private void newTargetElement_OnSelectColumnItem(object item)
        {
            if (OnSelectColumnItem != null) OnSelectColumnItem(item);
        }

        private void newTargetElement_OnSelectTarget(object item, object operatorValue)
        {
            if (OnSelectTarget != null) OnSelectTarget(item, operatorValue);
        }

        private void newTargetElement_OnRemoveColumn(object item)
        {
            if (OnRemoveColumn != null)
                OnRemoveColumn(item);
        }

        private void newTargetElement_OnNewColumn()
        {
            if (OnNewColumn != null)
                OnNewColumn();
        }

        #endregion

        #region Display Views
        
        /// <summary>
        /// Display default measure Ui.
        /// </summary>
        private void DisplayMeasureControls() 
        {
            TypeComboBox.SelectedItem = Kernel.Application.ParameterType.MEASURE.ToString();
            OnTypeChange(Kernel.Application.ParameterType.MEASURE);
            HideControls(MeasureGrid, false);          
        }

        /// <summary>
        /// Display default period Ui.
        /// </summary>
        private void DisplayPeriodControls() 
        {
            TypeComboBox.SelectedItem = Kernel.Application.ParameterType.PERIOD.ToString();
            HideControls(PeriodGrid, false);
            this.FormatComboBox.ItemsSource = this.formatToDisplay;
            this.FormatComboBox.SelectionChanged -= OnFormatDateChanged;
            this.FormatComboBox.SelectedItem = "";
            OnTypeChange(Kernel.Application.ParameterType.PERIOD);
            this.FormatComboBox.SelectionChanged += OnFormatDateChanged;
        }
      
        /// <summary>
        /// Display default Tag Ui.
        /// </summary>
        private void DisplayTagControls() 
        {
            TypeComboBox.SelectedItem = Kernel.Application.ParameterType.TAG.ToString();
            HideControls(TagGrid, false);
            OnTypeChange(Kernel.Application.ParameterType.TAG);
        }

        /// <summary>
        /// Display default scope Ui.
        /// </summary>
        private void DisplayScopeControls() 
        {
            TypeComboBox.SelectedItem = Kernel.Application.ParameterType.SCOPE.ToString();
            HideControls(ScopeGrid, false);          
            OnTypeChange(Kernel.Application.ParameterType.SCOPE);
        }

        /// <summary>
        /// Display default target Ui.
        /// </summary>
        private void DisplayTargetControls() 
        {
            TypeComboBox.SelectedItem = Kernel.Application.ParameterType.TARGET.ToString();
            InitializeTarget(this.AutomaticSourcingColumn.targetType);
            this.newTargetElement.GroupTextBox.Text = this.AutomaticSourcingColumn.targetGroup != null ? this.AutomaticSourcingColumn.targetGroup.name :  this.AutomaticSourcingColumn.Name;
            OnTypeChange(Kernel.Application.ParameterType.TARGET);
            string targetTypeSelected = targetList.Keys.ElementAt(TargetTypeComboBox.SelectedIndex).ToString();
            object targetParams = getTargetType(targetTypeSelected);
            if (OnChooseTargetType != null)
            {
                HideControls(NewTargetGrid, (TargetType)targetParams != Kernel.Application.TargetType.CREATE);
                
                DisplayTargetColumnItems(this.AutomaticSourcingColumn, this.filterList);
                OnChooseTargetType(targetParams);
            }
        }
      
        #endregion
       
        #region Initialize Data

        private void InitializeData()
        {
            InitializeParameterType();
        }

        private void InitializeTargetItems() 
        {
            targetList = new Dictionary<TargetType, string>(0);
            targetList.Add(Kernel.Application.TargetType.CREATE, "Create new Scope");
            targetList.Add(Kernel.Application.TargetType.USE_AS_SCOPE, "Use target as Scope");
            this.TargetTypeComboBox.ItemsSource = targetList.Values;
            this.TargetTypeComboBox.SelectedItem = "Create new Scope";            
        }
        
        private void InitializeParameterType()
        {
            HideControls();
            
            this.TypeComboBox.ItemsSource = new string[] { "",
            Kernel.Application.ParameterType.MEASURE.ToString(), 
            Kernel.Application.ParameterType.PERIOD.ToString(),
            Kernel.Application.ParameterType.TAG.ToString(), 
            Kernel.Application.ParameterType.SCOPE.ToString() 
            };
            this.TypeComboBox.SelectedItem = "";
        }

        private void InitializeParameterTypeForAutomaticTarget()
        {
            HideControls();
            this.TypeComboBox.ItemsSource = new string[] { "",
            Kernel.Application.ParameterType.SCOPE.ToString(), 
            Kernel.Application.ParameterType.TARGET.ToString(), 
            };
            this.TypeComboBox.SelectedItem = "";
        }

        public void InitializeTarget(TargetType targetType)
        {
            string selectedItem;
            targetList.TryGetValue(targetType, out selectedItem);
            this.TargetTypeComboBox.SelectedItem = selectedItem;
        }
       
        #endregion

        #region Public Methods

        public void DisplayMeasure() 
        {
            this.OnTypeChanged(Kernel.Application.ParameterType.MEASURE,null);
        }

        public void DisplayScope()
        {
            this.OnTypeChanged(Kernel.Application.ParameterType.SCOPE, null);
        }

        public void DisplayPeriod()
        {
            this.OnTypeChanged(Kernel.Application.ParameterType.PERIOD, null);
        }

        public void DisplayTag()
        {
            this.OnTypeChanged(Kernel.Application.ParameterType.TAG, null);
        }


        /// <summary>
        /// Set Measure to the current AutomaticSourcingColumn
        /// </summary>
        /// <param name="measure"></param>
        public void SetMeasure(Measure measure)
        {
            this.MeasureTextBox.Text = measure.name;
        }

        /// <summary>
        /// Set Attribute to the current AutomaticSourcingColumn
        /// </summary>
        /// <param name="attribute"></param>
        public void SetAttribute(Kernel.Domain.Attribute attribute)
        {
            AttributeTextBox.Text = attribute.name;
        }
            
        /// <summary>
        /// Display properties of the current AutomaticSourcingColumn
        /// in Ui.
        /// </summary>
        /// <param name="automaticSourcingColumn"></param>
        public void Display(AutomaticSourcingColumn automaticSourcingColumn, List<AutomaticSourcingColumn> liste)
        {
            throwChange = false;
            this.AutomaticSourcingColumn = automaticSourcingColumn;
            this.ListeAutomaticSourcingColumn = liste;
            HideControls();

            if (this.AutomaticSourcingColumn != null)
            {
                this.DefaultValuePanel.Display(this.AutomaticSourcingColumn.defaultValue);
                this.ExcludedValuePanel.Display(this.AutomaticSourcingColumn);
                string columnType = TypeComboBox.SelectedItem.ToString();

                if (columnType != "" || this.AutomaticSourcingColumn.parameterType.ToString() != "")
                {
                    //HideControls();
                    if (this.AutomaticSourcingColumn.parameterType.ToString() != "")
                    {
                        if (this.AutomaticSourcingColumn.parameterType.ToString() == Kernel.Application.ParameterType.MEASURE.ToString())
                        {
                            DisplayMeasure(this.AutomaticSourcingColumn);
                        }
                        else if (this.AutomaticSourcingColumn.parameterType.ToString() == Kernel.Application.ParameterType.PERIOD.ToString())
                        {
                            DisplayPeriod(this.AutomaticSourcingColumn);
                        }
                        else if (this.AutomaticSourcingColumn.parameterType.ToString() == Kernel.Application.ParameterType.TARGET.ToString())
                        {
                            DisplayTarget(this.AutomaticSourcingColumn);
                        }
                        else if (this.AutomaticSourcingColumn.parameterType.ToString() == Kernel.Application.ParameterType.SCOPE.ToString())
                        {
                            DisplayScope(this.AutomaticSourcingColumn);
                        }
                        else if (this.AutomaticSourcingColumn.parameterType.ToString() == Kernel.Application.ParameterType.TAG.ToString())
                        {
                            DisplayTagName(this.AutomaticSourcingColumn);
                        }
                    }
                }
            }
            throwChange = true;
        }
        
        public CellPropertyAllocationData GetAllocationData()
        {
            return this.allocationPanel.AllocationData;
        }

        public void DisplayAllocationData(CellPropertyAllocationData CellPropertyAllocationData)
        {
            this.allocationPanel.DisplayAllocationData(CellPropertyAllocationData);
        }

        #endregion
     
        #region Display Datas

        /// <summary>
        /// Display period of the current AutomaticSourcingColumn
        /// </summary>
        /// <param name="automaticSourcingColumn"></param>
        private void DisplayPeriod(AutomaticSourcingColumn automaticSourcingColumn)
        {
            HideControls(PeriodGrid, false);
            TypeComboBox.SelectedItem = Kernel.Application.ParameterType.PERIOD.ToString();
            PeriodNameTextBox.Text = automaticSourcingColumn.periodName;
            FormatComboBox.SelectionChanged -= OnFormatDateChanged;
            FormatComboBox.SelectedItem = automaticSourcingColumn.dateFormat;
        }

        /// <summary>
        /// Display measure of the current AutomaticSourcingColumn
        /// </summary>
        /// <param name="automaticSourcingColumn"></param>
        private void DisplayMeasure(AutomaticSourcingColumn automaticSourcingColumn)
        {
            HideControls(MeasureGrid, false);
            TypeComboBox.SelectedItem = Kernel.Application.ParameterType.MEASURE.ToString();
            if (automaticSourcingColumn.measure != null)
            {
                MeasureTextBox.Text = automaticSourcingColumn.measure.name;
                allocationPanel.DisplayAllocationData(automaticSourcingColumn.allocationData);
                automaticSourcingColumn.parameterType = ParameterType.MEASURE;
            }
            else
            {
                TypeComboBox.SelectedItem = "";
            }
        }

        /// <summary>
        /// Display scope of the current AutomaticSourcingColumn
        /// </summary>
        /// <param name="automaticSourcingColumn"></param>
        private void DisplayScope(AutomaticSourcingColumn automaticSourcingColumn)
        {
            if (this.AutomaticSourcingColumn.attribute != null)
            {
                HideControls(ScopeGrid, false);
                TypeComboBox.SelectedItem = Kernel.Application.ParameterType.SCOPE.ToString();
                AttributeTextBox.Text = automaticSourcingColumn.attribute.name;
                automaticSourcingColumn.parameterType = ParameterType.SCOPE;
            }
            else
            {
                TypeComboBox.SelectedItem = "";
            }
        }

        /// <summary>
        /// Display TagName of the current AutomaticSourcingColumn
        /// </summary>
        /// <param name="automaticSourcingColumn"></param>
        private void DisplayTagName(AutomaticSourcingColumn automaticSourcingColumn)
        {
            if (automaticSourcingColumn.tagName != null)
            {
                HideControls(TagGrid, false);
                TypeComboBox.SelectedItem = Kernel.Application.ParameterType.TAG.ToString();
                TagNameTextBox.Text = this.AutomaticSourcingColumn.tagName;
                automaticSourcingColumn.parameterType = ParameterType.TAG;
            }
            else
            {
                TypeComboBox.SelectedItem = "";
            }
        }
        
        #endregion

        #region Target Methods

        /// <summary>
        /// Display target presentation when the type is Target
        /// </summary>
        /// <param name="automaticSourcingColumn"></param>
        private void DisplayTarget(AutomaticSourcingColumn automaticSourcingColumn)
        {
            HideControls(TargetGrid, false);
            TypeComboBox.SelectedItem = Kernel.Application.ParameterType.TARGET.ToString();

            if (automaticSourcingColumn.targetType != null)
            {
                if (automaticSourcingColumn.targetType == Kernel.Application.TargetType.CREATE)
                {
                    TargetTypeComboBox.SelectedItem = Kernel.Application.TargetType.CREATE.ToString();
                    AddColumnsTarget(automaticSourcingColumn);
                }
                else if (automaticSourcingColumn.targetType == Kernel.Application.TargetType.USE_AS_SCOPE)
                {
                    TargetTypeComboBox.SelectedItem = Kernel.Application.TargetType.USE_AS_SCOPE.ToString();
                }
                else
                {
                    TargetTypeComboBox.SelectedItem = "";
                }
            }
        }

        /// <summary>
        /// Display the columnTargetItems of the current target.
        /// </summary>
        /// <param name="automaticSourcingColumn"></param>
        private void DisplayTargetColumnItems(AutomaticSourcingColumn automaticSourcingColumn,List<object> liste)
        {
            if(automaticSourcingColumn.columnTargetItemListChangeHandler == null)
            {
                automaticSourcingColumn.columnTargetItemListChangeHandler = new PersistentListChangeHandler<ColumnTargetItem>();
            }

            if (liste == null) liste = new List<object>(0);
            newTargetElement.columnsItemsPanel.DisplayAutomaticSourcingColumn(liste, automaticSourcingColumn);
        }

        /// <summary>
        /// Gets the ParameterType which the given string value.
        /// </summary>
        /// <param name="selectedOption">the given string value</param>
        /// <returns>an element in TargetType enum</returns>
        private object getTargetType(string selectedOption)
        {
            if (selectedOption == "") return null;
            object targetTypeParams = null;
            if (selectedOption == Kernel.Application.TargetType.CREATE.ToString())
            {
                targetTypeParams = Kernel.Application.TargetType.CREATE;
            }
            else if (selectedOption == Kernel.Application.TargetType.USE_AS_SCOPE.ToString())
            {
                targetTypeParams = Kernel.Application.TargetType.USE_AS_SCOPE;
            }
            return targetTypeParams;
        }

        /// <summary>
        /// Add the visual component that represent a ColumnTargetItem.
        /// </summary>
        /// <param name="automaticSourcingColumn"></param>
        private void AddColumnsTarget(AutomaticSourcingColumn automaticSourcingColumn)
        {
            throwChange = false;

            if (automaticSourcingColumn.targetType == TargetType.CREATE)
            {
                if (automaticSourcingColumn.columnTargetItemListChangeHandler != null)
                {
                    if (filterList == null) filterList = new List<object>(0);
                    this.newTargetElement.columnsItemsPanel.AddItemPanel(automaticSourcingColumn.columnTargetItemListChangeHandler, filterList.Distinct().ToList());
                }
                else
                {
                    HideControls(NewTargetGrid, false);
                }
            }
            throwChange = true;
        }
     
        #endregion
        
        #region Utils
       
        /// <summary>
        /// Hide/Show all Uicontrols
        /// </summary>
        /// <param name="hide"></param>
        public void HideControls(bool hide = true)
        {
            HideControls(NewTargetGrid, hide);
            MeasureTextBox.Text = "";
           // allocationPanel.OutputMeasureTextBox.Text = "";
            allocationPanel.RefMeasureTextBox.Text = "";
            AttributeTextBox.Text = "";
            AttributeTextBox.IsEnabled = false;
            HideControls(ScopeGrid, hide);
            HideControls(TargetGrid, hide);
            HideControls(TagGrid, hide);
            TagNameTextBox.Text = "";
            HideControls(PeriodGrid, hide);
            HideControls(MeasureGrid, hide);
        }

        /// <summary>
        /// Hide/Show a specific Uicontrols
        /// </summary>
        /// <param name="control"></param>
        /// <param name="hide"></param>
        public void HideControls(UIElement control, bool hide = true)
        {
            if (hide)
            {
                control.Visibility = Visibility.Collapsed;
            }
            else
                control.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Fill a combo box.
        /// </summary>
        /// <param name="combobox"></param>
        /// <param name="elements"></param>
        public void FillCombBox(ComboBox combobox, object elements)
        {
            if (combobox.ItemsSource != null) combobox.ItemsSource = null;
            if (elements is IList)
            {
                combobox.ItemsSource = elements as IList;
            }
            else if (elements is Array) 
            {
                combobox.ItemsSource = elements as Array;
            }
        }

        #endregion


        public void setPeriodName(PeriodName periodName)
        {
            PeriodNameTextBox.Text = periodName.name;
            if (this.OnSetPeriod != null)
                OnSetPeriod(periodName.name);
        }

        public void customizeForTarget()
        {
            InitializeParameterTypeForAutomaticTarget();
            this.newTargetElement.customizeForAutomaticTarget();
        }

        public string getGroupTargetName()
        {
            return this.newTargetElement.getGroupTargetName();
        }
    }
}
