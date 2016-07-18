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
using System.Runtime.InteropServices;
using System.Drawing;
using Misp.Kernel.Service;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridSheetForm : UserControl, IEditableView<Grille>
    {

        #region Properties
        
        public static int COLUMNS_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightCoral);
        
        public InputGridPropertiesPanel InputGridPropertiesPanel { get; private set; }

        public SheetPanel SpreadSheet { get; private set; }

        public Periodicity periodicity { get; set; }
        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// Design en édition
        /// </summary>
        public Grille EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public Kernel.Service.GroupService GroupService { get; set; }

        public InputGridService InputGridService { get; set; }

        #endregion
        
        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public InputGridSheetForm()
        {
            InitializeComponents();
        }
        
        protected virtual void InitializeComponents()
        {
            InputGridPropertiesPanel = new InputGridPropertiesPanel();

            this.Background = System.Windows.Media.Brushes.White;
            this.BorderBrush = System.Windows.Media.Brushes.White; 

            Uri rd1 = new Uri("../Resources/Styles/TabControl.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(Application.LoadComponent(rd1) as ResourceDictionary);
            
            try
            {
                windowsFormsHost = new WindowsFormsHost();
                this.SpreadSheet = new SheetPanel();
                this.SpreadSheet.CreateNewExcelFile();
                this.SpreadSheet.BuildSheetPanelMethod();
                this.SpreadSheet.RemoveTempFiles();
                windowsFormsHost.Child = SpreadSheet;

                image = new System.Windows.Controls.Image();
                Grid grid = new Grid();
                grid.Children.Add(windowsFormsHost);
                grid.Children.Add(image);
                image.Visibility = System.Windows.Visibility.Hidden;
                windowsFormsHost.Visibility = System.Windows.Visibility.Visible;
                this.Content = grid;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e);
            }
        }

        WindowsFormsHost windowsFormsHost;
        System.Windows.Controls.Image image;

        bool isMasked = false;
        public void Mask(bool mask)
        {
            if (mask)
            {
                if (isMasked) return;
                image.Source = GetScreenInt();
                image.Visibility = System.Windows.Visibility.Visible;
                windowsFormsHost.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                image.Visibility = System.Windows.Visibility.Hidden;
                windowsFormsHost.Visibility = System.Windows.Visibility.Visible;
            }
            isMasked = mask;
        }

        public BitmapSource GetScreenInt()
        {
            Bitmap bm = new Bitmap(SpreadSheet.ClientRectangle.Width, SpreadSheet.ClientRectangle.Height);
            Graphics g = Graphics.FromImage(bm);
            PrintWindow(SpreadSheet.Handle, g.GetHdc(), 0);
            g.ReleaseHdc(); g.Flush();
            BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            src.Freeze();
            bm.Dispose();
            bm = null;
            return src;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags); 


        #endregion
        
        
        #region Methods

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
        public Grille getNewObject() 
        {
            Grille grid = new Grille();
            return grid; 
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return InputGridPropertiesPanel.validateEdition();
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.InputGridPropertiesPanel.FillGrid(this.EditedObject);
        }
     
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            this.InputGridPropertiesPanel.Display(this.EditedObject);
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
            //fillObject();
            //this.SpreadSheet.DisableSheet(false);
            this.SpreadSheet.protectSheet(false);
            BuildSheetTableWithoutFill();
            this.SpreadSheet.protectSheet();
            //this.SpreadSheet.DisableSheet();
        }

        public void BuildSheetTableWithoutFill()
        {
            Grille grid = this.EditedObject;
            if (grid == null) return;
            this.SpreadSheet.ClearUsedExcelCells();
            BuildColunms();
        }
        
        /// <summary>
        /// Build columns
        /// </summary>
        public void BuildColunms()
        {
            Grille grid = this.EditedObject;
            string sheetName = this.SpreadSheet.getActiveSheetName();
            int row = 1;
            int col = 1;
            foreach (GrilleColumn column in grid.columnListChangeHandler.Items)
            {
                if(!column.show) continue;
                String value = column.name;
                this.SpreadSheet.SetValueAt(row, col, value);
                this.SpreadSheet.SetColorAt(row, col++, COLUMNS_COLOR);
            }            
        }


        #endregion


    }
}
