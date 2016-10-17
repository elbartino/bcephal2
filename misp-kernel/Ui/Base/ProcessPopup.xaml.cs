using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using System;
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
        private Dictionary<int?,Value> selectedValuesDico { get; set; }

        /// <summary>
        /// ExcelFilesGrid
        /// </summary>
        public BrowserGrid ValuesGrid { get; set; }

        public ProcessPopup()
        {
            InitializeComponent();
            selectedValuesDico = new Dictionary<int?, Value>(0);
            setTitle("Parametize Loop");
            InitializeValuesGrid();
            InitializeHandlers();
            GridPanel.Content = ValuesGrid;
        }

        public void InitializeHandlers() 
        {
            this.helpButton.Click += OnShowHelp;
            this.nextButton.Click += OnNextClick;
            this.stopButton.Click += OnStopProcess;
        }


        private void InitializeValuesGrid()
        {
            ValuesGrid = new BrowserGrid();
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
            ValuesGrid.SelectionChanged += onValuesGridSelectionChanged;
        }

        private void onValuesGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(IsOneChoice && selectedValuesDico.Count == 1)
            {
               MessageDisplayer.DisplayInfo("Loop User Template ","You are not allowed to select more than one value");
               return;
            }
            if (e.AddedItems.Count > 0) 
            {
                if (e.AddedItems[0] is Value)
                {
                    Value value = (Value)e.AddedItems[0];
                    selectedValuesDico.Add(value.oid, value);
                }
                
            }
            if (e.RemovedItems.Count > 0) 
            {
                if (e.RemovedItems[0] is Value)
                {
                    var value = e.RemovedItems[0] as Value;
                    selectedValuesDico.Remove(value.oid);
                }
            }
        }
            
        private void OnStopProcess(object sender, RoutedEventArgs e)
        {
            LoopUserTemplateData = new LoopUserDialogTemplateData();
            LoopUserTemplateData.stop = true;
            this.Close();
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            LoopUserTemplateData.values = new List<Value>(0);
            LoopUserTemplateData.values.AddRange(selectedValuesDico.Values);
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
            ValuesGrid.ItemsSource = new ObservableCollection<Value>(LoopTemplate.values);
            this.TextLabel.Content = LoopTemplate.message;
            this.HelpTextBlock.Text = LoopTemplate.help;
            this.IsOneChoice = LoopTemplate.onePossibleChoice;
        }
    }
}
