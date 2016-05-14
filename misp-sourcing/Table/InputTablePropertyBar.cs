using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock;


namespace Misp.Sourcing.Table
{
    public class InputTablePropertyBar : PropertyBar
    {

        public LayoutAnchorable TableLayoutAnchorable { get; set; }
        public LayoutAnchorable CellLayoutAnchorable { get; set; }

        public LayoutAnchorable ParameterLayoutAnchorable { get; set; }
        public LayoutAnchorable MappingLayoutAnchorable { get; set; }

        public LayoutAnchorablePane Pane { get; set; }

        protected override void UserInitialisation() 
        {
            this.TableLayoutAnchorable = new LayoutAnchorable();
            this.CellLayoutAnchorable = new LayoutAnchorable();
            this.ParameterLayoutAnchorable = new LayoutAnchorable();
            this.MappingLayoutAnchorable = new LayoutAnchorable();

            this.TableLayoutAnchorable.Title = "Table Properties";
            this.CellLayoutAnchorable.Title = "Cell Properties";
            this.ParameterLayoutAnchorable.Title = "Cell Properties"; //"Parameter";
            this.MappingLayoutAnchorable.Title = "Mapping";

            this.TableLayoutAnchorable.CanClose = false;
            this.TableLayoutAnchorable.CanFloat = false;
            this.TableLayoutAnchorable.CanHide = false;
            this.TableLayoutAnchorable.CanAutoHide = false;

            this.CellLayoutAnchorable.CanClose = false;
            this.CellLayoutAnchorable.CanFloat = false;
            

            this.ParameterLayoutAnchorable.CanClose = false;
            this.ParameterLayoutAnchorable.CanFloat = false;
            this.ParameterLayoutAnchorable.CanHide = false;
            this.ParameterLayoutAnchorable.CanAutoHide = false;
            this.MappingLayoutAnchorable.CanClose = false;
            this.MappingLayoutAnchorable.CanFloat = false;
            
            /** Décommenter pour avoir le mapping et la parametrisation des cells */
            //DockingManager manager = new DockingManager();
            //LayoutRoot root = new LayoutRoot();
            //LayoutPanel panel = new LayoutPanel();
            //LayoutAnchorablePane cellPane = new LayoutAnchorablePane();
            //cellPane.Children.Add(ParameterLayoutAnchorable);
            //cellPane.Children.Add(MappingLayoutAnchorable);
            //manager.Layout = root;
            //root.RootPanel = panel;
            //panel.Children.Add(cellPane);
            //CellLayoutAnchorable.Content = manager;
            //LayoutAnchorablePane pane = new LayoutAnchorablePane();
            //pane.Children.Add(TableLayoutAnchorable);
            //pane.Children.Add(CellLayoutAnchorable);
            //this.Panes.Add(pane);

            Pane = new LayoutAnchorablePane();
            
            
            Pane.Children.Add(TableLayoutAnchorable);
            Pane.Children.Add(ParameterLayoutAnchorable);
            this.Panes.Add(Pane);

        }

    }
}
