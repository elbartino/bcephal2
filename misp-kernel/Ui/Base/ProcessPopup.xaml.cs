using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
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

        /// <summary>
        /// ExcelFilesGrid
        /// </summary>
        public BrowserGrid ItemsGrid { get; set; }

        public ProcessPopup()
        {
            InitializeComponent();
            InitializeHandlers();
            setTitle("Parametize Loop");
            ItemsGrid = new BrowserGrid();
            GridPanel.Content = ItemsGrid;
        }

        public void InitializeHandlers() 
        {
            this.helpButton.Click += OnShowHelp;
            this.nextButton.Click += OnNextClick;
            this.stopButton.Click += OnStopProcess;
        }

        private void OnStopProcess(object sender, RoutedEventArgs e)
        {
            LoopUserTemplateData = new LoopUserDialogTemplateData();
            LoopUserTemplateData.stop = true;
            this.Close();
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            LoopUserTemplateData = new LoopUserDialogTemplateData();
            LoopUserTemplateData.values = (List<Value>)this.ItemsGrid.SelectedItems;
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
            this.ItemsGrid.ItemsSource = LoopTemplate.values;
            this.TextLabel.Content = LoopTemplate.message;
            this.HelpTextBlock.Text = LoopTemplate.help;
            this.IsOneChoice = LoopTemplate.onePossibleChoice;
        }
    }
}
