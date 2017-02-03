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
using System.Threading;
using Misp.Kernel.Ui.Office.DevExpressSheet;

namespace Misp.Sourcing.Designer
{
    public class DesignerForm : UserControl, IEditableView<Design>
    {

        #region Properties
        
        public static int CENTRAL_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);
        public static int ROWS_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
        public static int COLUMNS_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightCoral);
        public static int NULL_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

        public static int TOTAL_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);

        #endregion
        
        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public DesignerForm()
        {
            InitializeComponents();
        }
        
        protected virtual void InitializeComponents()
        {
            DesignerPropertiesPanel = new DesignerPropertiesPanel();

            this.Background = Brushes.White;
            this.BorderBrush = Brushes.White; 

            Uri rd1 = new Uri("../Resources/Styles/TabControl.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(Application.LoadComponent(rd1) as ResourceDictionary);     
            
            try
            {
                this.SpreadSheet = new DESheetPanel();
                this.SpreadSheet.CreateNewExcelFile();                
                this.Content = SpreadSheet;
            }
            catch (Exception) { }
        }


        #endregion
        
        #region Properties

        public bool IsReadOnly { get; set; }
        
        public DesignerPropertiesPanel DesignerPropertiesPanel { get; private set; }
        
        public DESheetPanel SpreadSheet { get; private set; }

        public Periodicity periodicity { get; set; }
        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }
        
        /// <summary>
        /// Design en édition
        /// </summary>
        public Design EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public Kernel.Service.GroupService GroupService { get; set; }

        public Kernel.Service.DesignService DesignService { get; set; }

        #endregion
        
        #region Methods

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
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
        public Design getNewObject() 
        {
            Design design = new Design();
            return design; 
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return DesignerPropertiesPanel.validateEdition() ;
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.DesignerPropertiesPanel.FillDesign(this.EditedObject);
        }
     
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            if (periodicity == null)
            {
                periodicity = Misp.Kernel.Application.ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetPeriodicityService().getPeriodicity();
            }
            this.DesignerPropertiesPanel.Display(this.EditedObject);
            BuildSheetTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            return controls;
        }

        public void BuildSheetTable()
        {
            fillObject();
            BuildSheetTableWithoutFill();
        }

        public void BuildSheetTableWithoutFill()
        {
            Design design = this.EditedObject;
            if (design == null) return;
            ClearSheet();
            BuildCentralPart();
            BuildColunms();
            BuildRows();
            BuildTotals();
        }

        protected void ClearSheet()
        {
            string sheetName = this.SpreadSheet.getActiveSheetName();
            this.SpreadSheet.ClearSheet(sheetName);
        }
        
        /// <summary>
        /// Contruit la partie commune
        /// </summary>
        protected void BuildCentralPart()
        {
            Design design = this.EditedObject;            
            string sheetName = this.SpreadSheet.getActiveSheetName();
            string value = "";
            string cont = "";
            foreach (DesignDimensionLine line in design.central.lineListChangeHandler.Items)
            {
                foreach (LineItem item in line.itemListChangeHandler.Items)
                {
                    value += cont + item.GetValue().ToString();
                    cont = " ; ";
                }
            }

            if (!string.IsNullOrEmpty(value))
            {
                this.SpreadSheet.SetValueAt(1, 1, sheetName, value, Designer.DesignerForm.CENTRAL_COLOR);
            }
            else this.SpreadSheet.SetValueAt(1, 1, sheetName, "", NULL_COLOR);
        }

        private void BuildTotals()
        {
            Design design = this.EditedObject;
            bool concatH = design.concatenateColumnHearder;
            bool concatR = design.concatenateRowHearder;
            int columnCount = getCartesianProductLineSize(new ObservableCollection<DesignDimensionLine>(design.columns.lineListChangeHandler.Items));
            int rowCount = getCartesianProductLineSize(new ObservableCollection<DesignDimensionLine>(design.rows.lineListChangeHandler.Items));
            columnCount = columnCount <= 0 ? 1 : columnCount;
            rowCount = rowCount <= 0 ? 1 : rowCount;

            int rowSize = concatR ? 1 : design.rows.lineListChangeHandler.Items.Count;
            int colSize = concatH ? 1 : design.columns.lineListChangeHandler.Items.Count;

            string sheetName = this.SpreadSheet.getActiveSheetName();

            if (design.addTotalColumnRight)
            {
                int row = colSize;
                int col = columnCount + rowSize + 1;
                this.SpreadSheet.SetValueAt(row, col, sheetName, "TOTAL", Designer.DesignerForm.TOTAL_COLOR);                
            }

            if (design.addTotalRowBelow)
            {
                int row = rowCount + colSize  + 1;
                int col = rowSize;
                this.SpreadSheet.SetValueAt(row, col, sheetName, "TOTAL", Designer.DesignerForm.TOTAL_COLOR);                
            }
        }



        /// <summary>
        /// Build columns
        /// </summary>
        public void BuildColunms()
        {
            Design design = this.EditedObject;            
            string sheetName = this.SpreadSheet.getActiveSheetName();
            
            string value = "";
            string cont = "";
            int row = 1, col = 1;
            bool concat = design.concatenateColumnHearder;

            ObservableCollection<DesignDimensionLine> Lines = new ObservableCollection<DesignDimensionLine>(design.columns.lineListChangeHandler.Items);
            int lineCount = Lines.Count;
            if (lineCount > 0)
            {
                int totalCardinality = getCartesianProductLineSize(Lines);
                row = lineCount;
                for (int currentLineRang = lineCount; currentLineRang >= 1; currentLineRang--)
                {
                    col = design.rows.lineListChangeHandler.Items.Count > 0 ? design.rows.lineListChangeHandler.Items.Count + 1 : 2;
                    if (design.concatenateRowHearder) col = 2;
                    DesignDimensionLine currentLine = Lines[currentLineRang - 1];
                    int currentLineItemCount = currentLine.GetItemCount();

                    ObservableCollection<DesignDimensionLine> AllLines = new ObservableCollection<DesignDimensionLine>(design.columns.lineListChangeHandler.Items);
                    AllLines.Remove(currentLine);
                    int cardinality = getCartesianProductLineSize(AllLines);

                    
                    
                    if (currentLineRang == 1 && currentLine.GetItemCount() != 0)
                    {
                        for (int j = 1; j <= totalCardinality / currentLine.GetItemCount(); j++)
                        {
                            foreach (LineItem item in currentLine.itemListChangeHandler.Items)
                            {
                                value = item.GetValue().ToString();
                                
                                if (!concat)
                                {
                                    this.SpreadSheet.SetValueAt(row, col, sheetName, value, Designer.DesignerForm.COLUMNS_COLOR);                                    
                                }
                                else
                                {
                                    object excelValue = this.SpreadSheet.getValueAt(row, col, sheetName);
                                    if (excelValue == null) excelValue = "";
                                    if (string.IsNullOrEmpty(excelValue.ToString())) cont = ""; else cont = " ; ";
                                    excelValue = "" + value + cont + excelValue;
                                    this.SpreadSheet.SetValueAt(row, col, sheetName, excelValue.ToString(), Designer.DesignerForm.COLUMNS_COLOR);
                                }
                                col++;
                            }
                        }
                    }

                    else
                    {
                        int n = totalCardinality - (currentLineItemCount * cardinality) + 1;
                        int m = cardinality;
                        if (currentLineRang != lineCount)
                        {
                            ObservableCollection<DesignDimensionLine> AllLine = design.columns.lineListChangeHandler.Items;
                            ObservableCollection<DesignDimensionLine> UPLines = new ObservableCollection<DesignDimensionLine>(AllLine.ToList().GetRange(0, currentLineRang - 1));
                            ObservableCollection<DesignDimensionLine> DOWNLines = new ObservableCollection<DesignDimensionLine>(AllLine.ToList().GetRange(currentLineRang, AllLine.Count - currentLineRang));

                            m = getCartesianProductLineSize(UPLines);
                            n = getCartesianProductLineSize(DOWNLines);
                        }

                        for (int j = 1; j <= n; j++)
                        {
                            foreach (LineItem item in currentLine.itemListChangeHandler.Items)
                            {
                                for (int i = 1; i <= m; i++)
                                {
                                    value = item.GetValue().ToString();
                                    if (!concat)
                                    {
                                        this.SpreadSheet.SetValueAt(row, col, sheetName, value, Designer.DesignerForm.COLUMNS_COLOR);
                                    }
                                    else
                                    {
                                        object excelValue = this.SpreadSheet.getValueAt(1, col, sheetName);
                                        if (excelValue == null) excelValue = "";
                                        if (string.IsNullOrEmpty(excelValue.ToString())) cont = ""; else cont = " ; ";
                                        excelValue = "" + value + cont + excelValue;
                                        this.SpreadSheet.SetValueAt(row, col, sheetName, excelValue.ToString(), Designer.DesignerForm.COLUMNS_COLOR);                                        
                                    }
                                    col++;
                                }
                            }
                        }
                    }
                    row--;
                }
            }
         
        }

        
        /// <summary>
        /// Build rows
        /// </summary>
        public void BuildRows()
        {
            Design design = this.EditedObject;
            string sheetName = this.SpreadSheet.getActiveSheetName();
            string value = "";
            string cont = "";
            int row = 1, col = 1;
            bool concat = design.concatenateRowHearder;

            ObservableCollection<DesignDimensionLine> Lines = new ObservableCollection<DesignDimensionLine>(design.rows.lineListChangeHandler.Items);
            int lineCount = Lines.Count;
            if (lineCount > 0)
            {

                int totalCardinality = getCartesianProductLineSize(Lines);
                col = lineCount;

                for (int currentLineRang = lineCount; currentLineRang >= 1; currentLineRang--)
                {
                    row = design.columns.lineListChangeHandler.Items.Count > 0 ? design.columns.lineListChangeHandler.Items.Count + 1 : 2;
                    if (design.concatenateColumnHearder) row = 2;
                    DesignDimensionLine currentLine = Lines[currentLineRang - 1];
                    int currentLineItemCount = currentLine.GetItemCount();

                    ObservableCollection<DesignDimensionLine> AllLines = new ObservableCollection<DesignDimensionLine>(design.rows.lineListChangeHandler.Items);
                    AllLines.Remove(currentLine);
                    int cardinality = getCartesianProductLineSize(AllLines);
                    
                    if (currentLineRang == 1 && currentLine.GetItemCount() != 0)
                    {
                        for (int j = 1; j <= totalCardinality / currentLine.GetItemCount(); j++)
                        {
                            foreach (LineItem item in currentLine.itemListChangeHandler.Items)
                            {
                                value = item.GetValue().ToString();                                
                                if (!concat)
                                {
                                    this.SpreadSheet.SetValueAt(row, col, sheetName, value, Designer.DesignerForm.ROWS_COLOR);
                                }
                                else
                                {
                                    object excelValue = this.SpreadSheet.getValueAt(row, col, sheetName);
                                    if (excelValue == null) excelValue = "";
                                    if (string.IsNullOrEmpty(excelValue.ToString())) cont = ""; else cont = " ; ";
                                    excelValue = "" + value + cont + excelValue;
                                    this.SpreadSheet.SetValueAt(row, col, sheetName, excelValue.ToString(), Designer.DesignerForm.ROWS_COLOR);                                    
                                }

                                row++;
                            }
                        }
                    }

                    else
                    {
                        int n = totalCardinality - (currentLineItemCount * cardinality) + 1;
                        int m = cardinality;
                        if (currentLineRang != lineCount)
                        {
                            ObservableCollection<DesignDimensionLine> AllLine = design.rows.lineListChangeHandler.Items;
                            ObservableCollection<DesignDimensionLine> UPLines = new ObservableCollection<DesignDimensionLine>(AllLine.ToList().GetRange(0, currentLineRang - 1));
                            ObservableCollection<DesignDimensionLine> DOWNLines = new ObservableCollection<DesignDimensionLine>(AllLine.ToList().GetRange(currentLineRang, AllLine.Count - currentLineRang));

                            m = getCartesianProductLineSize(UPLines);
                            n = getCartesianProductLineSize(DOWNLines);
                        }

                        for (int j = 1; j <= n; j++)
                        {
                            foreach (LineItem item in currentLine.itemListChangeHandler.Items)
                            {
                                for (int i = 1; i <= m; i++)
                                {
                                    value = item.GetValue().ToString();
                                    if (!concat)
                                    {
                                        this.SpreadSheet.SetValueAt(row, col, sheetName, value.ToString(), Designer.DesignerForm.ROWS_COLOR);
                                    }
                                    else
                                    {
                                        object excelValue = this.SpreadSheet.getValueAt(row, 1, sheetName);
                                        if (excelValue == null) excelValue = "";
                                        if (string.IsNullOrEmpty(excelValue.ToString())) cont = ""; else cont = " ; ";
                                        excelValue = "" + value + cont + excelValue;
                                        this.SpreadSheet.SetValueAt(row, col, sheetName, excelValue.ToString(), Designer.DesignerForm.ROWS_COLOR);
                                    }
                                    row++;
                                }
                            }
                        }
                    }
                    col--;
                }
            }
            
        }

        protected int getCartesianProductLineSize(ObservableCollection<DesignDimensionLine> lines)
        {
            int size = 1;
            foreach (DesignDimensionLine line in lines)
            {
                size = size * line.GetItemCount();
            }
            return size;
        }

        #endregion


    }
}
