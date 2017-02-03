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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Ui.Office.EDraw;
using Misp.Kernel.Domain;
using System.Windows.Forms.Integration;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Misp.Sourcing.Base;
using Misp.Kernel.Ui.Office.DevExpressSheet;
namespace Misp.Sourcing.AutomaticSourcingViews
{
    public class AutomaticSourcingForm : UserControl, IEditableView<Misp.Kernel.Domain.AutomaticSourcing>
    {
        #region Properties

        public bool IsReadOnly { get; set; }
        
        public MenuItem RunMenuItem;
        public MenuItem ClearMenuItem;
        public CheckBox ApplyToAllMenuItem;

        public MenuItem AuditMenuItem;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public AutomaticSourcingForm()
        {
            InitializeComponents();
            InitializeExcelMenu();
      
        }

        private void InitializeExcelMenu()
        {
            RunMenuItem = BuildContextMenuItem("Run");
            ClearMenuItem = BuildContextMenuItem("Clear");
            ApplyToAllMenuItem = BuildContextCheckBox("Apply To All");
            AuditMenuItem = BuildContextMenuItem("Audit");

        }

        protected System.Windows.Controls.MenuItem BuildContextMenuItem(string header)
        {
            System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
            menuItem.Header = header;
            return menuItem;
        }

        protected System.Windows.Controls.CheckBox BuildContextCheckBox(string header)
        {
            System.Windows.Controls.CheckBox menuItem = new System.Windows.Controls.CheckBox();
            menuItem.Content = header;
            return menuItem;
        }

        TabItem auditTabItem;
        TabItem designTabItem;
        WindowsFormsHost host;
        System.Windows.Controls.Image image;
        protected virtual void InitializeComponents()
        {

            AutomaticSourcingPanel = new AutomaticSourcingPanel();
            AutomaticTablePropertiesPanel = new AutomaticTablePropertiesPanel();
            this.Background = null;
            this.BorderBrush = null;

        //    ItemsPanelTemplate temp = this.ItemsPanel;
            Uri rd1 = new Uri("../Resources/Styles/TabControl.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(Application.LoadComponent(rd1) as ResourceDictionary);     
            designTabItem = new TabItem();
            designTabItem.Header = "Design";
            designTabItem.Background = null;
            designTabItem.BorderBrush = null;
            auditTabItem = new TabItem();
            auditTabItem.Header = "Audit";
            auditTabItem.Background = null;
            auditTabItem.BorderBrush = null;


            try
            {
                this.SpreadSheet = new DESheetPanel();
                //this.SpreadSheet.Office.CreateNew(EdrawOffice.EXCEL_ID);
                //this.SpreadSheet.BuildSheetPanelMethod();
                //image = new System.Windows.Controls.Image();
                //host = new WindowsFormsHost();
                //host.Child = SpreadSheet;
                this.Content = this.SpreadSheet;
            }
            catch (Exception) { }
        }

        

        #endregion


        #region Properties

        public AutomaticSourcingPanel AutomaticSourcingPanel { get; private set; }
        public AutomaticTablePropertiesPanel AutomaticTablePropertiesPanel { get; private set; }

        public BGroup tableGroup;

        public DESheetPanel SpreadSheet { get; private set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'InputTable en édition
        /// </summary>
        public Misp.Kernel.Domain.AutomaticSourcing EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public Kernel.Service.GroupService GroupService { get; set; }

      

        #endregion


        #region Methods

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }

        public virtual bool isAutomaticTarget()
        {
            return false;
        }

        public virtual bool isGrid()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public Misp.Kernel.Domain.AutomaticSourcing getNewObject() { return new Misp.Kernel.Domain.AutomaticSourcing(); }
              
        #endregion


        public bool validateEdition()
        {
            return true;

        }

        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.AutomaticTablePropertiesPanel.fillAutomaticSourcing(this.EditedObject);
        }

        public void SetSelectedAttribute(Kernel.Domain.Attribute attribute) 
        {
            this.AutomaticSourcingPanel.SetSelectedAttribute(attribute);
        }

        public void SetSelectedRange(bool selected) 
        {
            this.AutomaticSourcingPanel.SheetPanel.RangeCheckBox.IsChecked = selected;
        }

        public void SetSelectedRange(string selectedRange)
        {
            this.AutomaticSourcingPanel.SheetPanel.RangeTextBox.Text = selectedRange;
        }

        public void SetSelectedMeasure(Kernel.Domain.Measure measure)
        {
            this.AutomaticSourcingPanel.SetSelectedMeasure(measure);
        }

        public void DisplayMeasure()
        {
            this.AutomaticSourcingPanel.DisplayMeasure();
        }

