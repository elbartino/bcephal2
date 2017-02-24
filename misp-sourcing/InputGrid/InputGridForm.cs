using Misp.Kernel.Administration.ObjectAdmin;
using Misp.Kernel.Administration.Profil;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.GridViews;
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

namespace Misp.Sourcing.InputGrid
{
    /// <summary>
    /// Interaction logic for InputGridForm.xaml
    /// </summary>
    public class InputGridForm : TabControl, IEditableView<Grille>
    {

        #region Properties

        public bool IsReadOnly { get; set; }

        public SubjectType SubjectType { get; set; }

        public TabItem AuditTabItem;
        public TabItem ConfigurationTabItem;
        public GrilleBrowserForm GridForm;
        public InputGridSheetForm InputGridSheetForm;

        public AdministrationBar AdministrationBar { get; set; }

        #endregion

        public InputGridForm(SubjectType SubjectType)
        {
            this.SubjectType = SubjectType;
            InitializeComponent();
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }

        public virtual void SetTarget(Target target)
        {
            this.GridForm.filterForm.targetFilter.SetTargetValue(target);
        }

        public virtual void SetPeriodInterval(PeriodInterval interval)
        {
            this.GridForm.filterForm.periodFilter.SetPeriodInterval(interval);
        }

        public virtual void SetPeriodItemName(string name)
        {
            this.GridForm.filterForm.periodFilter.SetPeriodItemName(name);
        }

        protected virtual void InitializeComponent()
        {
            this.Background = Brushes.White;
            this.TabStripPlacement = Dock.Bottom;
            AuditTabItem = new TabItem();
            ConfigurationTabItem = new TabItem();
            GridForm = new GrilleBrowserForm(this.SubjectType);
            InputGridSheetForm = new InputGridSheetForm(this.SubjectType);

            AuditTabItem.Header = "Grid";
            AuditTabItem.Background = Brushes.White;
            AuditTabItem.Content = GridForm;

            ConfigurationTabItem.Header = "Configuration";
            ConfigurationTabItem.Background = Brushes.White;
            ConfigurationTabItem.Content = InputGridSheetForm;
            if (ApplicationManager.Instance.User.IsAdmin())
            {
                this.AdministrationBar = new AdministrationBar(this.SubjectType);
            }
            this.Items.Add(AuditTabItem);
            this.Items.Add(ConfigurationTabItem);
        }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'objet en édition
        /// </summary>
        public Grille EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public Grille getNewObject()
        {
            return this.InputGridSheetForm.getNewObject();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
            this.InputGridSheetForm.SetChangeEventHandler(ChangeEventHandler);
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return this.InputGridSheetForm.validateEdition();
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public virtual void fillObject()
        {
            this.GridForm.fillObject();
            this.InputGridSheetForm.fillObject();
        }

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public virtual void displayObject()
        {
            this.InputGridSheetForm.EditedObject = this.EditedObject;
            this.InputGridSheetForm.displayObject();

            this.GridForm.EditedObject = this.EditedObject;
            this.GridForm.displayObject();

            if (this.AdministrationBar != null)
            {
                this.AdministrationBar.EditedObject = this.EditedObject;
                this.AdministrationBar.Display();
            }
        }

        public virtual void displayObjectInGridForm()
        {
            this.GridForm.EditedObject = this.EditedObject;
            this.GridForm.displayObject();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            //controls.AddRange(userRightPanel.getEditableControls());
            controls.AddRange(this.InputGridSheetForm.getEditableControls());
            return controls;

        }
    }
}
