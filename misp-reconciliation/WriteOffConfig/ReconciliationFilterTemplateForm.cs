using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Misp.Reconciliation.WriteOffConfig
{
    public class ReconciliationFilterTemplateForm : UserControl, IEditableView<Kernel.Domain.ReconciliationFilterTemplate>
    {
          #region Properties

        public bool IsReadOnly { get; set; }
        
        public WriteOffConfigPanel WriteOffConfigPanel { get; private set; }

        //public ReconciliationContextPropertyBar ReconciliationContextPropertyBar { get; set; }

        //public ModelService ModelService { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'Target en édition
        /// </summary>
        public Kernel.Domain.ReconciliationFilterTemplate EditedObject { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public Kernel.Service.GroupService GroupService { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructeur
        /// </summary>
        public ReconciliationFilterTemplateForm()
        {
            InitializeComponents();
            InitializeHandlers();
        }

        protected virtual void InitializeComponents()
        {
            this.Background = Brushes.White;
            this.BorderBrush = null;

            this.WriteOffConfigPanel = new WriteOffConfigPanel();
            //this.ReconciliationContextPropertyBar = new ReconciliationContextPropertyBar();
            this.Content = WriteOffConfigPanel;
        }

        protected void InitializeHandlers()
        {
            //this.WriteOffConfigPanel.OnAddField += OnAddField;
            //this.WriteOffConfigPanel.OnDeleteField += OnDeleteField;

        }

        private void OnDeleteField(object item)
        {

        }

        private void OnAddField(object item)
        {

        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            this.WriteOffConfigPanel.ActiveFieldPanel.setAttribute(attribute);
        }

        public void setValue(Kernel.Domain.AttributeValue value)
        {
            //this.WriteOffConfigPanel.ActiveFieldPanel.setAttribute(attribute);
        }

        public void setMeasure(Kernel.Domain.Measure measure) 
        {
            this.WriteOffConfigPanel.ActiveFieldPanel.setMeasure(measure);
        }

        public void setPeriodName(Kernel.Domain.PeriodName periodName)
        {
            this.WriteOffConfigPanel.ActiveFieldPanel.setPeriodName(periodName);
        }

        public void setPeriodInterval(Kernel.Domain.PeriodInterval periodInterval)
        {

        }

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
        public Kernel.Domain.ReconciliationFilterTemplate getNewObject() { return new Kernel.Domain.ReconciliationFilterTemplate(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            //bool valid = this.ReconciliationContextPanel.validateEdition();
            return true;
        }
        
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            this.EditedObject = new Kernel.Domain.ReconciliationFilterTemplate();
            this.EditedObject.writeOffConfig = new Kernel.Domain.WriteOffConfiguration();
            int index = 0;
            for (int i = 0; i < 3; i++)
            {
               
                Kernel.Domain.WriteOffField writeOffField = new Kernel.Domain.WriteOffField();
                writeOffField.position = index;
                index++;
                writeOffField.mandatory = true;
                writeOffField.measureField = new Kernel.Domain.Measure(){
                    name ="Measure1"
                };

                for (int v = 0; v < 5; v++)
                {
                    Kernel.Domain.WriteOffFieldValue value = new Kernel.Domain.WriteOffFieldValue();
                    value.position = v;
                    value.attribute = new Kernel.Domain.Attribute() {name="Attribute1"};
                    value.defaultValueType = Kernel.Domain.WriteOffFieldValueType.LEFT_SIDE;
                    writeOffField.valueListChangeHandler.AddNew(value);
                }
                
                this.EditedObject.writeOffConfig.fieldListChangeHandler.AddNew(writeOffField);
              

                Kernel.Domain.WriteOffField writeOffField1 = new Kernel.Domain.WriteOffField();
                writeOffField1.position = index;
                index++;
                writeOffField1.mandatory = false;
                writeOffField1.attributeField = new Kernel.Domain.Attribute()
                {
                    name ="Attribute1"
                };
                             

                this.EditedObject.writeOffConfig.fieldListChangeHandler.AddNew(writeOffField1);

                Kernel.Domain.WriteOffField writeOffField2 = new Kernel.Domain.WriteOffField();
                writeOffField2.position = index;
                index++;
                writeOffField2.mandatory = true;
                writeOffField2.periodField = new Kernel.Domain.PeriodName("Period1");
                for (int v = 0; v < 5; v++)
                {
                    Kernel.Domain.WriteOffFieldValue value = new Kernel.Domain.WriteOffFieldValue();
                    value.position = v;
                    value.period = new Kernel.Domain.PeriodInterval() { name = "PeriodInterval" };
                    value.defaultValueType = Kernel.Domain.WriteOffFieldValueType.CUSTOM;
                    writeOffField2.valueListChangeHandler.AddNew(value);
                }

                this.EditedObject.writeOffConfig.fieldListChangeHandler.AddNew(writeOffField2);
               
            }
            if (this.EditedObject == null)
            {
                this.WriteOffConfigPanel.display(null);
            }
            else
            {
                this.WriteOffConfigPanel.display(this.EditedObject.writeOffConfig);
                this.WriteOffConfigPanel.getActiveFieldPanel();
            }
        }

        public void fillObject()
        {
            
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.AddRange(this.WriteOffConfigPanel.getEditableControls());
            return controls;
        }

       

        #endregion

        
    }
}
