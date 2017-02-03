using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Util;
using Misp.Planification.Tranformation.TransformationTable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using Misp.Kernel.Ui.Base;
using System.Windows.Controls.Primitives;

namespace Misp.Planification.Tranformation.InstructionControls
{
    /// <summary>
    /// Interaction logic for ThenOrElseItemPanel.xaml
    /// </summary>
    public partial class ThenOrElseItemPanel : Grid
    {

        public ChangeEventHandler ChangeEventHandler;
        public Kernel.Ui.Base.ChangeItemEventHandler Deleted;
        public Kernel.Ui.Base.ChangeItemEventHandler Added;
        public Kernel.Ui.Base.ChangeItemEventHandler IfActionSelected;

        public event OnCloseSlideDialogEventHandler OnCloseSlideDialog;
        public event OnCloseTransformationTableDialogEventHandler OnCloseTransformationTableDialog;

        public delegate void OnCloseSlideDialogEventHandler();
        public delegate void OnCloseTransformationTableDialogEventHandler();    

        public TransformationTableDialog TransformationTableDialog { get; set; }
        public TransformationSlideDialog TransformationSlideDialog { get; set; }
        
        string defaultTransformationTable ="New Table";
        string defaultTransformationSlide= "New Slide";
        string lastSelectedTableName { get; set; }
        string lastSelectedSlideName { get; set; }
        string editedTableName { get; set; }
        public bool trow = false;

        public ThenOrElseItemPanel()
        {
            InitializeComponent();
            this.ActionComboBox.ItemsSource = Instruction.ACTIONS;
            this.LoopComboBox.ItemsSource = BlockPanel.Loops;
            this.ActionComboBox.SelectionChanged += OnSelectedActionChange;

            ActionComboBox.Visibility = Visibility.Visible;
            TableComboBox.Visibility = Visibility.Collapsed;
            LoopComboBox.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;

            initHandlers();
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            this.ActionComboBox.IsEnabled = !readOnly;
            this.LoopComboBox.IsEnabled = !readOnly;
        }

        public Instruction Fill()
        {
            if (ActionComboBox.SelectedItem == null) return null;
            Instruction instruction = new Instruction(ActionComboBox.SelectedItem.ToString(), Instruction.END);
            if (TableComboBox.IsVisible && TableComboBox.SelectedItem != null) instruction.args = TableComboBox.SelectedItem.ToString();
            if (LoopComboBox.IsVisible && LoopComboBox.SelectedItem != null) instruction.args = LoopComboBox.SelectedItem.ToString();
            instruction.comment = this.CommentTextBlock.Text.Trim();
            return instruction;
        }

        public void Display(Instruction instruction)
        {
            trow = false;
            if (instruction == null)
            {
                Reset();
                return;
            }
            this.ActionComboBox.SelectedItem = instruction.start;
            
            if (instruction.isCreateTable())
            {
                if (BlockPanel.TransformationTables == null || BlockPanel.TransformationTables.Count == 0)
                {
                    BlockPanel.TransformationTables = new List<string>();
                    string[] defaultTableList = new string[] { defaultTransformationTable, "" };
                    BlockPanel.TransformationTables.AddRange(defaultTableList.ToList<string>());
                }
                if (!string.IsNullOrEmpty(instruction.args) && !BlockPanel.TransformationTables.Contains(instruction.args))
                {
                    BlockPanel.TransformationTables.Remove("");
                    BlockPanel.TransformationTables.Add(instruction.args);
                    BlockPanel.TransformationTables.Add("");
                }
                this.TableComboBox.SelectedItem = instruction.args;
            }
            if (instruction.isCreateSlide())
            {
                if (BlockPanel.TransformationSlides == null || BlockPanel.TransformationSlides.Count == 0)
                {
                    BlockPanel.TransformationSlides = new List<string>();
                    string[] defaultTableList = new string[] { defaultTransformationSlide, "" };
                    BlockPanel.TransformationSlides.AddRange(defaultTableList.ToList<string>());
                }
                if (!string.IsNullOrEmpty(instruction.args) && !BlockPanel.TransformationSlides.Contains(instruction.args))
                {
                    BlockPanel.TransformationSlides.Remove("");
                    BlockPanel.TransformationSlides.Add(instruction.args);
                    BlockPanel.TransformationSlides.Add("");
                }
                this.TableComboBox.SelectedItem = instruction.args;
            }
            this.LoopComboBox.SelectedItem = BlockPanel.LoopByName(instruction.args);
            this.CommentTextBlock.Text = String.IsNullOrWhiteSpace(instruction.comment) ? "" : instruction.comment.Trim();
            refreshCommentIcon();
            trow = true;
        }
        