        public void DisplayScope()
        {
            this.AutomaticSourcingPanel.DisplayScope();
        }

        public void DisplayPeriod()
        {
            this.AutomaticSourcingPanel.DisplayPeriod();
        }

        public void DisplayTag()
        {
            this.AutomaticSourcingPanel.DisplayTag();
        }


        public void SetTargetItemValue(Target target) 
        {
            this.AutomaticTablePropertiesPanel.filterScopePanel.SetTargetItemValue(target);
        }

        public void SetPeriodName(PeriodName periodName)
        {
            this.AutomaticSourcingPanel.setPeriodName(periodName);
        }

        public String GetActiveSheetName() 
        {
            return this.SpreadSheet.getActiveSheetName();
        }

        public int GetActiveSheetIndex() 
        {
            return this.SpreadSheet.getActiveSheetIndex();
        }

        public void displayObject()
        {
            this.AutomaticSourcingPanel.Display(this.EditedObject);
            if (!isAutomaticTarget())
            {
                this.AutomaticTablePropertiesPanel.Visibility = System.Windows.Visibility.Visible;
                this.AutomaticTablePropertiesPanel.displayAutomaticSourcing(this.EditedObject);
            }
            else 
            {
                this.AutomaticTablePropertiesPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void displayColumn(AutomaticSourcingColumn column, List<AutomaticSourcingColumn> liste) 
        {
            this.AutomaticSourcingPanel.DisplayColumn(column,liste);
        }

        public void displayColumns(List<AutomaticSourcingColumn> list)
        {
            this.AutomaticSourcingPanel.displayColumns(list);
        }

        public void displaySheet(AutomaticSourcingSheet sheet) 
        {
            this.AutomaticSourcingPanel.displaySheet(sheet);
        }

        public void resetView() 
        {
            this.AutomaticSourcingPanel.Display(null);
            this.AutomaticTablePropertiesPanel.displayAutomaticSourcing(null);
        }
             

        public int GetSelectedListIndex() 
        {
            return this.AutomaticSourcingPanel.GetSelectedListIndex();
        }

        public void SetSelectedIndex(int index) 
        {
            this.AutomaticSourcingPanel.SetSelectedIndex(index);
        }

        public bool GetSelectionRangeState() 
        {
            return this.AutomaticSourcingPanel.GetSelectionRangeState();
        }

        public Range GetUsableRange(Range range,bool selectedRange,bool firstRow) 
        {
          // return this.SpreadSheet.getUsableRange(range,selectedRange,firstRow);
            return null;
        }

        public int GetActiveColumnIndex() 
        {
            return this.SpreadSheet.getActiveCell().Column;
        }

        public AutomaticSourcingColumn GetSelectedColumn() 
        {
          return  this.AutomaticSourcingPanel.GetSelectedColumn();
        }

        public void DisplayAllocationData(CellPropertyAllocationData data) 
        {
            this.AutomaticSourcingPanel.DisplayAllocationData(data);
        }
        
        public List<int> GetListBoxItems() 
        {
           return this.AutomaticSourcingPanel.GetListBoxItems();
        }

        public int getColumnInListBox(int columnIndex)
        {
            return this.AutomaticSourcingPanel.getColumnInListBox(columnIndex);
        }



        public string getTargetGroupName()
        {
            return this.AutomaticSourcingPanel.getTargetGroupName();
        }

        public String GetSelectedRange() 
        {
            return this.AutomaticSourcingPanel.SheetPanel.RangeTextBox.Text;
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(AutomaticTablePropertiesPanel.getEditableControls());
            
            controls.AddRange(AutomaticSourcingPanel.getEditableControls());
            if (this.SpreadSheet != null) controls.Add(this.SpreadSheet);
            return controls;
        }

        bool isMasked = false;
        public void Mask(bool mask)
        {
            if (mask)
            {
                if (isMasked) return;
                //image.Source = GetScreenInt();
                //image.Visibility = System.Windows.Visibility.Visible;
                //host.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                //image.Visibility = System.Windows.Visibility.Hidden;
                //host.Visibility = System.Windows.Visibility.Visible;
            }
            isMasked = mask;
        }

        public BitmapSource GetScreenInt()
        {
            //Bitmap bm = new Bitmap(SpreadSheet.ClientRectangle.Width, SpreadSheet.ClientRectangle.Height);
            //Graphics g = Graphics.FromImage(bm);
            //PrintWindow(SpreadSheet.Handle, g.GetHdc(), 0);
            //g.ReleaseHdc(); g.Flush();
            //BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            //src.Freeze();
            //bm.Dispose();
            //bm = null;
            //return src;
            return null;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);






    }
}
