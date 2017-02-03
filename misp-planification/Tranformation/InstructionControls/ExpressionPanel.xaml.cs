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

namespace Misp.Planification.Tranformation.InstructionControls
{
    /// <summary>
    /// Interaction logic for ExpressionPanel.xaml
    /// </summary>
    public partial class ExpressionPanel : Grid
    {
        public ChangeEventHandler ChangeEventHandler;
        public Kernel.Ui.Base.ChangeItemEventHandler Deleted;
        public Kernel.Ui.Base.ChangeItemEventHandler Added;
        
        public bool trow = false;

        public bool IsReadOnly { get; set; }

        public ExpressionPanel()
        {
            InitializeComponent();
            this.OperatorComboBox.ItemsSource = Instruction.lOGICAL_OPERATORS;
            this.SignComboBox.ItemsSource = Instruction.COMP_OPERATORS;
            this.OpenBracketComboBox.ItemsSource = Instruction.OPEN_BRACKETS;
            this.CloseBracketComboBox.ItemsSource = Instruction.CLOSE_BRACKETS;
            this.OperatorComboBox.SelectedItem = "AND";
            initHandlers();
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
            this.OperatorComboBox.IsEnabled = OperatorComboBox.IsEnabled && !readOnly;
            this.SignComboBox.IsEnabled = !readOnly;
            this.OpenBracketComboBox.IsEnabled = !readOnly;
            this.CloseBracketComboBox.IsEnabled = !readOnly;
            this.OperatorComboBox.IsEnabled = !readOnly;
            this.AddButton.Visibility = readOnly ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            this.DeleteButton.Visibility = readOnly ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            this.Arg2TextBox.IsEnabled = !readOnly;
            this.Arg1TextBox.IsEnabled = !readOnly;
            this.CommentTextBlock.IsEnabled = !readOnly;
            this.NoCommentButton.ToolTip = readOnly ? "View comment" : "Edit comment";
        }

        public ConditionItem Fill()
        {
            ConditionItem item = new ConditionItem();
            if (OperatorComboBox.SelectedItem != null) item.operatorSign = OperatorComboBox.SelectedItem.ToString();
            if (OpenBracketComboBox.SelectedItem != null) item.openBracket = OpenBracketComboBox.SelectedItem.ToString();
            if (Arg1TextBox.Text != null) item.arg1 = Arg1TextBox.Text;
            if (SignComboBox.SelectedItem != null) item.sign = SignComboBox.SelectedItem.ToString();
            if (Arg2TextBox.Text != null) item.arg2 = Arg2TextBox.Text;
            if (CloseBracketComboBox.SelectedItem != null) item.closeBracket = CloseBracketComboBox.SelectedItem.ToString();
            item.comment = this.CommentTextBlock.Text.Trim();
            return item;
        }

        public String FillAsString()
        {
            String text = "";
            if (OperatorComboBox.SelectedItem != null)
            {
                String op = OperatorComboBox.SelectedItem.ToString();
                if (op.Equals("AND")) text += ConditionItem.AND;
                else if (op.Equals("OR")) text += ConditionItem.OR;
                else text += op;
            }
            if (OpenBracketComboBox.SelectedItem != null) text += " " + OpenBracketComboBox.SelectedItem.ToString();
            if (Arg1TextBox.Text != null) text += " " + Arg1TextBox.Text;
            if (SignComboBox.SelectedItem != null) text += " " + SignComboBox.SelectedItem.ToString();
            if (Arg2TextBox.Text != null) text += " " + Arg2TextBox.Text;
            if (CloseBracketComboBox.SelectedItem != null) text += " " + CloseBracketComboBox.SelectedItem.ToString();
            //this.CommentTextBlock.Text = "";
            return text;
        }

        public void Display(ConditionItem condition)
        {
            trow = false;
            if (condition == null)
            {
                Reset();
                return;
            }
            
            this.OperatorComboBox.SelectedItem = condition.operatorSign != null ? condition.operatorSign : "";
            this.SignComboBox.SelectedItem = condition.sign != null ? condition.sign : "";
            this.OpenBracketComboBox.SelectedItem = condition.openBracket != null ? condition.openBracket : "";
            this.CloseBracketComboBox.SelectedItem = condition.closeBracket != null ? condition.closeBracket : "";
            this.Arg1TextBox.Text = condition.arg1 != null ? condition.arg1 : "";
            this.Arg2TextBox.Text = condition.arg2 != null ? condition.arg2 : "";
            this.CommentTextBlock.Text = String.IsNullOrWhiteSpace(condition.comment) ? "" : condition.comment.Trim();
            refreshCommentIcon();
            if (IsReadOnly) SetReadOnly(IsReadOnly);
            trow = true;
        }

        public void DisplayFromLoop(ConditionItem condition) 
        {
            String result = "Result";
            if (condition != null && (!string.IsNullOrEmpty(condition.arg1) && !condition.arg1.Equals(result,StringComparison.OrdinalIgnoreCase)) && string.IsNullOrEmpty(condition.arg2))
            {
                
                condition.arg2 = condition.arg1;
                condition.arg1 = result;
            }
            Display(condition);
        }

        public void Reset()
        {
            trow = false;
            this.OperatorComboBox.SelectedItem = "";
            this.SignComboBox.SelectedItem = "";
            this.OpenBracketComboBox.SelectedItem = "";
            this.CloseBracketComboBox.SelectedItem = "";

            this.Arg1TextBox.Text = "";
            this.Arg2TextBox.Text = "";
            this.CommentTextBlock.Text = "";
            refreshCommentIcon();
            trow = true;
        }

        protected void initHandlers()
        {
            this.AddButton.Click += OnAddButtonClick;
            this.DeleteButton.Click += OnDeleteButtonClick;
            
            this.SignComboBox.SelectionChanged += onChange;
            this.OpenBracketComboBox.SelectionChanged += onChange;
            this.CloseBracketComboBox.SelectionChanged += onChange;
            this.OperatorComboBox.SelectionChanged += onChange;
            this.Arg1TextBox.TextChanged += onChange;
            this.Arg2TextBox.TextChanged += onChange;

            this.CommentTextBlock.TextChanged += OnCommentChange;
            this.CommentPopup.Opened += OnCommentPopupOpened;

            this.CommentButton.Checked += OnComment;
            this.NoCommentButton.Checked += OnComment;
        }

        private void OnComment(object sender, RoutedEventArgs e)
        {
            this.CommentPopup.IsOpen = true;
        }

        private void OnCommentPopupOpened(object sender, EventArgs e)
        {
            this.CommentTextBlock.Focus();
        }

        private void refreshCommentIcon()
        {
            bool hasComment = !string.IsNullOrWhiteSpace(this.CommentTextBlock.Text);
            this.CommentButton.Visibility = hasComment ? Visibility.Visible : Visibility.Hidden;
            this.NoCommentButton.Visibility = hasComment ? Visibility.Hidden : Visibility.Visible;            
        }

        private void OnCommentChange(object sender, TextChangedEventArgs e)
        {
            onChange();
            refreshCommentIcon();
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Deleted != null) Deleted(this);
            onChange();
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Added != null) Added(this);
        }

        private void onChange(object sender, TextChangedEventArgs e)
        {
            onChange();
        }

        private void onChange(object sender, SelectionChangedEventArgs e)
        {
            onChange();
        }

        public void onChange()
        {
            if (trow && ChangeEventHandler != null) ChangeEventHandler();
        }


    }
}
