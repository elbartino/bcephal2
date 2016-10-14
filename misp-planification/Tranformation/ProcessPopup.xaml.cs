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

namespace Misp.Planification.Tranformation
{
    /// <summary>
    /// Interaction logic for ProcessPopup.xaml
    /// </summary>
    public partial class ProcessPopup : Window
    {

        public LoopUserDialogTemplate LoopUserTemplate { get; set; }

        /// <summary>
        /// ExcelFilesGrid
        /// </summary>
        public BrowserGrid ItemsGrid { get; set; }

        public ProcessPopup()
        {
            InitializeComponent();
            InitializeHandlers();
            ItemsGrid = new BrowserGrid();
            GridPanel.Content = ItemsGrid;
        }

        public void InitializeHandlers() 
        {
            this.helpButton.Click += OnShowHelp;
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

        public void Display() 
        {
            if (this.LoopUserTemplate == null) return ;
            this.TextLabel.Content = this.LoopUserTemplate.message;
            this.HelpTextBlock.Text = this.LoopUserTemplate.help;
        }


    }
}
