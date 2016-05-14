using Misp.Kernel.Application;
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

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for ModelModificationDialog.xaml
    /// </summary>
    public partial class ModelModificationDialog : Window
    {

        public event SendOkRequestEventHandler OnValidate;
        public delegate void SendOkRequestEventHandler(Kernel.Domain.TableSaveIssue tableSaveIssue);

        private bool isIgnore;

        public bool canClose { get; set; }
        private bool isMatch { get; set; }

        private string unKnowText = "Unknow ";
        private string valueText = " value ";
        private string measureText = " measure ";
        private string periodText = " period ";
        public Kernel.Domain.TableSaveIssue tableSaveIssue { get; set; }
        public InputTableService InputTableService { get; set; }
        private Kernel.Domain.AttributeValue selectedAttributeValue { get; set; }
        public ModelModificationDialog()
        {
            InitializeComponent();
            this.Owner = ApplicationManager.Instance.MainWindow;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.okButton.Click += OnOkClick;
            this.treeviewMatch.SelectionChanged += treeviewMatch_SelectionChanged;
            this.measureTreeview.SelectionChanged +=measureTreeview_SelectionChanged;
            this.ApplyToAll.Checked += OnApplyToAll;
            this.ApplyToAll.Unchecked += OnApplyToAll;
            this.dateChooser.SelectedDateChanged += dateChooser_SelectedDateChanged;
            this.ApplyToAll.Visibility = System.Windows.Visibility.Visible;
            this.stopDateRadioButton.Visibility = System.Windows.Visibility.Collapsed;
            this.ApplyToAllDate.Checked += OnApplyToAll;
            this.stopDateRadioButton.Checked += stopDateRadioButton_Checked;
            canClose = true;
        }

        private void stopDateRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            isMatch = false;
            this.dateChooser.SelectedDate = null;
            this.okButton.IsEnabled = true;
        }

        private void measureTreeview_SelectionChanged(object newSelection)
        {
            if (newSelection == null) return;
            if (newSelection is Kernel.Domain.Measure)
            {
                this.manualMatch.Text = ((Kernel.Domain.Measure)newSelection).name;
                this.okButton.IsEnabled = true;
                if (this.tableSaveIssue.cellMeasure == null) return;
                this.tableSaveIssue.cellMeasure.measure = (Kernel.Domain.Measure)newSelection;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            //if (!canClose)
           // return;
        }


        private void dateChooser_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? date = dateChooser.SelectedDate;
            this.manualMatch.Text = date.HasValue ? date.Value.ToString() : "";
            this.tableSaveIssue.period = date.HasValue ? date.Value.ToString() : "";
            this.okButton.IsEnabled = date != null;
            this.stopDateRadioButton.IsChecked = date == null;
        }

        private void OnApplyToAll(object sender, RoutedEventArgs e)
        {
            if(this.ApplyToAll.IsVisible)
                this.tableSaveIssue.applyToAll = this.ApplyToAll.IsChecked.Value;
            if(this.ApplyToAllDate.IsVisible)
                this.tableSaveIssue.applyToAll = this.ApplyToAllDate.IsChecked.Value;
        }

   
        public void Display(Kernel.Domain.TableSaveIssue tablesavesssue) 
        {
            if (tablesavesssue == null) return;
            this.tableSaveIssue = tablesavesssue;
            this.ValueCell.Text = tableSaveIssue.excelCellValue;
            if (String.IsNullOrEmpty(tableSaveIssue.tableName))
            {
                this.TableName.Visibility = System.Windows.Visibility.Collapsed;
                this.dateLabel.Visibility = System.Windows.Visibility.Collapsed;
                this.TableLabel.Visibility = System.Windows.Visibility.Collapsed;
                this.Title = "Target save issues";
            }
            else
            {
                this.TableName.Text = tableSaveIssue.tableName;
            }

            this.UploadTable.Visibility = System.Windows.Visibility.Visible;
            this.Display();
        }

        private void DisplayMeasureView() 
        {
            this.LabelValue.Content = unKnowText + measureText;
            if (!isMatch)
            {
                HideAll();
            }
            else
            {
               // this.ApplyToAll.Visibility = System.Windows.Visibility.Visible;
                this.treeviewMatch.Visibility = System.Windows.Visibility.Collapsed;
                this.measureTreeview.Visibility = System.Windows.Visibility.Visible;
                this.dateGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.measureTreeview.DisplayRoot(this.InputTableService.MeasureService.getRootMeasure());
            }
        }

        private void DisplayTargetItemView()
        {
            this.LabelValue.Content = unKnowText + valueText;
            if (!isMatch)
            {
                HideAll();
            }
            else
            {
               // this.ApplyToAll.Visibility = System.Windows.Visibility.Visible;
                this.treeviewMatch.Visibility = System.Windows.Visibility.Visible;
                this.measureTreeview.Visibility = System.Windows.Visibility.Collapsed;
                this.dateGrid.Visibility = System.Windows.Visibility.Collapsed;
                if (this.tableSaveIssue.targetItem.attribute != null) DisplayAttributeWithValues(this.tableSaveIssue.targetItem.attribute);
            }
        }
        
        private void DisplayPeriodView()
        {
            this.LabelValue.Content = unKnowText + periodText;
            
            this.dateGrid.Visibility = System.Windows.Visibility.Visible;
            this.ApplyToAllDate.Visibility = System.Windows.Visibility.Visible;
            this.stopDateRadioButton.Visibility = System.Windows.Visibility.Visible;
            this.addAuto.IsChecked = false;
            this.addManual.IsChecked = true;
           
            isMatch = true;
            this.treeviewMatch.Visibility = System.Windows.Visibility.Collapsed;
            this.checkOption.Visibility = System.Windows.Visibility.Collapsed;
            this.measureTreeview.Visibility = System.Windows.Visibility.Collapsed;
            this.addAuto.Visibility = System.Windows.Visibility.Collapsed;
            this.addManual.Visibility = System.Windows.Visibility.Collapsed;
            this.manualMatch.Visibility = System.Windows.Visibility.Collapsed;
            this.ApplyToAll.Visibility = Visibility.Collapsed;
            this.Height = 250;
        }

        public void Display(InputTableService service, Kernel.Domain.TableSaveIssue tableSaveIssue)
        {
            this.InputTableService = service;
            this.tableSaveIssue = tableSaveIssue;
            Display(tableSaveIssue);
        }

  
        private void treeviewMatch_SelectionChanged(object newSelection)
        {
            if (newSelection == null) return;
            if (newSelection is Kernel.Domain.AttributeValue)
            {
                selectedAttributeValue = (Kernel.Domain.AttributeValue)newSelection;
                this.manualMatch.Text = selectedAttributeValue.name;
                this.okButton.IsEnabled = true;
                if (this.tableSaveIssue.targetItem == null) return;
                this.tableSaveIssue.targetItem.value = (Kernel.Domain.AttributeValue)newSelection;
            }
            
        }
        
        public void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (isMatch && this.manualMatch.Text == "")
            {
                Kernel.Util.MessageDisplayer.DisplayInfo("Save issue", "There is no matching value");
                return;
            }
            
            if(this.addAuto.IsChecked.Value) this.tableSaveIssue.decision = Kernel.Domain.TableSaveIssue.Decision.ADD.ToString();
            else if (this.stopDateRadioButton.IsChecked.Value && this.stopDateRadioButton.IsVisible)
            {
                this.tableSaveIssue.decision = Kernel.Domain.TableSaveIssue.Decision.STOP.ToString();
                MessageBoxResult response = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Save issue", "You are about to stop the current operation.\nDo tou want to continue?");
                if (response != MessageBoxResult.Yes) return;
            }
            else if (this.addManual.IsChecked.Value) this.tableSaveIssue.decision = Kernel.Domain.TableSaveIssue.Decision.MACHT.ToString();

            else if (this.UploadTable.IsChecked.Value)
            {
                this.tableSaveIssue.decision = Kernel.Domain.TableSaveIssue.Decision.STOP.ToString();
                MessageBoxResult response = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Save issue", "You are about to stop the current operation.\nDo tou want to continue?");
                if (response != MessageBoxResult.Yes) return;
            }
            else this.tableSaveIssue.decision = Kernel.Domain.TableSaveIssue.Decision.NULL.ToString();


            canClose = true;
            this.Close();     
        }

        private void addAuto_Checked_1(object sender, RoutedEventArgs e)
        {
            isMatch = false;

            HideAll();
            if (this.manualMatch == null) return;
            this.manualMatch.Text = "";
          //  this.ApplyToAll.Visibility = System.Windows.Visibility.Visible;
            this.okButton.IsEnabled = true;
            this.tableSaveIssue.decision = Kernel.Domain.TableSaveIssue.Decision.ADD.ToString();
        }

        private void addManual_Checked_1(object sender, RoutedEventArgs e)
        {
            isMatch = true;
            this.tableSaveIssue.decision = Kernel.Domain.TableSaveIssue.Decision.MACHT.ToString();
            this.tableSaveIssue.applyToAll = this.ApplyToAll.IsChecked.Value;
            this.manualMatch.Visibility = System.Windows.Visibility.Visible;
            this.okButton.IsEnabled = false;
            this.Display();
        }

        private void Display(){
            if (this.tableSaveIssue.cellMeasure != null) DisplayMeasureView();
            else if (this.tableSaveIssue.targetItem != null) DisplayTargetItemView();
            else DisplayPeriodView();
        }

        private void OnCancelUpload(object sender, RoutedEventArgs e)
        {
            this.treeviewMatch.Visibility = System.Windows.Visibility.Collapsed;
            this.manualMatch.Visibility = System.Windows.Visibility.Collapsed;
            this.tableSaveIssue.decision = Kernel.Domain.TableSaveIssue.Decision.STOP.ToString();
            this.okButton.IsEnabled = true;
        }

        private void DisplayAttributeWithValues(Kernel.Domain.Attribute attribute)
        {
            this.treeviewMatch.DisplayAttributeWithValues(attribute);
        }


        private void HideAll() {
            if (this.treeviewMatch == null) return;
            if (this.measureTreeview == null) return;
            this.treeviewMatch.Visibility =  System.Windows.Visibility.Collapsed;
            this.measureTreeview.Visibility = System.Windows.Visibility.Collapsed;
            this.dateChooser.Visibility = System.Windows.Visibility.Collapsed;
            this.manualMatch.Visibility = System.Windows.Visibility.Collapsed;
            this.ApplyToAllDate.Visibility = System.Windows.Visibility.Collapsed;
        }

    }
}
