﻿using System;
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
using Misp.Kernel.Domain;
using Misp.Kernel.Util;
using Misp.Kernel.Ui.Measure;
using Misp.Kernel.Ui.TreeView;
using Misp.Kernel.Service;


namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for AllocationPanel.xaml
    /// </summary>
    public partial class AllocationPanel : ScrollViewer
    {
        public Kernel.Ui.Base.ChangeEventHandler Change;
        public Kernel.Ui.Base.ChangeItemEventHandler AllocationTypeChanged;
        public bool thrawChange = true;

        public CellPropertyAllocationData AllocationData { get; set; }

        public MeasureService MeasureService { get; set; }

        private Measure outputMeasure;
        public Measure OutputMeasure
        {
            get { return this.outputMeasure; }
            set
            {
                this.outputMeasure = value;
                this.OutputMeasureTextBox.Text = outputMeasure != null ? outputMeasure.name : "";
            }
        }

        private Measure refMeasure;
        public Measure RefMeasure
        {
            get { return this.refMeasure; }
            set
            {
                this.refMeasure = value;
                this.RefMeasureTextBox.Text = refMeasure != null ? refMeasure.name : "";
            }
        }

        

        public AllocationPanel()
        {
            InitializeComponent();
            SequenceTextBox.Text = "1";
            InitTypeComboBox();
            this.MeasureService = Kernel.Application.ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetMeasureService();
            updateButtons();
            InitHandlers();
        }

        private void InitTypeComboBox()
        {
            this.TypeComboBox.ItemsSource = new string[] { 
                CellPropertyAllocationData.AllocationType.NoAllocation.ToString(),
                CellPropertyAllocationData.AllocationType.Linear.ToString(), 
                CellPropertyAllocationData.AllocationType.Reference.ToString() 
                //CellPropertyAllocationData.AllocationType.Template.ToString() 
            };
            this.TypeComboBox.SelectedItem = CellPropertyAllocationData.AllocationType.NoAllocation.ToString();
        }


        /// <summary>
        /// recupere les parametres d'allocation et les enregistre 
        /// </summary>
        public void FillAllocationData()
        {
            if (this.AllocationData == null) this.AllocationData = new CellPropertyAllocationData();
            string selectType = (string)TypeComboBox.SelectedItem;
            this.AllocationData.type = string.IsNullOrEmpty(selectType) ? null : selectType;
            bool addRefMeasuref = selectType.Equals(CellPropertyAllocationData.AllocationType.Reference.ToString());
            this.AllocationData.measureRef = addRefMeasuref ? this.RefMeasure : null;
            this.AllocationData.showGridInShortcut = this.ShowInShorcut.IsChecked.Value;
            this.AllocationData.considerCell = this.ConsiderCell.IsChecked.Value;
        }
        
        /// <summary>
        /// affiche les parametres d'allocation sur la vue
        /// </summary>
        /// <param name="allocationData"></param>
        public void DisplayAllocationData(CellPropertyAllocationData allocationData,bool readOnly = false)
        {
            thrawChange = false;
            this.AllocationData = allocationData;
            if (allocationData != null)
            {
                this.TypeComboBox.SelectedItem = allocationData.type;
                this.RefMeasure = allocationData.measureRef;
                this.ShowInShorcut.IsChecked = allocationData.showGridInShortcut;
                this.ConsiderCell.IsChecked = allocationData.considerCell;
            }
            else
            {
                this.TypeComboBox.SelectedItem = CellPropertyAllocationData.AllocationType.NoAllocation.ToString();
                this.OutputMeasure = null;
                this.RefMeasure = null;
                this.TemplateTextBox.Text = "";
                this.SequenceTextBox.Text = "1";
                this.ShowInShorcut.IsChecked = true;
                this.ConsiderCell.IsChecked = true;
            }
            updateButtons();
            thrawChange = true;
        }

        public void SetReadOnly(bool readOnly)
        {
            this.TypeComboBox.IsEnabled = !readOnly;
            this.ShowInShorcut.IsEnabled = !readOnly;
            this.ConsiderCell.IsEnabled = !readOnly;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void updateButtons()
        {
            string selectType = (string)TypeComboBox.SelectedItem;
            if (string.IsNullOrEmpty(selectType)) return;
            if (CellPropertyAllocationData.AllocationType.Linear.ToString() == selectType)
            {
                MeasureRow.Height = new GridLength(0, GridUnitType.Star);
                RefMeasureRow.Height = new GridLength(0, GridUnitType.Star);
                TemplateRow.Height = new GridLength(0, GridUnitType.Star);
                SequenceRow.Height = new GridLength(0, GridUnitType.Star); ;

                MeasureGrid.Visibility = System.Windows.Visibility.Collapsed;
                RefMeasureGrid.Visibility = System.Windows.Visibility.Collapsed;
                TemplateGrid.Visibility = System.Windows.Visibility.Collapsed;
                ShowGridGrid.Visibility = System.Windows.Visibility.Visible;
                ConsiderCellGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (CellPropertyAllocationData.AllocationType.Scope2Scope.ToString() == selectType)
            {
                MeasureRow.Height = new GridLength(0, GridUnitType.Star);
                RefMeasureRow.Height = new GridLength(27);
                TemplateRow.Height = new GridLength(0, GridUnitType.Star);
                SequenceRow.Height = new GridLength(27);

                MeasureGrid.Visibility = System.Windows.Visibility.Collapsed;
                RefMeasureGrid.Visibility = System.Windows.Visibility.Visible;
                TemplateGrid.Visibility = System.Windows.Visibility.Collapsed;
                SequenceGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else if (CellPropertyAllocationData.AllocationType.Reference.ToString() == selectType) 
            {
                RefMeasureRow.Height = new GridLength(27);
                RefMeasureGrid.Visibility = System.Windows.Visibility.Visible;
                this.RefMeasureButton.Visibility = System.Windows.Visibility.Collapsed;
                ShowGridGrid.Visibility = System.Windows.Visibility.Visible;
                ConsiderCellGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else if (CellPropertyAllocationData.AllocationType.Template.ToString() == selectType)
            {

            }
            else
            {
                MeasureRow.Height = new GridLength(0, GridUnitType.Star);
                RefMeasureRow.Height = new GridLength(0, GridUnitType.Star);
                TemplateRow.Height = new GridLength(0, GridUnitType.Star);
                SequenceRow.Height = new GridLength(0, GridUnitType.Star);

                MeasureGrid.Visibility = System.Windows.Visibility.Collapsed;
                RefMeasureGrid.Visibility = System.Windows.Visibility.Collapsed;
                TemplateGrid.Visibility = System.Windows.Visibility.Collapsed;
                SequenceGrid.Visibility = System.Windows.Visibility.Collapsed;
                ShowGridGrid.Visibility = System.Windows.Visibility.Collapsed;
                ConsiderCellGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            SequenceGrid.Visibility = System.Windows.Visibility.Collapsed;
            this.OutputMeasureButton.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// initialise les handler sur les changements effectué sur le bloc allocation panel
        /// </summary>
        public void InitHandlers()
        {
            this.TypeComboBox.SelectionChanged += OnAllocationTypeChanged;
            this.OutputMeasureButton.Click += OnOutputMeasureButtonClicked;
            this.RefMeasureButton.Click += OnRefMeasureButtonClicked;
            this.TemplateButton.Click += OnTemplateButtonClicked;
            this.SequenceTextBox.TextChanged += OnSequenceChanged;
            this.ShowInShorcut.Checked += OnSelectShorcutOptions;
            this.ShowInShorcut.Unchecked += OnSelectShorcutOptions;
            this.ConsiderCell.Checked += OnConsiderCell;
            this.ConsiderCell.Unchecked += OnConsiderCell;
        }

        private void OnConsiderCell(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        private void OnSelectShorcutOptions(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSequenceChanged(object sender, TextChangedEventArgs e)
        {
            OnChange();
        }
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAllocationTypeChanged(object sender, SelectionChangedEventArgs e)
        {            
            updateButtons();
            if (AllocationTypeChanged != null) AllocationTypeChanged(this.TypeComboBox.SelectedItem);
            OnChange();
        }

        /// <summary>
        /// methode appelé lorsqu'on veut selectioner un measure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOutputMeasureButtonClicked(object sender, EventArgs e)
        {
            Misp.Kernel.Ui.Measure.MeasureTreeView measureTreeView = new Misp.Kernel.Ui.Measure.MeasureTreeView();
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = measureTreeView;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            if (this.OutputMeasure != null && !this.OutputMeasure.IsLeaf())
            {
                measureTreeView.DisplayRoot(this.OutputMeasure);
            }
            else
            {
                Kernel.Domain.Measure root = this.MeasureService.getRootMeasure();
                measureTreeView.DisplayRoot(root);
                if (this.OutputMeasure != null) measureTreeView.SetSelectedMeasure(this.OutputMeasure);
            }
                        
            Dialog dialog = new Dialog("Select Output Measure", scrollViewer);
            dialog.Height = 200;
            dialog.Width = 300;
            if (dialog.ShowCenteredToMouse().Value)
            {
                Measure selectedMeasure = measureTreeView.GetSelectedMultiMeasure();
                if (selectedMeasure != null)
                {
                    if (selectedMeasure.oid == null)
                    {
                        selectedMeasure = this.MeasureService.Save(selectedMeasure);
                    }
                    else selectedMeasure = this.MeasureService.getByOid(selectedMeasure.oid.Value);
                    this.OutputMeasure = selectedMeasure;
                    OnChange();
                }
            }
        }

        private void OnTemplateButtonClicked(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnRefMeasureButtonClicked(object sender, RoutedEventArgs e)
        {
            Misp.Kernel.Ui.Measure.MeasureTreeView measureTreeView = new Misp.Kernel.Ui.Measure.MeasureTreeView();
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = measureTreeView;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Kernel.Domain.Measure root = this.MeasureService.getRootMeasure();
            measureTreeView.DisplayRoot(root);
            if (this.OutputMeasure != null) measureTreeView.SetSelectedMeasure(this.RefMeasure);
            Dialog dialog = new Dialog("Select Reference Measure", scrollViewer);
            dialog.Height = 200;
            dialog.Width = 300;
            if (dialog.ShowCenteredToMouse().Value)
            {
                Measure selectedMeasure = measureTreeView.GetSelectedMultiMeasure();
                if (selectedMeasure != null)
                {
                    if (selectedMeasure.oid == null)
                    {
                        selectedMeasure = this.MeasureService.Save(selectedMeasure);
                    }
                    else selectedMeasure = this.MeasureService.getByOid(selectedMeasure.oid.Value);
                    this.RefMeasure = selectedMeasure;
                    OnChange();
                }
            }
        }

        public List<System.Windows.UIElement> getEditableControls()
        {
            List<System.Windows.UIElement> controls = new List<System.Windows.UIElement>(0);
            //controls.Add(this.re);
            return controls;
        }

        private void OnChange()
        {
            if (Change != null && thrawChange)
            {
                FillAllocationData();
                Change();
            }
        }



        public void setReferenceMeasure(Kernel.Domain.Measure measure)
        {
            if (this.TypeComboBox.SelectedItem.ToString().Equals(CellPropertyAllocationData.AllocationType.Reference.ToString()))
            {
                this.RefMeasure = measure;
                OnChange();
            }
        }
    }
}
