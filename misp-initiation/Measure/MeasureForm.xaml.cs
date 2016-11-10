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
using Misp.Kernel.Ui.Measure;
using System.Collections.ObjectModel;
using System.Collections;
using Misp.Kernel.Ui.EditableTree;

namespace Misp.Initiation.Measure
{
    /// <summary>
    /// Interaction logic for MeasureForm.xaml
    /// </summary>
    public partial class MeasureForm : Grid, IEditableView<Misp.Kernel.Domain.Measure>
    {
        
        /// <summary>
        /// 
        /// </summary>
        public MeasureForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }
        
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
        public Misp.Kernel.Domain.Measure EditedObject
        {
            get;
            set;
        }

        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public Misp.Kernel.Domain.Measure getNewObject() { return new Misp.Kernel.Domain.Measure(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// Et retire  la valeur par défaut des listes à envoyer dans au serveur.
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
        public void fillObject() 
        {
            
        }
        
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject() 
        {
         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls() 
        {
            List<object> controls = new List<object>(0);
            controls.Add(measureEditableTree);
            return controls;
        }

        public MeasureEditableTree MeasureTree
        {
            get { return measureEditableTree; } 
        }
        
    }
}
