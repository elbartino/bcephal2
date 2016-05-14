using System;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Misp.Kernel.Ui.TreeView
{
    public class EditableBlock : ContentControl
    {
        public TextBlock Renderer { get; private set; }
        public TextBox Editor { get; private set; }
        public string Text { get; set; }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditableBlock), new UIPropertyMetadata("AAAA", UpdateText));

        /// <summary>
        /// Determines whether [is in edit mode update] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void UpdateText(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            EditableBlock block = obj as EditableBlock;
            if (null != block)
            {
                block.Renderer.Text = (string)e.NewValue;
            }
        }


        public EditableBlock()
        {
            InitComponents();
            InitHandlers();
        }

        public void Edit()
        {
            Editor.Text = Renderer.Text;            
            Editor.Visibility = System.Windows.Visibility.Visible;
            Renderer.Visibility = System.Windows.Visibility.Hidden;
            Editor.SelectAll();
            Editor.Focus();
        }

        public void CancelEdition()
        {
            Editor.Visibility = System.Windows.Visibility.Hidden;
            Renderer.Visibility = System.Windows.Visibility.Visible;
        }

        public void ValidateEdition()
        {
            Renderer.Text = Editor.Text;
            CancelEdition();
        }

        protected void InitComponents()
        {

            Grid grid = new Grid();
            Renderer = new TextBlock();
            Editor = new TextBox();
            grid.Children.Add(Renderer);
            grid.Children.Add(Editor);
            this.Content = grid;
            Renderer.Visibility = System.Windows.Visibility.Visible;
            Editor.Visibility = System.Windows.Visibility.Hidden;
        }

        protected void InitHandlers()
        {
            MouseDoubleClick += new MouseButtonEventHandler(edit);
            Renderer.MouseLeftButtonUp += new MouseButtonEventHandler(tryToEdit);
            Editor.LostFocus += new System.Windows.RoutedEventHandler(cancelEdition);
            this.LostFocus += new System.Windows.RoutedEventHandler(cancelEdition);
            Editor.KeyUp += new KeyEventHandler(validateEdition);
            //Renderer.MouseEnter += new MouseEventHandler(mouseEnter);
        }



        protected void mouseEnter(object sender, MouseEventArgs args)
        {
            Cursor = Cursors.Pen;
        }

        protected void tryToEdit(object sender, MouseButtonEventArgs args)
        {
            if (args.ClickCount == 2) Edit();
        }

        protected void edit(object sender, MouseButtonEventArgs args)
        {
            Edit();
        }

        protected void validateEdition(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                CancelEdition();
            }
            else if (args.Key == Key.Enter)
            {
                ValidateEdition();
            }
        }

        public void cancelEdition(object sender, RoutedEventArgs args)
        {
            CancelEdition();
        }


    }
}
