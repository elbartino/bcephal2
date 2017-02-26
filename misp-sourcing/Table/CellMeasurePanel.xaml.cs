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

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for CellMeasurePanel.xaml
    /// </summary>
    public partial class CellMeasurePanel : Grid
    {

        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement, notament lorsqu'on set la valeur du TargetItem.
        /// </summary>
        public event ChangeEventHandler Changed;

        /// <summary>
        /// 
        /// </summary>
        public event AddEventHandler Added;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateEventHandler Updated;
        
        public event ValidateFormulaEventHandler ValidateFormula;

        #endregion


        #region Constructor

        public bool IsExpanded { get; set; }

        /// <summary>
        /// Construit une nouvelle instance de CellMeasurePanel
        /// </summary>
        public CellMeasurePanel()
        {
            InitializeComponent();            
            InitializeHandlers();
            Expand(false);
        }

        public void CustomizeForLoopCondition()
        {
            this.formulalabel.Visibility = Visibility.Collapsed;
            this.formulaTextBox.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Construit une nouvelle instance de CellMeasurePanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">CellMeasure à afficher</param>
        public CellMeasurePanel(CellMeasure item, bool forReport) : this()
        {
            Display(item);
        }

        #endregion


        #region Properties

        public CellMeasure CellMeasure { get; set; }

        public TextBox ValueTextBox { get { return this.valueTextBox; } }
        public TextBox FormulaTextBox { get { return this.formulaTextBox; } }

        #endregion


        #region Operations

        public void SetReadOnly(bool readOnly)
        {
            this.formulaTextBox.IsReadOnly = readOnly;
        }

        public void Expand(bool expand){
            IsExpanded = expand;
            if (expand)
            {
                FormulaCol.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                FormulaCol.Width = new GridLength(0, GridUnitType.Star);
            }
        }

        public void Display(CellMeasure item,bool readOnly=false)
        {
            update = false;
            this.CellMeasure = item;
            this.ValueTextBox.Text = item != null ? item.measure != null ? item.measure.name : item.name: "";
            this.FormulaTextBox.Text = item != null && item.formula != null ? item.formula : "";
            this.FormulaTextBox.IsEnabled = !readOnly;
            update = true;
        }

        public void DisplayMeasureName(string name)
        {
            this.ValueTextBox.Text = name != null ? name : "";
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetValue(CellMeasure item)
        {
            bool added = false;
            if (this.CellMeasure == null)
            {
                this.CellMeasure = new CellMeasure();                
                added = true;
            }
            this.CellMeasure.name = item.name;
            this.CellMeasure.measure = item.measure;
            //this.CellMeasure.formula = item.formula;

            this.ValueTextBox.Text = this.CellMeasure != null ? this.CellMeasure.measure.name : "";
            //this.FormulaTextBox.Text = this.CellMeasure != null ? this.CellMeasure.formula : "";
            if (Added != null && added) Added(this);
            if (Updated != null && !added) Updated(this);
        }
        
        #endregion


        #region Handlers

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.FormulaTextBox.KeyDown += OnValidateFormula;
        }
        
        private void OnValidateFormula(object sender, KeyEventArgs e)
        {
            if (this.CellMeasure == null)
            {
                this.CellMeasure = new CellMeasure();
            }
            if (e.Key == Key.Enter && ValidateFormula != null) ValidateFormula(this);
        }

        bool update = true;
       
        
        #endregion

    }
}
