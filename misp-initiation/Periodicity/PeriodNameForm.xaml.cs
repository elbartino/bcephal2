using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Misp.Initiation.Periodicity
{
    /// <summary>
    /// Interaction logic for PeriodNameForm.xaml
    /// </summary>
    public partial class PeriodNameForm : Grid, IEditableView<Misp.Kernel.Domain.PeriodName>
    {

        public Kernel.Domain.SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }

        public Hyperlink hyperLink;
        PeriodName editedPeriodName;
        public int selectedPeriodNamePosition;
        public event ChangeEventHandler Changed;

        /// <summary>
        /// 
        /// </summary>
        public PeriodNameForm(Kernel.Domain.SubjectType subjectType)
        {
            this.SubjectType = subjectType;
            selectedPeriodNamePosition = -1;
            InitializeComponent();
            BuildStandardPeriodLink();
            InitializeHandlers();
        }
        
        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }

        private void InitializeHandlers()
        {
            hyperLink.RequestNavigate += OnShowPeriodIntervalParams;
            this.periodTree.propertiesMenuItem.Click += OnPropertiesMenuItemClicked;
            this.periodTree.Changed += OnPeriodTreeChanged;
            this.PeriodIntervalleTree.Changed += OnPeriodIntervalTreeChanged;
            this.periodTree.tree.SelectedItemChanged += OnSelectionChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'objet en édition
        /// </summary>
        public Misp.Kernel.Domain.PeriodName EditedObject { get; set; }

        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public Misp.Kernel.Domain.PeriodName getNewObject() { return new Misp.Kernel.Domain.PeriodName(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// Et retire  la valeur par défaut des listes à envoyer dans au serveur.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition() { return true; }
        
        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject() 
        {
            if (EditedObject != null) EditedObject.ForgetChild(PeriodTree.defaultValue);
        }
        
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject() {
            this.PeriodTree.DisplayRoot(this.EditedObject);
            PeriodName name = null;
            if (selectedPeriodNamePosition > -1) name = this.EditedObject.getPeriodNameByPosition(selectedPeriodNamePosition);
            else if (this.EditedObject.childrenListChangeHandler.Items.Count > 0) name = this.EditedObject.childrenListChangeHandler.Items[0];
            if (name != null) name.IsSelected = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls() 
        {
            List<object> controls = new List<object>(0);
            controls.Add(periodTree);
            return controls;
        }

        public PeriodNameTreeView PeriodTree
        {
            get { return periodTree; } 
        }

        /// <summary>
        /// 
        /// </summary>
        private void BuildStandardPeriodLink()
        {
            NavigationToken standardPeriodToken = NavigationToken.GetCreateViewToken("STANDARD_PERIOD_FUNCTIONALITY");
            Run run = new Run("New Standard Period");
            this.hyperLink = new Hyperlink(run)
            {
                NavigateUri = new Uri("http://localhost//" + "New Standard Period"),
                DataContext = standardPeriodToken
            };
            StandardperiodTextBlock.Inlines.Add(hyperLink);
            StandardperiodTextBlock.ToolTip = "Create new standard period group";
        }

        private void OnPropertiesMenuItemClicked(object sender, RoutedEventArgs e)
        {
            showStandartPeriodEditor();
        }

        private void OnShowPeriodIntervalParams(object sender, RequestNavigateEventArgs e)
        {
            editedPeriodName = this.periodTree.GetSelectedPeriod();
            if (!editedPeriodName.name.Equals(PeriodNameTreeView.Label_DEFAULT_PERIOD) 
                && sender is Hyperlink) showStandartPeriodEditor();            
        }

        private void showStandartPeriodEditor()
        {
            StandardPeriodEditorDialog standardPeriodEditorController = new StandardPeriodEditorDialog();
            standardPeriodEditorController.OnValidateChange += OnValidateChange;
            editedPeriodName = this.periodTree.GetSelectedPeriod();
            if (editedPeriodName == null) return;
            if (editedPeriodName.IsDefault) return;
            standardPeriodEditorController.periodName = editedPeriodName;
            standardPeriodEditorController.DisplayObject();
            standardPeriodEditorController.ShowDialog();
        }

        private void OnValidateChange(object item, bool changed)
        {
            if (item is Kernel.Domain.PeriodName)
            {
                this.EditedObject.UpdateChild(editedPeriodName);
                this.PeriodIntervalleTree.DisplayPeriodInterval(editedPeriodName.GetRootPeriodInterval());
                this.periodTree.DisplayRoot(this.EditedObject);
                this.periodTree.SetSelectedPeriod(editedPeriodName);
                this.PeriodTree.tree.Items.Refresh();
                if (Changed != null) Changed();
            }
        }

        private void OnPeriodIntervalItemChanged(object item)
        {
            Kernel.Domain.PeriodInterval interval = (Kernel.Domain.PeriodInterval)item;
            this.EditedObject.UpdateChild(interval.GetRoot().periodName);
            if (Changed != null) Changed();
        }

        private void OnPeriodIntervalTreeChanged()
        {
            PeriodName periodName = this.periodTree.GetSelectedPeriod();
            if (periodName == null || periodName.IsDefault) return;
            periodName.parent.UpdateChild(periodName);
            if (Changed != null) Changed();
        }

        private void OnPeriodTreeChanged()
        {
            if (Changed != null) Changed();
        }

        private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            PeriodName periodName = this.periodTree.GetSelectedPeriod();
            if(periodName != null && !periodName.Equals(this.periodTree.defaultValue)) selectedPeriodNamePosition = periodName.position;
            this.PeriodIntervalleTree.DisplayPeriodInterval(periodName != null ? periodName.GetRootPeriodInterval() : null);
            if (periodName != null) e.Handled = true; 
        }


    }
}
