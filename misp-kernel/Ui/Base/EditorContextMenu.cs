using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Ui.Base
{
    public class EditorContextMenu
    {
        public MenuItem DeleteItemMenu { get; set; }
        public MenuItem RenameItemMenu { get; set; }
        public MenuItem SaveItemMenu { get; set; }
        public MenuItem SaveAsItemMenu { get; set; }


        public EditorContextMenu()
        {
            DeleteItemMenu = new MenuItem();
            DeleteItemMenu.Header = "Delete";

            RenameItemMenu = new MenuItem();
            RenameItemMenu.Header = "Rename";

            SaveItemMenu = new MenuItem();
            SaveItemMenu.Header = "Save";

            SaveAsItemMenu = new MenuItem();
            SaveAsItemMenu.Header = "SaveAs";
        }
    }
}
