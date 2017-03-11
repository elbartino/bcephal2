using Misp.Bfc.Model;
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

namespace Misp.Bfc.Advisements
{
    /// <summary>
    /// Interaction logic for AdvisementForm.xaml
    /// </summary>
    public partial class AdvisementForm : Grid, IEditableView<Advisement>
    {
       
        public AdvisementForm(Kernel.Domain.SubjectType subjectType)
        {
            this.SubjectType = subjectType;
            InitializeComponent();
        }

        public Kernel.Domain.SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }

        public Advisement EditedObject { get; set; }

        public Advisement getNewObject()
        {
            return new Advisement();
        }

        public bool validateEdition()
        {
            return true;
        }

        public void fillObject()
        {
            
        }

        public void displayObject()
        {
        }

        public List<object> getEditableControls()
        {
            return new List<object>();
        }

        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
           
        }

        public void SetReadOnly(bool readOnly)
        {
           
        }

        public void Customize(List<Kernel.Domain.Right> rights, bool readOnly)
        {
            
        }

        public bool IsReadOnly { get; set; }
    }
}
