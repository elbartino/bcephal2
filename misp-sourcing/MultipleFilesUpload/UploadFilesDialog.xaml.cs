using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Task;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Sourcing.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace Misp.Sourcing.MultipleFilesUpload
{
    /// <summary>
    /// Interaction logic for selectFolder.xaml
    /// </summary>
    public partial class UploadFilesDialog : Window
    {

        private string uploadingStepTile = "4. Tables uploading...";
        private string FileTile = "\nFile : ";
        private SaveInfo currentInfo { get; set; }
        private List<SaveInfo> listeSaveInfo { get; set; }
        private string dialogTitleSeparator = " - ";
        private string dialogTitle = "Multiple files upload";
        /// <summary>
        /// Step1
        /// </summary>
        public Step1 Step1 { get; set; }

        /// <summary>
        /// Step2
        /// </summary>
        public Step2 Step2 { get; set; }

        /// <summary>
        /// Step3
        /// </summary>
        public Step3 Step3 { get; set; }

        /// <summary>
        /// Step4
        /// </summary>
        public Step4 Step4 { get; set; }

        public int ActiveStepNumber { get; set; }

        /// <summary>
        /// InputTableService
        /// </summary>
        public InputTableService InputTableService { get; set; }
               


        public UploadFilesDialog()
        {
            InitializeComponent();
            this.Title = dialogTitle;
            InitializeSteps();
            InitializeHandlers();
        }
                
        private void DisplayStep(int number)
        {
            bool valid = true;
            if (ActiveStepNumber == 1) valid = this.Step1.Validate();
            else if (ActiveStepNumber == 2 && number == 3) valid = this.Step2.Validate();
            if (!valid) return;
            switch (number)
            {
                case 1:
                    {
                        this.Step1.Visibility = System.Windows.Visibility.Visible;
                        this.Step2.Visibility = System.Windows.Visibility.Collapsed;
                        this.Step3.Visibility = System.Windows.Visibility.Collapsed;
                        this.Step4.Visibility = System.Windows.Visibility.Collapsed;
                        this.CancelButton.Visibility = System.Windows.Visibility.Visible;
                        this.BackButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.NextButton.Visibility = System.Windows.Visibility.Visible;
                        this.UploadButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.CloseButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.ProgressGrid1.Visibility = System.Windows.Visibility.Collapsed;
                        this.ProgressGrid2.Visibility = System.Windows.Visibility.Collapsed;
                        this.StatusBarLabel1.Visibility = System.Windows.Visibility.Collapsed;
                        this.StatusBarLabel2.Visibility = System.Windows.Visibility.Collapsed;
                        this.TitleLabel.Content = "1. Select the folder containing the files to upload";
                        break;
                    }

                case 2:
                    {
                        this.Step1.Visibility = System.Windows.Visibility.Collapsed;
                        this.Step2.Visibility = System.Windows.Visibility.Visible;
                        this.Step3.Visibility = System.Windows.Visibility.Collapsed;
                        this.Step4.Visibility = System.Windows.Visibility.Collapsed;
                        this.CancelButton.Visibility = System.Windows.Visibility.Visible;
                        this.BackButton.Visibility = System.Windows.Visibility.Visible;
                        this.NextButton.Visibility = System.Windows.Visibility.Visible;
                        this.UploadButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.CloseButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.ProgressGrid1.Visibility = System.Windows.Visibility.Collapsed;
                        this.ProgressGrid2.Visibility = System.Windows.Visibility.Collapsed;
                        this.StatusBarLabel1.Visibility = System.Windows.Visibility.Collapsed;
                        this.StatusBarLabel2.Visibility = System.Windows.Visibility.Collapsed;
                        this.TitleLabel.Content = "2. Select the input table template";
                        this.Step2.DisplayTales(InputTableService);
                        break;
                    }

                case 3:
                    {
                        this.Step1.Visibility = System.Windows.Visibility.Collapsed;
                        this.Step2.Visibility = System.Windows.Visibility.Collapsed;
                        this.Step3.Visibility = System.Windows.Visibility.Visible;
                        this.Step4.Visibility = System.Windows.Visibility.Collapsed;
                        this.CancelButton.Visibility = System.Windows.Visibility.Visible;
                        this.BackButton.Visibility = System.Windows.Visibility.Visible;
                        this.NextButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.UploadButton.Visibility = System.Windows.Visibility.Visible;
                        this.CloseButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.ProgressGrid1.Visibility = System.Windows.Visibility.Collapsed;
                        this.ProgressGrid2.Visibility = System.Windows.Visibility.Collapsed;
                        this.StatusBarLabel1.Visibility = System.Windows.Visibility.Collapsed;
                        this.StatusBarLabel2.Visibility = System.Windows.Visibility.Collapsed;
                        this.TitleLabel.Content = "3. Define additional parameter";
                        this.Step3.DisplayDefaultGroup(InputTableService.GroupService);
                        break;
                    }
                case 4:
                    {
                        this.Step1.Visibility = System.Windows.Visibility.Collapsed;
                        this.Step2.Visibility = System.Windows.Visibility.Collapsed;
                        this.Step3.Visibility = System.Windows.Visibility.Collapsed;
                        this.Step4.Visibility = System.Windows.Visibility.Visible;
                        this.CancelButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.BackButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.NextButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.UploadButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.CloseButton.Visibility = System.Windows.Visibility.Collapsed;
                        this.ProgressGrid1.Visibility = System.Windows.Visibility.Visible;
                        this.ProgressGrid2.Visibility = System.Windows.Visibility.Hidden;
                        this.StatusBarLabel1.Visibility = System.Windows.Visibility.Visible;
                        this.StatusBarLabel2.Visibility = System.Windows.Visibility.Hidden;
                        this.TitleLabel.Content = uploadingStepTile;
                        break;
                    }
            }
            ActiveStepNumber = number;
            
        }
        

        private void InitializeSteps()
        {
            this.Step1 = new Step1();
            this.StepPanel.Children.Add(this.Step1);
            this.Step1.Visibility = System.Windows.Visibility.Collapsed;

            this.Step2 = new Step2();
            this.StepPanel.Children.Add(this.Step2);
            this.Step2.Visibility = System.Windows.Visibility.Collapsed;

            this.Step3 = new Step3();
            this.StepPanel.Children.Add(this.Step3);
            this.Step3.Visibility = System.Windows.Visibility.Collapsed;

            this.Step4 = new Step4();
            this.StepPanel.Children.Add(this.Step4);
            this.Step4.Visibility = System.Windows.Visibility.Collapsed;

            

            DisplayStep(1);
        }

        protected virtual void OnDoubleClick(object sender, MouseButtonEventArgs args)
        {
            openTable();
        }

        private void InitializeHandlers()
        {
            CancelButton.Click += OnCancel;
            NextButton.Click += OnNext;
            BackButton.Click += OnBack;
            UploadButton.Click += OnUpload;
            CloseButton.Click += OnCancel;
            this.Step4.Grid.MouseDoubleClick += new MouseButtonEventHandler(OnDoubleClick);
            this.Step4.contextMenu.OpenMenuItem.Click +=OpenMenuItem_Click;
            this.Closing +=UploadFilesDialog_Closing;
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            openTable();
        }

        private void UploadFilesDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {   bool cannotClose = false;
           
            if (this.currentInfo != null && !this.currentInfo.isEnd)
                cannotClose = true;

            if (cannotClose)
            {
                e.Cancel = cannotClose;
                MessageDisplayer.ShowAutoClosingMessageBox(this,"Please wait until the process ends", "Load multiple Table");
                return;
            }

        }

        protected void OnUpload(object sender, RoutedEventArgs e)
        {
            List<string> selectedFiles = new List<string>(0);
            foreach (Object item in this.Step1.ExcelFilesGrid.SelectedItems)
            {
                string name = ((FileInfo)item).FullName;
                if (ApplicationManager.Instance.ApplcationConfiguration.IsMultiuser())
                {
                    string path = System.IO.Path.GetDirectoryName(name) + System.IO.Path.DirectorySeparatorChar;
                    name = System.IO.Path.GetFileName(name);
                    this.InputTableService.FileService.FileTransferService.MultipleUploadTable(name, path);                    
                }
                selectedFiles.Add(name);
            }
          
            int selectedTableOID = ((InputTableBrowserData)Step2.TableGrid.SelectedItem).oid;

            if (Step3.GroupField.Group == null) MessageDisplayer.DisplayInfo("Group ", "The groupe value cannot be empty");

            if (Step3.GroupField.Group == null) Step3.GroupField.Group = Step3.GroupField.GroupService.getDefaultGroup();

            int groupOid = Step3.GroupField.Group.oid.Value;

            DisplayStep(4);

            MultiTableLoadData data = new MultiTableLoadData();
            data.templateOid = selectedTableOID;
            data.groupOid = groupOid;
            data.excelFiles = selectedFiles;

            InputTableService.LoadMultipleTableHandler += Update;
            InputTableService.LoadMultipleTable(data);
        }

        protected void Update(SaveInfo info, Object table)
        {
            this.currentInfo = info;
            if (info == null) return;

            int rate = info.stepCount != 0 ? (Int32)(info.stepRuned * 100 / info.stepCount) : 0;
            if (rate >= 100) rate = 100;

            if (info.isEnd == true)
            {
                this.CloseButton.Visibility = System.Windows.Visibility.Visible;
                ProgressGrid2.Visibility = System.Windows.Visibility.Collapsed;
                this.ProgressGrid1.Visibility = Visibility.Hidden;
                this.ProgressGrid2.Visibility = Visibility.Hidden;
                this.StatusBarLabel1.Visibility = Visibility.Hidden;
                this.StatusBarLabel2.Visibility = Visibility.Hidden;

                this.StatusBarLabel1.Content = info.errorMessage;
                this.StatusBarLabel2.Content = "";

                this.TitleLabel.Content = uploadingStepTile;
                this.Title = dialogTitle;
                return;
            }
            else this.Title = dialogTitle + dialogTitleSeparator + info.item;
            ProgressBar1.Maximum = info.stepCount;
            ProgressBar1.Value = info.stepRuned;
           
            ProgressBarTextBlock1.Text = "" + rate + " %" + "( " + info.stepRuned + " / " + info.stepCount + ")";
            //StatusBarLabel1.Content = info.item;
            StatusBarLabel1.Content = "Loading tables ...";

            if (info.currentStepInfo != null)
            {
                rate = info.currentStepInfo.stepCount != 0 ? (Int32)(info.currentStepInfo.stepRuned * 100 / info.currentStepInfo.stepCount) : 0;
                //if (rate > 100) rate = 100;

                if (info.currentStepInfo.stepCount != 0)
                {
                    ProgressGrid2.Visibility = System.Windows.Visibility.Visible;
                    ProgressBar2.Visibility = System.Windows.Visibility.Visible;
                    ProgressBarTextBlock2.Visibility = System.Windows.Visibility.Visible;
                    StatusBarLabel2.Visibility = System.Windows.Visibility.Visible;
                    ProgressBar2.Maximum = info.currentStepInfo.stepCount;
                    ProgressBar2.Value = info.currentStepInfo.stepRuned;
                    ProgressBarTextBlock2.Text = "" + rate + " %" + "( " + info.currentStepInfo.stepRuned + " / " + info.currentStepInfo.stepCount + ")";
                    if (!System.IO.File.Exists(info.currentStepInfo.item))
                    {
                        StatusBarLabel2.Content = info.currentStepInfo.item;                       
                    } 
                }
                if (rate == 100 && !this.CloseButton.IsVisible)
                {
                    ProgressBarTextBlock2.Text = "Table created";
                }
                if(info.currentStepInfo.stepCount <= 75) this.Step4.UpdateGrid(info);        
            }
                
        }    

        private void OnBack(object sender, RoutedEventArgs e)
        {
            DisplayStep(ActiveStepNumber - 1);
        }

        private void OnNext(object sender, RoutedEventArgs e)
        {
            DisplayStep(ActiveStepNumber + 1);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void openTable()
        {
            if (CloseButton.Visibility != System.Windows.Visibility.Visible || this.Step4.Grid.SelectedItems.Count <= 0) return;
            List<object> ids = new List<object>(0);
            int count = this.Step4.Grid.SelectedItems.Count;
            if (this.Step4.Grid.SelectedItems != null && count > 0)
            {
                Object item = this.Step4.Grid.SelectedItems[count - 1];
                if (((SaveInfo)item).errorMessage == null) ids.Add(((SaveInfo)item).oid);
                else 
                {
                    MessageDisplayer.DisplayInfo("Multiple Upload", "Unable to open the table");
                    return;
                }
                HistoryHandler.Instance.openPage(NavigationToken.GetModifyViewToken(Misp.Sourcing.Base.SourcingFunctionalitiesCode.INPUT_TABLE_EDIT, ids));
                Close();
            }
        }
    }
}
