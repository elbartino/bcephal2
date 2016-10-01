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
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.ObjectModel;
using Misp.Kernel.Util;
using Misp.Kernel.Ui.Office.SyncFusion;
namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for InputTableForm.xaml
    /// </summary>
    public class InputTableForm : TabControl, IEditableView<InputTable>
    {

        #region Properties
        
        public MenuItem RunMenuItem;
        public MenuItem ClearMenuItem;
        public CheckBox ApplyToAllMenuItem;

        public MenuItem AuditMenuItem;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public InputTableForm()
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
        WindowsFormsHost windowsFormsHost;
        System.Windows.Controls.Image image;


        protected virtual void InitializeComponents()
        {
            this.Background = null;
            this.BorderBrush = null;
            this.TabStripPlacement = Dock.Bottom;

            ItemsPanelTemplate temp = this.ItemsPanel;
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

            this.TablePropertiesPanel = new TablePropertiesPanel();
            this.TableCellParameterPanel = new TableCellParameterPanel();
            this.CellPropertyGrid = new CellPropertyGrid();
            this.CellPropertyGrid.hideContextMenu();

            this.SpreadSheet = new SyncFusionSheet();
            designTabItem.Content = this.SpreadSheet;

            auditTabItem.Content = CellPropertyGrid;
            this.Items.Add(designTabItem);
            this.Items.Add(auditTabItem);
         
            this.SelectionChanged += onSelectTabChancged;
        }


        bool isMasked = false;
        public void Mask(bool mask)
        {
            //if (mask)
            //{
            //    if (isMasked) return;
            //    image.Source = GetScreenInt();
            //    image.Visibility = System.Windows.Visibility.Visible;
            //    windowsFormsHost.Visibility = System.Windows.Visibility.Hidden;
            //}
            //else
            //{
            //    image.Visibility = System.Windows.Visibility.Hidden;
            //    windowsFormsHost.Visibility = System.Windows.Visibility.Visible;
            //}
            //isMasked = mask;
        }

        //public BitmapSource GetScreenInt()
        //{
        //    Bitmap bm = new Bitmap(SpreadSheet.ClientRectangle.Width, SpreadSheet.ClientRectangle.Height);
        //    Graphics g = Graphics.FromImage(bm);
        //    PrintWindow(SpreadSheet.Handle, g.GetHdc(), 0);
        //    g.ReleaseHdc(); g.Flush();
        //    BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        //    src.Freeze();
        //    bm.Dispose();
        //    bm = null;
        //    return src;
        //}

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        bool sendRequest = true;
        protected void onSelectTabChancged(object sender, SelectionChangedEventArgs e)
        {
            if (auditTabItem.IsSelected)
            {
                if (InputTableService != null && sendRequest)
                {
                    List<CellProperty> cells = InputTableService.getCellsValues(this.EditedObject.name);
                    if (cells == null) cells = new List<CellProperty>(0);
                    foreach (CellProperty cellProperty in cells) completeCell(cellProperty);
                    this.CellPropertyGrid.ItemsSource = new ObservableCollection<CellProperty>(cells);
                    sendRequest = false;
                    return;
                }
            }
            else
            {
                sendRequest = true;
            }
        }

        protected void completeCell(CellProperty cellProperty){
            if (cellProperty.cellScope != null){
                cellProperty.cellScope.targetItemListChangeHandler.Items = cellProperty.cellScope.targetItemListChangeHandler.getItems();
                foreach (TargetItem item in cellProperty.cellScope.targetItemListChangeHandler.Items)
                {
                    if (TagFormulaUtil.isFormula(item.formula) && item.value == null)
                    {
                        System.Windows.Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(item.formula));
                        item.value = new Target();
                        item.value.name = GetValue((int)coord.X, (int)coord.Y, cellProperty.nameSheet);
                    }
                    cellProperty.cellScope.buildName();
                }
            }
            
            if (cellProperty.cellMeasure != null && TagFormulaUtil.isFormula(cellProperty.cellMeasure.formula) && cellProperty.cellMeasure.measure == null)
            {
                if (TagFormulaUtil.isFormula(cellProperty.cellMeasure.formula) && cellProperty.cellMeasure.measure == null)
                {
                    System.Windows.Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(cellProperty.cellMeasure.formula));
                    String measureName = GetValue((int)coord.X, (int)coord.Y, cellProperty.nameSheet);
                    cellProperty.cellMeasure.name = measureName;
                }
            }

            if(cellProperty.period != null){
                cellProperty.period.itemListChangeHandler.Items = cellProperty.period.itemListChangeHandler.getItems();
                foreach (PeriodItem item in cellProperty.period.itemListChangeHandler.Items)
                {
                    if (TagFormulaUtil.isFormula(item.formula) && item.value == null)
                    {
                        System.Windows.Point coord = TagFormulaUtil.getCoordonne(TagFormulaUtil.getFormulaWithoutEqualSign(item.formula));
                        item.value = GetValue((int)coord.X, (int)coord.Y, cellProperty.nameSheet);
                    }
                    cellProperty.period.BuildName();
                }
            }

        }


        protected string GetValue(int colFormula, int rowFormula, string sheetName)
        {
            object value = SpreadSheet.getValueAt(rowFormula, colFormula, sheetName);
            return value != null ? value.ToString() : "";
        }

        #endregion

        #region Properties

        public TablePropertiesPanel TablePropertiesPanel { get; private set; }

        public TableCellParameterPanel TableCellParameterPanel { get; private set; }

        //public EdrawOffice SpreadSheet { get;  set; }

        public SyncFusionSheet SpreadSheet { get; set; }

        public CellPropertyGrid CellPropertyGrid { get; private set; }

        public InputTableService InputTableService { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'InputTable en édition
        /// </summary>
        public InputTable EditedObject { get; set; }

        public GroupProperty GroupProperty { get; set; }
        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public Kernel.Service.GroupService GroupService { get; set; }

        #endregion


        #region Methods

        protected virtual bool isReport()
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
        public InputTable getNewObject() { return new InputTable(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            bool valid = this.TablePropertiesPanel.validateEdition();
            return valid;
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.SpreadSheet.DocumentUrl = this.EditedObject.excelFileName;
            this.TablePropertiesPanel.fillTable(this.EditedObject);
        }
         /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            this.TableCellParameterPanel.allocationPanel.FillAllocationData();
            bool isNoAllocation = !isReport() && this.TableCellParameterPanel.allocationPanel.AllocationData.type ==
                CellPropertyAllocationData.AllocationType.NoAllocation.ToString();
            this.TablePropertiesPanel.displayTable(this.EditedObject,isNoAllocation);
            this.CellPropertyGrid.ItemsSource = this.EditedObject.cellPropertyListChangeHandler.Items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(TablePropertiesPanel.getEditableControls());
            controls.AddRange(TableCellParameterPanel.getEditableControls());
            if (this.SpreadSheet != null) controls.Add(this.SpreadSheet);
            return controls;
        }

        #endregion


    }
}
