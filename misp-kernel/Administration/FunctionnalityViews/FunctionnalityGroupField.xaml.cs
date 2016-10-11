using Misp.Kernel.Domain;
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

namespace Misp.Kernel.Administration.FunctionnalityViews
{
    /// <summary>
    /// Interaction logic for FunctionnalityGroupField.xaml
    /// </summary>
    public partial class FunctionnalityGroupField : Border
    {

        #region Events

        public event OnAddFunctionality AddFunctionality;
        public delegate void OnAddFunctionality(Functionality data);

        public event OnRemoveFunctionality RemoveFunctionality;
        public delegate void OnRemoveFunctionality(Functionality data);

        #endregion

        public string newFunctionCode;

        public FunctionnalityView FunctionnalityView;

        public Functionality functionality { get; set; }

        public FunctionnalityGroupField()
        {
            InitializeComponent();
        }

        public FunctionnalityGroupField(string newFunctionCde) : this()
        {
            this.newFunctionCode = newFunctionCde;
        }

        public FunctionnalityGroupField(Functionality data) 
        {
            
        }

        public void DisplayFunctionality() 
        {
            if (this.functionality == null) return;
            setMainFunctionality(this.functionality);
            setSubFunctionalities(this.functionality.Children);
        }

        public void setMainFunctionality(Functionality data)
        {
            this.MainFunctionality.Children.Clear();
            FunctionnalityField item = new FunctionnalityField(data);
            item.SelectMainFunctionality += OnCheckMainFunctionality;
            this.MainFunctionality.Children.Add(item);
        }

        private void OnCheckMainFunctionality(Functionality data, bool isRemove,bool enableSub)
        {
            if (AddFunctionality != null && !isRemove)
            {
                AddFunctionality(data);
                EnableSubFunctionalities(enableSub);
            }
            if (RemoveFunctionality != null && isRemove)
            {
                RemoveFunctionality(data);
                EnableSubFunctionalities(enableSub);
            }
        }
                   
        private void setSubFunctionalities(List<Domain.Functionality> datas) 
        {
            this.FieldPanel.Children.Clear();
            foreach (Domain.Functionality data in datas)
            {
                FunctionnalityField item = new FunctionnalityField(data);
                item.SelectSubMainFunctionality += OnCheckSubFunctionality;
                item.GroupField = this;
                this.FieldPanel.Children.Add(item);
            }
        }

        private void EnableSubFunctionalities(bool disable)
        {
            foreach (UIElement panel in this.FieldPanel.Children)
            {
                if (panel is FunctionnalityField)
                {
                    ((FunctionnalityField)panel).CheckBox.IsEnabled = disable;
                }
            }
        }

        private void OnCheckSubFunctionality(Functionality data, bool isRemove)
        {
            if (AddFunctionality != null && !isRemove) AddFunctionality(data);
            if (RemoveFunctionality != null && isRemove) RemoveFunctionality(data);
        }      
    }
}