        public void Reset()
        {
            this.LoopComboBox.ItemsSource = BlockPanel.Loops;
            this.ActionComboBox.SelectedItem = "";
            this.TableComboBox.SelectedItem = "";
            this.LoopComboBox.SelectedItem = "";
            this.CommentTextBlock.Text = "";
            refreshCommentIcon();
            trow = true;
        }
        bool isSlideOption = false;
        private void OnSelectedActionChange(object sender, SelectionChangedEventArgs e)
        {
            Object selection = this.ActionComboBox.SelectedItem;
            
            if (Instruction.CONTINUE.Equals(selection) || Instruction.STOP.Equals(selection))
            {
                ActionComboBox.Visibility = Visibility.Visible;
                TableComboBox.Visibility = Visibility.Collapsed;
                LoopComboBox.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
            }
            else if (Instruction.NEXT_VALUE.Equals(selection))
            {
                ActionComboBox.Visibility = Visibility.Visible;
                LoopComboBox.Visibility = Visibility.Visible;
                TableComboBox.Visibility = Visibility.Collapsed;                
                EditButton.Visibility = Visibility.Collapsed;
            }
            else if (Instruction.CREATE_TABLE.Equals(selection))
            {
                isSlideOption = false;
                ActionComboBox.Visibility = Visibility.Visible;
                LoopComboBox.Visibility = Visibility.Collapsed;

                TableComboBox.Visibility = Visibility.Visible;
                if (BlockPanel.TransformationTables == null || BlockPanel.TransformationTables.Count == 0)
                {
                    BlockPanel.TransformationTables = new List<string>();
                    BlockPanel.TransformationTables = new List<string>();
                    string[] defaultTableList = new string[] { defaultTransformationTable, "" };
                    BlockPanel.TransformationTables.AddRange(defaultTableList.ToList<string>());
                }
                TableComboBox.ItemsSource = BlockPanel.TransformationTables;
            }
            else if (Instruction.CREATE_SLIDE.Equals(selection))
            {
                isSlideOption = true;
                ActionComboBox.Visibility = Visibility.Visible;
                LoopComboBox.Visibility = Visibility.Collapsed;

                TableComboBox.Visibility = Visibility.Visible;
                if (BlockPanel.TransformationSlides == null || BlockPanel.TransformationSlides.Count == 0)
                {
                    BlockPanel.TransformationSlides = new List<string>();
                    string[] defaultTableList = new string[] { defaultTransformationSlide, "" };
                    BlockPanel.TransformationSlides.AddRange(defaultTableList.ToList<string>());
                }
                TableComboBox.ItemsSource = BlockPanel.TransformationSlides;
            }
            else if (Instruction.IF.Equals(selection))
            {
                if (trow && IfActionSelected != null) IfActionSelected(this);
            }
        }

        protected void initHandlers()
        {
            this.AddButton.Click += OnAddButtonClick;
            this.DeleteButton.Click += OnDeleteButtonClick;
            this.EditButton.Click += OnEditButtonClick;
            this.TableComboBox.SelectionChanged += OnSelectTransformationTable;

            ActionComboBox.SelectionChanged += onChange;            
            LoopComboBox.SelectionChanged += onChange;

            this.CommentTextBlock.TextChanged += OnCommentChange;
            this.CommentPopup.Opened += OnCommentPopupOpened;

            this.CommentButton.Checked += OnComment;
            this.NoCommentButton.Checked += OnComment;
        }

