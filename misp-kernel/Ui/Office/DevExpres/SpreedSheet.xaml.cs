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

namespace Misp.Kernel.Ui.Office.DevExpres
{
    /// <summary>
    /// Interaction logic for SpreedSheet.xaml
    /// </summary>
    public partial class SpreedSheet : UserControl,ISpreadsheet
    {
        public SpreedSheet()
        {
            InitializeComponent();
        }

        public event Base.ChangeEventHandler Changed;

        public event EditEventHandler Edited;

        public event SelectionChangedEventHandler SelectionChanged;

        public event SheetActivateEventHandler SheetActivated;

        public event SheetAddedEventHandler SheetAdded;

        public event SheetDeletedEventHandler SheetDeleted;

        public object AddExcelMenu(string Header)
        {
            throw new NotImplementedException();
        }

        public Application.OperationState Import()
        {
            throw new NotImplementedException();
        }

        public string CreateNewExcelFile()
        {
            throw new NotImplementedException();
        }

        public Application.OperationState Open(string filePath, string progID)
        {
            throw new NotImplementedException();
        }

        public object RemoveExcelMenu(string Header)
        {
            throw new NotImplementedException();
        }

        public Application.OperationState Close()
        {
            throw new NotImplementedException();
        }

        public Application.OperationState SaveAs(string filePath, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public Application.OperationState Export(string filePath)
        {
            throw new NotImplementedException();
        }

        public string GetFilePath()
        {
            throw new NotImplementedException();
        }

        public Range GetSelectedRange()
        {
            return null;
        }

        public void SetValueAt(int row, int colunm, string sheetName, object value)
        {
            
        }

        public object getValueAt(int row, int colunm, string sheetName)
        {
            return null;
        }

        public void ClearValueAt(int row, int colunm)
        {
            
        }

        public Cell getActiveCell()
        {
            return new Cell(this.SpreadSheet.ActiveCell.RowIndex, this.SpreadSheet.ActiveCell.ColumnIndex);
        }

        public void DisableToolBar(bool value)
        {
            
        }

        public void DisableTitleBar(bool value)
        {
            
        }

        public Range getActiveCellAsRange()
        {
            throw new NotImplementedException();
        }
    }
}
