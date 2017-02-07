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

        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'objet en édition
        /// </summary>
        public ReconciliationFilterTemplate EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public ReconciliationFilterTemplateForm()
        {
            InitializeComponent();
            this.LeftGrid.HideRecoToolBar();
            this.RightGrid.HideRecoToolBar();
            this.BottomGrid.HideHeaderPanel();
        }


        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }


        public virtual void SetTarget(Target target)
        {
            //this.GridForm.filterForm.targetFilter.SetTargetValue(target);
        }

        public virtual void SetPeriodInterval(PeriodInterval interval)
        {
            //this.GridForm.filterForm.periodFilter.SetPeriodInterval(interval);
        }

        public virtual void SetPeriodName(PeriodName name)
        {
            //this.GridForm.filterForm.periodFilter.SetPeriodItemName(name);
        }

        public virtual void SetMeasure(Measure measure)
        {
            //this.GridForm.filterForm.periodFilter.SetPeriodItemName(name);
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
        }

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public virtual void displayObject()
        {
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            return new List<object>(0);
        }


    }
}
