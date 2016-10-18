using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for ProcessPopup.xaml
    /// </summary>
    public partial class ProcessPopup : Window
    {

        public LoopUserDialogTemplateData LoopUserTemplateData { get; set; }
        private bool IsOneChoice;
        private bool isCloseAction;

        /// <summary>
        /// ExcelFilesGrid
        /// </summary>
        public BrowserGrid ValuesGrid { get; set; }

        public ProcessPopup()
        {
            InitializeComponent();
            setTitle("Parametize Loop");
            InitializeValuesGrid();
            InitializeHandlers();
            GridPanel.Content = ValuesGrid;
            nextButton.IsEnabled = false;
        }

        public void InitializeHandlers() 
        {
            this.helpButton.Click += OnShowHelp;
            this.nextButton.Click += OnNextClick;
            this.stopButton.Click += OnStopProcess;
            this.Closing += OnClosing;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isCloseAction = true;
            if (isCloseAction) 
            {
                stopProcess();
            }
        }


        private void InitializeValuesGrid()
        {
            ValuesGrid = new BrowserGrid();
            ValuesGrid.FilterHandler.Handler -= ValuesGrid.OnFilter;
            ValuesGrid.hideContextMenu();
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
            checkboxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsSelected") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1) });
            gridFactory.AppendChild(checkboxFactory);
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            ValuesGrid.RowHeaderTemplate = template;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            ValuesGrid.AlternatingRowBackground = bruch;
            ValuesGrid.AlternatingRowBackground.Opacity = 0.3;

            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = "Values";
            column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            column.Binding = new System.Windows.Data.Binding("name");
            ValuesGrid.Columns.Add(column);
            ValuesGrid.SelectionChanged += onSelectionChanged;
        }

        private void onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList items = ValuesGrid.SelectedItems;
            nextButton.IsEnabled = items.Count > 0;
        }
                    
        private void OnStopProcess(object sender, RoutedEventArgs e)
        {
            isCloseAction = false;
            stopProcess();
        }

        private void stopProcess() 
        {
            MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Stop tree execution", "You are about to stop the tree execution.\nDo You want to stop?");
            if (response == MessageBoxResult.Yes)
            {
                LoopUserTemplateData = new LoopUserDialogTemplateData();
                LoopUserTemplateData.stop = true;                
            }
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            LoopUserTemplateData = new LoopUserDialogTemplateData();
            IList items = ValuesGrid.SelectedItems;
            if (IsOneChoice && items.Count > 1)
            {
                MessageDisplayer.DisplayWarning("Wrong selection", "You are not allowed to select more than one element!");
                return;
            }
            foreach (Object obj in items)
            {
                if (obj is Value) LoopUserTemplateData.values.Add((Value)obj);
            }
            isCloseAction = false;
            this.Close();            
        }

        private void OnShowHelp(object sender, RoutedEventArgs e)
        {
            HelpPopup.IsOpen = true;
        }


        public void setTitle(String title) 
        {
            this.Title = title;
        }

        public void setTextLable(String text) 
        {
            this.TextLabel.Content = text;
        }
        
        public void Display(LoopUserDialogTemplateData LoopTemplate)
        {
            if (LoopTemplate == null) return;
            this.LoopUserTemplateData = LoopTemplate;
            this.IsOneChoice = LoopTemplate.onePossibleChoice;
            ValuesGrid.SelectionMode = this.IsOneChoice ? DataGridSelectionMode.Single : DataGridSelectionMode.Extended;
            ValuesGrid.ItemsSource = new ObservableCollection<Value>(LoopTemplate.values);
            this.TextLabel.Content = LoopTemplate.message;
            this.HelpTextBlock.Text = LoopTemplate.help;
            this.OnChoiceCheckbox.IsChecked = this.IsOneChoice;
        }
    }
}
