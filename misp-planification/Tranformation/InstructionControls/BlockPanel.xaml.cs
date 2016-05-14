using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Misp.Planification.Tranformation.InstructionControls
{
    /// <summary>
    /// Interaction logic for BlockPanel.xaml
    /// </summary>
    public partial class BlockPanel : Grid
    {
        public ChangeEventHandler ChangeEventHandler;
        public Kernel.Ui.Base.ChangeItemEventHandler Deleted;
        public Kernel.Ui.Base.ChangeItemEventHandler Added;

        #region Events
        public event OnCloseSlideDialogEventHandler OnCloseSlideDialog;
        public event OnCloseTransformationTableDialogEventHandler OnCloseTransformationTableDialog;


        public delegate void OnCloseSlideDialogEventHandler();
        public delegate void OnCloseTransformationTableDialogEventHandler();
        #endregion


        public static List<TransformationTreeItem> Loops { get; set; }

        public static List<string> TransformationTables { get; set; }

        public static List<string> TransformationSlides { get; set; }

        public static string ActionReportName { get; set; }

        public static ObservableCollection<Kernel.Domain.Browser.InputTableBrowserData> listeTotalReport { get; set; }

        public static TransformationTreeItem LoopByName(String name)
        {
            if (Loops == null) return null;
            if (string.IsNullOrEmpty(name)) return null;
            foreach (TransformationTreeItem loop in Loops)
            {
                if (loop.name.Equals(name.Trim())) return loop;
            }
            return null;
        }

        public bool trow = false;
        
        public BlockPanel()
        {
            InitializeComponent();
            this.IfInstructionPanel.CustomizeForIf();
            this.ThenInstructionPanel.CustomizeForThen();
            this.ElseInstructionPanel.CustomizeForElse();
            initHandlers();
        }

        public Instruction Fill()
        {
            Instruction instruction = new Instruction(Instruction.BLOCK, Instruction.END);

            Instruction subInstruction = this.IfInstructionPanel.Fill();
            subInstruction.position = 0;
            instruction.subInstructions.Add(subInstruction);

            subInstruction = this.ThenInstructionPanel.Fill();
            subInstruction.position = 1;
            instruction.subInstructions.Add(subInstruction);

            subInstruction = this.ElseInstructionPanel.Fill();
            subInstruction.position = 2;
            instruction.subInstructions.Add(subInstruction);

            return instruction;
        }

        public void Display(Instruction block)
        {
            trow = false;
            if (block == null)
            {
                Reset();
                return;
            }
            this.IfInstructionPanel.Display(block.getIfInstruction());
            this.ThenInstructionPanel.Display(block.getThenInstruction());
            this.ElseInstructionPanel.Display(block.getElseInstruction());
            trow = true;
        }

        private void Reset()
        {
            trow = false;
            this.IfInstructionPanel.Reset();
            this.ThenInstructionPanel.Reset();
            this.ElseInstructionPanel.Reset();

            this.IfInstructionPanel.CustomizeForIf();
            this.ThenInstructionPanel.CustomizeForThen();
            this.ElseInstructionPanel.CustomizeForElse();
            trow = true;
        }

        protected void initHandlers()
        {
            this.AddButton.Click += OnAddButtonClick;
            this.DeleteButton.Click += OnDeleteButtonClick;

            this.IfInstructionPanel.ChangeEventHandler += onChange;
            this.ThenInstructionPanel.ChangeEventHandler += onChange;
            this.ElseInstructionPanel.ChangeEventHandler += onChange;
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Deleted != null) Deleted(this);
            onChange();
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Added != null) Added(this);
            onChange();
        }

        public void onChange()
        {
            if (trow && ChangeEventHandler != null) ChangeEventHandler();
        }

    }
}
