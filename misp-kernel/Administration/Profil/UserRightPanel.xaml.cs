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
    /// Interaction logic for UserRightPanel.xaml
    /// </summary>
    public partial class UserRightPanel : Grid
    {
        
        public bool IsReadOnly { get; set; }

        public ChangeEventHandler Changed;

        public bool trow;


        public UserRightPanel()
        {
            InitializeComponent();
            InitializeHandlers();           
            trow = false;
        }

        private void InitializeHandlers() 
        {
            
        }

        private void OnChange()
        {
            if (!trow) trow = true;
            else return;
            if (trow && Changed != null) Changed();
        }


        public void Display() 
        {
            
            if (this.IsReadOnly) SetReadOnly(this.IsReadOnly);
            
            trow = false;
        }

        public void SetReadOnly(bool readOnly) 
        {
            
        }

        public void Fill()
        {
            
            trow = false;
        }

        public void setValue(object value)
        {
           
        }

        public void resetComponent()
        {
            
        }
    }
}
