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

namespace Misp.Kernel.Administration.Profil
{
    /// <summary>
    /// Interaction logic for RightGroup.xaml
    /// </summary>
    public partial class RightGroup : Border
    {

        public event RightEventHandler RightSelected;

        public RightGroup()
        {
            InitializeComponent();
            InitHandlers();
        }
        
        public RightGroup(Functionality functionality) : this()
        {
            this.RightField.SetFunctionality(functionality);
        }

        public void Select(String functionalityCode)
        {
            this.RightField.Select(functionalityCode);
        }

        protected void InitHandlers()
        {
            this.RightField.RightSelected += OnRightSelected;
        }

        private void OnRightSelected(string functionality, bool selected)
        {
            if (RightSelected != null) RightSelected(functionality, selected);
        }
    }
}
