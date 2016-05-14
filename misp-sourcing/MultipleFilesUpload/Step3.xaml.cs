using Misp.Kernel.Service;
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

namespace Misp.Sourcing.MultipleFilesUpload
{
    /// <summary>
    /// Interaction logic for Step3.xaml
    /// </summary>
    public partial class Step3 : Grid
    {

        private bool alreadyDisplay;

        public Step3()
        {
            alreadyDisplay = false;
            InitializeComponent();
        }

        public void DisplayDefaultGroup(GroupService service)
        {
            if (alreadyDisplay || service == null) return;
            GroupField.GroupService = service;
            GroupField.Group = service.getDefaultGroup();
            GroupField.subjectType = SubjectTypeFound();
            alreadyDisplay = true;
        }

        public  Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.INPUT_TABLE;
        }
    }
}
