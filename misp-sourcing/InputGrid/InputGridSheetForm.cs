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
using Misp.Sourcing.GridViews;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridSheetForm : UserControl, IEditableView<Grille>
    {

        #region Properties
        
        public static int COLUMNS_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightCoral);
        
        public InputGridPropertiesPanel InputGridPropertiesPanel { get; private set; }

        public InputGridRelationshipPanel InputGridRelationshipPanel { get; private set; }
        
        public GridBrowser GridBrowser { get; private set; }

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
            InputGridRelationshipPanel = new InputGridRelationshipPanel();

            this.Background = System.Windows.Media.Brushes.White;
            this.BorderBrush = System.Windows.Media.Brushes.White; 

            Uri rd1 = new Uri("../Resources/Styles/TabControl.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(Application.LoadComponent(rd1) as ResourceDictionary);
            this.GridBrowser = new GridBrowser();
            this.Content = this.GridBrowser;
        }
               

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
            this.InputGridRelationshipPanel.FillGrid(this.EditedObject);
        }
     
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            this.InputGridPropertiesPanel.Display(this.EditedObject);
            this.InputGridRelationshipPanel.Display(this.EditedObject);
            BuildColunms();
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

        /// <summary>
        /// Build columns
        /// </summary>
        public void BuildColunms()
        {
            this.GridBrowser.buildColumns(this.EditedObject);
        }


        #endregion


    }
}
