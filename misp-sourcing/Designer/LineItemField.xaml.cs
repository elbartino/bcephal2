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

namespace Misp.Sourcing.Designer
{
    /// <summary>
    /// Interaction logic for LineItemField.xaml
    /// </summary>
    public partial class LineItemField : UserControl 
    {
        
        
        #region Events
        
        /// <summary>
        /// 
        /// </summary>
        public event UpdateEventHandler Updated;

        /// <summary>
        /// Evenement déclenché lorsqu'on clique sur le boutton pour supprimer le TargetItem.
        /// </summary>
        public event DeleteEventHandler Deleted;

        /// <summary>
        /// 
        /// </summary>
        public event ActivateEventHandler Activated;

        #endregion


        #region Properties

        private bool throwEvent = true;

        public int Index { get; set; }

        private LineItem lineItem;
        public LineItem LineItem
        {
            get { lineItem.position = Index; return lineItem; }
            set
            {
                this.lineItem = value;
                if (lineItem != null) Label.Content = lineItem.GetValue();
            }
        }

        #endregion


        #region Contructors
        
        /// <summary>
        /// 
        /// </summary>
        public LineItemField()
        {            
            InitializeComponent();
            InitializeHandlers();
        }

        public LineItemField(int index) : this()
        {
            this.Index = index;            
        }

        /// <summary>
        /// Construit une nouvelle instance de LineItemField
        /// </summary>
        /// <param name="index">Index du panel</param>
        /// <param name="item">LineItem à afficher</param>
        public LineItemField(LineItem item)
            : this(item.position)
        {
            this.LineItem = item; 
        }

        #endregion


        #region Handlers

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.CloseButton.Click += OnButtonClick;
            this.CloseButton.GotFocus += OnGotFocus;
            this.Label.GotFocus += OnGotFocus;
            this.MouseDown += OnMouseDown;
            this.Label.MouseDown += OnMouseDown;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (Deleted != null) Deleted(this);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (throwEvent && Activated != null) Activated(this);
        }

        #endregion

    }
}