        private void OnComment(object sender, RoutedEventArgs e)
        {
            this.CommentPopup.IsOpen = true;
        }

        private void OnCommentPopupOpened(object sender, EventArgs e)
        {
            this.CommentTextBlock.Focus();
        }

        private void OnCommentChange(object sender, TextChangedEventArgs e)
        {
            onChange();
            refreshCommentIcon();
        }

        private void refreshCommentIcon()
        {
            bool hasComment = !string.IsNullOrWhiteSpace(this.CommentTextBlock.Text);
            this.CommentButton.Visibility = hasComment ? Visibility.Visible : Visibility.Hidden;
            this.NoCommentButton.Visibility = hasComment ? Visibility.Hidden : Visibility.Visible;            
        }

        private void OnSelectTransformationTable(object sender, SelectionChangedEventArgs e)
        {            
            if(this.TableComboBox.SelectedItem == null)
            {
                this.TableComboBox.Text = defaultTransformationTable;
                return;
            }
            
            if (this.TableComboBox.SelectedItem.ToString().Trim().Equals(defaultTransformationTable.Trim())) 
            {
                displayTransformationTable("");
                return;
            }
            if (this.TableComboBox.SelectedItem.ToString().Trim().Equals(defaultTransformationSlide.Trim()))
            {
                displayTransformationSlide("");
                return;
            }
            else
            {
                if (isSlideOption)
                {
                    lastSelectedSlideName = this.TableComboBox.SelectedItem.ToString().Trim();
                }
                else
                {
                    lastSelectedTableName = this.TableComboBox.SelectedItem.ToString().Trim();
                }
                onChange();
            }
            if(!String.IsNullOrEmpty(this.TableComboBox.SelectedItem.ToString())) EditButton.Visibility = Visibility.Visible;
        }

        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            editedTableName = this.TableComboBox.SelectedItem.ToString();
            if (isSlideOption)
            {
                displayTransformationSlide(this.TableComboBox.SelectedItem.ToString());
            }
            else
            {
                displayTransformationTable(this.TableComboBox.SelectedItem.ToString());
            }
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Deleted != null) Deleted(this);
            onChange();
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Added != null) Added(this);
        }

        private void displayTransformationTable(String name)
        {
            if (this.TransformationTableDialog == null)
            {
                this.TransformationTableDialog = new TransformationTableDialog();
                this.TransformationTableDialog.SaveButton.Click += OnTransformationTableDialogSave;
                this.TransformationTableDialog.CancelButton.Click += OnTransformationTableDialogCancel;
                this.TransformationTableDialog.Closing += OnTransformationTableDialogClosing;
                this.TransformationTableDialog.Owner = ApplicationManager.Instance.MainWindow;
            }
            
            Kernel.Domain.TransformationTable table = !string.IsNullOrEmpty(name) ? 
              this.TransformationTableDialog.TransformationTableController.GetTransformationTableService().getByName(name) : null;
           
            if (table == null)
            {
                table = new Kernel.Domain.TransformationTable();
                table.name = name;
                table.group = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetGroupService().getDefaultGroup();
            }

            this.TransformationTableDialog.TransformationTable = table;
            this.TransformationTableDialog.initializeSideBarData(new ObservableCollection<TransformationTreeItem>(BlockPanel.Loops));
            this.TransformationTableDialog.DisplayItem();
            if (this.TransformationSlideDialog != null)
            {
                this.TransformationSlideDialog.PresentationEditorController.OnChange();
            }
            this.TransformationTableDialog.ShowDialog();
        }

        private void displayTransformationSlide(String name)
        {
            Process process = Kernel.Ui.Office.PowerpointLoader.GetRunningPowerPointProcess();
            while (process != null)
            {
                PowerPoint.Application PowerPointApplication = new PowerPoint.Application();
                if (PowerPointApplication.Visible != MsoTriState.msoTrue) Kernel.Ui.Office.PowerpointLoader.KillProcess(process);
                if (MessageDisplayer.DisplayYesNoQuestion("Transformation Tree - Powerpoint ", "You need to close opened Powerpoint application before use this functionnality!\n Do you want B-Cephal to close Powerpoint ?")
                        == MessageBoxResult.Yes) Kernel.Ui.Office.PowerpointLoader.KillProcess(process);
                else return;
                process = null;
            }

            if (this.TransformationSlideDialog == null)
            {
                this.TransformationSlideDialog = new TransformationSlideDialog();
                this.TransformationSlideDialog.SaveButton.Click += OnTransformationSlideDialogSave;
                this.TransformationSlideDialog.CancelButton.Click += OnTransformationSlideDialogCancel;
                this.TransformationSlideDialog.Closing += OnTransformationSlideDialogClosing;
                this.TransformationSlideDialog.Owner = ApplicationManager.Instance.MainWindow;
            }

            Kernel.Domain.Presentation slide = !string.IsNullOrEmpty(name) ?
              this.TransformationSlideDialog.PresentationEditorController.GetPresentationService().getByName(name) : null;

            if (slide == null)
            {
                slide = new Kernel.Domain.Presentation();
                slide.name = name;
                slide.group = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetGroupService().getDefaultGroup();
            }

            this.TransformationSlideDialog.TransformationSlide = slide;
            this.TransformationSlideDialog.initializeSideBarData(new ObservableCollection<TransformationTreeItem>(BlockPanel.Loops));
            this.TransformationSlideDialog.DisplayTransformationSlide();
            this.TransformationSlideDialog.ShowDialog();
        }

        private void OnTransformationTableDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            cancel();
        }

        private void OnTransformationTableDialogCancel(object sender, RoutedEventArgs e)
        {
            cancel();
        }

        private void cancel()
        {
            this.TransformationTableDialog.CloseTableWithoutSave();
            fillTableComboBox(this.lastSelectedTableName, false);
            this.TransformationTableDialog.Hide();
        }

        private void OnTransformationTableDialogSave(object sender, RoutedEventArgs e)
        {
            this.TransformationTableDialog.FillItem();

            bool isModify = this.TransformationTableDialog.TransformationTableController.IsModify;
            if (isModify)
            {
                Kernel.Domain.StructuredReport table = this.TransformationTableDialog.SaveTable();
                if (table != null)
                {
                    fillTableComboBox(table.name, isModify);
                }
            }
          //  this.TransformationTableDialog.TransformationTableController.Close();
           // this.TransformationTableDialog.Hide();
        }

        private void OnTransformationSlideDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (this.TransformationSlideDialog.IsReportView)
            {
                this.TransformationSlideDialog.CloseReportWithoutSave();
                return;
            }

            this.TransformationSlideDialog.CloseSlideWithoutSave();
            fillSlideComboBox(this.lastSelectedSlideName, false);
            this.TransformationSlideDialog.Hide();
            TreeActionDialog.IsActionReportView = true;
        }

        private void OnTransformationSlideDialogCancel(object sender, RoutedEventArgs e)
        {
            if (this.TransformationSlideDialog.IsReportView)
            {
                this.TransformationSlideDialog.CloseReportWithoutSave();
                return;
            }
            this.TransformationSlideDialog.CloseSlideWithoutSave();
            fillSlideComboBox(this.lastSelectedSlideName, false);
            this.TransformationSlideDialog.Hide();
            TreeActionDialog.IsActionReportView = true;
        }

        private void OnTransformationSlideDialogSave(object sender, RoutedEventArgs e)
        {
            if (this.TransformationSlideDialog.IsReportView)
            {
                this.TransformationSlideDialog.CloseReportWithSave();
                return;
            }

            bool isModify = this.TransformationSlideDialog.PresentationEditorController.IsModify;
            if (isModify)
            {
                Kernel.Domain.Presentation slide = this.TransformationSlideDialog.SaveSlide();
                if (slide != null)
                {
                    fillSlideComboBox(slide.name, isModify);
                }
            }
            TreeActionDialog.IsActionReportView = true;
        }

        
        private void fillTableComboBox(string tableName, bool IsModify)
        {
            if (BlockPanel.TransformationTables == null || BlockPanel.TransformationTables.Count == 0)
            {
                BlockPanel.TransformationTables = new List<string>(0);
                string[] defaultTableList = new string[] { defaultTransformationTable, "" };
                BlockPanel.TransformationTables.AddRange(defaultTableList.ToList<string>());
            }

            if (String.IsNullOrEmpty(tableName) || tableName.Trim().Equals(defaultTransformationTable))
            {
                this.TableComboBox.ItemsSource = null;
                this.TableComboBox.ItemsSource = BlockPanel.TransformationTables;
                return;
            }

            if (!String.IsNullOrEmpty(tableName) && !BlockPanel.TransformationTables.Contains(tableName) && IsModify)
            {
                BlockPanel.TransformationTables.Remove(editedTableName);
                BlockPanel.TransformationTables.Remove("");
                BlockPanel.TransformationTables.Add(tableName);
                BlockPanel.TransformationTables.Add("");
                this.TableComboBox.ItemsSource = null;
                this.TableComboBox.ItemsSource = BlockPanel.TransformationTables;
                this.TableComboBox.SelectedItem = tableName;
            }
            else
            {
                this.TableComboBox.SelectedItem = !String.IsNullOrEmpty(tableName) &&  !this.lastSelectedTableName.Equals(defaultTransformationTable)  ?
                  tableName : "";
            }
        }

        private void fillSlideComboBox(string slideName, bool IsModify)
        {
            if (BlockPanel.TransformationSlides == null || BlockPanel.TransformationSlides.Count == 0)
            {
                BlockPanel.TransformationSlides = new List<string>(0);
                string[] defaultTableList = new string[] { defaultTransformationSlide, "" };
                BlockPanel.TransformationSlides.AddRange(defaultTableList.ToList<string>());
            }

            if (String.IsNullOrEmpty(slideName) || slideName.Trim().Equals(defaultTransformationSlide))
            {
                this.TableComboBox.ItemsSource = null;
                this.TableComboBox.ItemsSource = BlockPanel.TransformationSlides;
                return;
            }

            if (!String.IsNullOrEmpty(slideName) && !BlockPanel.TransformationSlides.Contains(slideName) && IsModify)
            {
                BlockPanel.TransformationSlides.Remove(editedTableName);
                BlockPanel.TransformationSlides.Remove("");
                BlockPanel.TransformationSlides.Add(slideName);
                BlockPanel.TransformationSlides.Add("");
                this.TableComboBox.ItemsSource = null;
                this.TableComboBox.ItemsSource = BlockPanel.TransformationSlides;
                this.TableComboBox.SelectedItem = slideName;
            }
            else
            {
                bool isLastSelectedSlideValid = !string.IsNullOrEmpty(lastSelectedSlideName);
                isLastSelectedSlideValid = !isLastSelectedSlideValid ? false : !this.lastSelectedSlideName.Equals(defaultTransformationSlide);
                this.TableComboBox.SelectedItem = isLastSelectedSlideValid ? lastSelectedSlideName : "";
            }
        }

        private void verifySlileItemsSource(string tableName) 
        {
            if (string.IsNullOrEmpty(editedTableName)) BlockPanel.TransformationSlides.Remove(editedTableName);
            BlockPanel.TransformationSlides.Remove("");
            BlockPanel.TransformationSlides.Add(tableName);
            BlockPanel.TransformationSlides.Add("");
            this.TableComboBox.ItemsSource = null;
            this.TableComboBox.ItemsSource = BlockPanel.TransformationSlides;
        }


        private void onChange(object sender, SelectionChangedEventArgs e)
        {
            onChange();
        }

        public void onChange()
        {
            if (trow && ChangeEventHandler != null) ChangeEventHandler();
        }


    }
}
