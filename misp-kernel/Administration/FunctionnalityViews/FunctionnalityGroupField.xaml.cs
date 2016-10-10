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
        public string newFunctionCode;

        public FunctionnalityView FunctionnalityView;

        public Functionality functionality { get; set; }

        public FunctionnalityGroupField()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        public void InitializeHandlers() 
        {
            this.MainFunctionality.CheckMainFunctionality += OnCheckMainFunctionality;
        }

        private void OnCheckMainFunctionality(Functionality data)
        {
            
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
            setSubFunctionalities(this.functionality.children);
        }

        public void setMainFunctionality(Functionality data)
        {
            this.MainFunctionality.Children.Clear();
            FunctionnalityField item = new FunctionnalityField(data);
            item.CheckMainFunctionality += OnCheckMainFunctionality;
            this.MainFunctionality.Children.Add(item);
        }

                   
        private void setSubFunctionalities(List<Domain.Functionality> datas) 
        {
            this.FieldPanel.Children.Clear();
            foreach (Domain.Functionality data in datas)
            {
                FunctionnalityField item = new FunctionnalityField(data);
                item.CheckSubFunctionality += OnCheckSubFunctionality;
                item.GroupField = this;
                this.FieldPanel.Children.Add(item);
            }
        }


        private void OnCheckSubFunctionality(Functionality data)
        {
            
        }
               
    }
}
