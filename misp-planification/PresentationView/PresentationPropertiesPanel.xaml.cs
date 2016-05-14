using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using Misp.Sourcing.Table;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Planification.PresentationView
{
    /// <summary>
    /// Interaction logic for PresentationPropertiesPanel.xaml
    /// </summary>
    public partial class PresentationPropertiesPanel : ScrollViewer
    {
        public Hyperlink hyperLink;
        public Hyperlink editHyperLink;
        public InputTableGroup InputTableGroup;
        public event OnNewReportEventHandler OnNewReport;
        public delegate void OnNewReportEventHandler();

        public event EditReportEventHander EditReport;

        public event SelectedItemChangedEventHandler OnInsertReport;

        public event OnFolderNameChangedEventHandler OnFolderNameChange;
        public delegate void OnFolderNameChangedEventHandler(string newFolderName);

        public event OnOpenAfterRunEventHandler OnOpenAfterRun;
        public delegate void OnOpenAfterRunEventHandler(bool onRun);

        private string defaultPwpFolder = "Default Folder";

        private bool isEditionMode;

        public PresentationPropertiesPanel()
        {
            InitializeComponent();
            BuildNewReportLink();
            BuildEditReportLink();
            this.OpenAfterRunCheckBox.Checked += OnOpenPresentation;
            this.OpenAfterRunCheckBox.Unchecked += OnOpenPresentation;
            this.savingFolderTextBox.KeyUp += OnValidateSavingFolderName;
            InputTableGroup = new InputTableGroup("Reports", true);
            InputTableGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            InputTableGroup.Background = System.Windows.Media.Brushes.LightBlue;
            InputTableGroup.InputTableTreeview.SelectionChanged += OnInserReport;
            this.ContentReportPanel.Children.Add(InputTableGroup);
        }

        private void OnOpenPresentation(object sender, RoutedEventArgs e)
        {
            if (OnOpenAfterRun != null) OnOpenAfterRun(this.OpenAfterRunCheckBox.IsChecked.Value);
        }

        private void OnInserReport(object selectedReport)
        {
            if (OnInsertReport != null) OnInsertReport(selectedReport);
        }

        public void fillReportList(List<InputTableBrowserData> listeReport) 
        {
            this.InputTableGroup.InputTableTreeview.fillTree(
          new ObservableCollection<InputTableBrowserData>(listeReport)
                );
        }

        public void updateReportList(Kernel.Domain.Report Report) 
        {
            this.InputTableGroup.InputTableTreeview.AddInputTable(Report);
        }

        public void displayPresentation(Kernel.Domain.Presentation presentation)
        {
            if (presentation == null) return;
            nameTextBox.Text = presentation.name;
            groupField.Group = presentation.group;
            savingFolderTextBox.Text = presentation.userSavingDir;
            OpenAfterRunCheckBox.IsChecked = presentation.openPresentationAfterRun;
        }

        public void fillPresentation(Kernel.Domain.Presentation presentation)
        {
            if (presentation == null) return;
            presentation.name = nameTextBox.Text;
            presentation.group = groupField.Group;
        }

        private void BuildNewReportLink()
        {
            Run run = new Run("Create new report");
            this.hyperLink = new Hyperlink(run)
            {
                NavigateUri = new Uri("http://localhost//" + "Create new report"),
            };
            isEditionMode = false;
            newReportTextBlock.Inlines.Add(hyperLink);
            newReportTextBlock.ToolTip = "New report";
            this.hyperLink.RequestNavigate += OnRequestNavigate;
        }

        private void BuildEditReportLink()
        {
            Run run = new Run("Edit report");
            this.editHyperLink = new Hyperlink(run)
            {
                NavigateUri = new Uri("http://localhost//" + "Edit report"),
            };
            isEditionMode = true;
            editReportTextBlock.Inlines.Add(editHyperLink);
            editReportTextBlock.ToolTip = "Edit selected report";
            this.editHyperLink.RequestNavigate += OnEditRequestNavigate;
        }

        private void OnEditRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (EditReport != null) EditReport();
        }
    
        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (OnNewReport != null) OnNewReport();
        }

        private void OnChooseSaveFolder(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = true;
            folderDialog.Description = "Save PowerPoint In";
            
            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            
            var selectedPath = folderDialog.SelectedPath;
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                changeSavingFolder(selectedPath);
            }
        }

        private void OnResetFolder(object sender, MouseButtonEventArgs e)
        {
            changeSavingFolder(null);
        }

        private void changeSavingFolder(String folderPath) 
        {
            string valueText = folderPath != null ? folderPath : defaultPwpFolder;
            this.savingFolderTextBox.Text = valueText;
            this.folderButton.IsEnabled = true;
            if (OnFolderNameChange != null) OnFolderNameChange(folderPath);
        }

        private void OnValidateSavingFolderName(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                changeSavingFolder(this.savingFolderTextBox.Text.Trim());
            }
        }
    }
}
