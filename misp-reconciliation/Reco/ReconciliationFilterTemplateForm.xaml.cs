using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
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

namespace Misp.Reconciliation.Reco
{
    /// <summary>
    /// Interaction logic for ReconciliationFilterTemplateForm.xaml
    /// </summary>
    public partial class ReconciliationFilterTemplateForm : TabControl, IEditableView<ReconciliationFilterTemplate>
    {

        #region Properties

        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'objet en édition
        /// </summary>
        public ReconciliationFilterTemplate EditedObject { get; set; }
        
        public Kernel.Service.ReconciliationFilterTemplateService ReconciliationFilterTemplateService { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }
        
        public ChangeEventHandler Changed { get; set; }

        #endregion


        #region Contollers

        public ReconciliationFilterTemplateForm()
        {
            InitializeComponent();
            UserInit();
            InitHandlers();
        }

        #endregion
        

        #region Initializations

        private void UserInit()
        {
            this.LeftGridProperties.InputGridPropertiesPanel.GroupPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.LeftGridProperties.InputGridPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Collapsed;
            this.RightGridProperties.InputGridPropertiesPanel.GroupPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.RightGridProperties.InputGridPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Collapsed;
            this.BottomGridProperties.InputGridPropertiesPanel.GroupPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.BottomGridProperties.InputGridPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion


        #region Operations

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }


        public virtual void SetTarget(Target target)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.SetTarget(target);
            }
            else if (this.SelectedIndex == 2)
            {
                this.LeftGridProperties.InputGridPropertiesPanel.SetValue(target);
            }
            else if (this.SelectedIndex == 3)
            {
                this.RightGridProperties.InputGridPropertiesPanel.SetValue(target);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(target);
            }
        }

        public virtual void SetPeriodInterval(PeriodInterval interval)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.setPeriodName(interval);
            }
            else if (this.SelectedIndex == 2)
            {
                this.LeftGridProperties.InputGridPropertiesPanel.SetValue(interval);
            }
            else if (this.SelectedIndex == 3)
            {
                this.RightGridProperties.InputGridPropertiesPanel.SetValue(interval);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(interval);
            }
        }

        public virtual void SetPeriodName(PeriodName name)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.setPeriodName(name);
            }
            else if (this.SelectedIndex == 2)
            {
                this.LeftGridProperties.InputGridPropertiesPanel.SetValue(name);
            }
            else if (this.SelectedIndex == 3)
            {
                this.RightGridProperties.InputGridPropertiesPanel.SetValue(name);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(name);
            }
        }

        public virtual void SetMeasure(Measure measure)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.setMeasure(measure);
            }
            else if (this.SelectedIndex == 2)
            {
                this.LeftGridProperties.InputGridPropertiesPanel.SetValue(measure);
            }
            else if (this.SelectedIndex == 3)
            {
                this.RightGridProperties.InputGridPropertiesPanel.SetValue(measure);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(measure);
            }
        }
                
        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public ReconciliationFilterTemplate getNewObject()
        {
            return new ReconciliationFilterTemplate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
            //this.InputGridSheetForm.SetChangeEventHandler(ChangeEventHandler);
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return true;
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public virtual void fillObject()
        {
            //this.GridForm.fillObject();
            //this.InputGridSheetForm.fillObject();
            if (this.EditedObject == null) this.EditedObject = new ReconciliationFilterTemplate();
            this.EditedObject.writeOffConfig = ConfigurationPanel.WriteOffConfigPanel.fillObject();
        }

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public virtual void displayObject()
        {
            this.LeftGrid.Template = this.EditedObject;
            this.RightGrid.Template = this.EditedObject;

            this.LeftGrid.EditedObject = this.EditedObject != null ? this.EditedObject.leftGrid : null;
            this.RightGrid.EditedObject = this.EditedObject != null ? this.EditedObject.rigthGrid : null;
            this.BottomGrid.EditedObject = this.EditedObject != null ? this.EditedObject.bottomGrid : null;
            this.LeftGrid.displayObject();
            this.RightGrid.displayObject();
            this.BottomGrid.displayObject();
            this.ConfigurationPanel.EditedObject = this.EditedObject;
            this.ConfigurationPanel.displayObject();

            this.LeftGridProperties.EditedObject = this.EditedObject != null ? this.EditedObject.leftGrid : null ;
            this.RightGridProperties.EditedObject = this.EditedObject != null ? this.EditedObject.rigthGrid : null;
            this.BottomGridProperties.EditedObject = this.EditedObject != null ? this.EditedObject.bottomGrid : null;
            this.LeftGridProperties.displayObject();
            this.RightGridProperties.displayObject();
            this.BottomGridProperties.displayObject();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            return new List<object>(0);
        }

        #endregion


        #region Handlers

        private void InitHandlers()
        {
            this.LeftGridProperties.InputGridPropertiesPanel.Changed += OnLeftGridPropertiesChange;
            this.RightGridProperties.InputGridPropertiesPanel.Changed += OnRightGridPropertiesChange;
            this.BottomGridProperties.InputGridPropertiesPanel.Changed += OnBottomGridPropertiesChange;

            this.LeftGrid.Changed += OnChange;
            this.RightGrid.Changed += OnChange;
        }
        
        private void OnBottomGridPropertiesChange(object item)
        {
            this.BottomGridProperties.BuildColunms();
            this.BottomGrid.GridBrowser.RebuildGrid = true;
        }

        private void OnRightGridPropertiesChange(object item)
        {
            this.RightGridProperties.BuildColunms();
            this.RightGrid.GrilleBrowserForm.gridBrowser.RebuildGrid = true;
        }

        private void OnLeftGridPropertiesChange(object item)
        {
            this.LeftGridProperties.BuildColunms();
            this.LeftGrid.GrilleBrowserForm.gridBrowser.RebuildGrid = true;
        }

        public void OnChange()
        {
            if (Changed != null) Changed();
        }

        #endregion

    }
}
