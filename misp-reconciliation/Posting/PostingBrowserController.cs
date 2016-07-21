using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Application;
using Misp.Kernel.Service;
using Misp.Kernel.Domain;
using Misp.Reconciliation.Base;

namespace Misp.Reconciliation.Posting
{
    public class PostingBrowserController : BrowserController<Misp.Kernel.Domain.Persistent, PostingBrowserData>
    {

        public PostingBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <returns></returns>
        public override OperationState Search()
        {
            try
            {
                GetPostingBrowser().form.Search();
               // GetPostingBrowser().form.filterForm.Display(null);
                return OperationState.CONTINUE;
            }
            catch (ServiceExecption e)
            {
                DisplayError("error", e.Message);
            }

            return OperationState.STOP;
        }
        
        public PostingBrowser GetPostingBrowser()
        {
            return (PostingBrowser)this.View;
        }

        public PostingService GetPostingService()
        {
            return (PostingService)this.Service;
        }

        /// <summary>
        /// Cette methode permet de créer une nouvelle reco.
        /// </summary>
        /// <returns>CONTINUE si la création de la nouvelle reconciliation se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {  
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.RECONCILIATION;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() {
            return ReconciliationFunctionalitiesCode.RECONCILIATION_POSTING_FUNCTIONALITY;
        }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new PostingBrowser(); }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() {
            GetPostingBrowser().form.PostingService = GetPostingService();
        }
        
        public override OperationState Search(object oid)
        {
            return OperationState.CONTINUE;
        }

        protected override void initializeViewHandlers()
        {
            
        }

        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar()
        {
            return new PostingSideBar();
        }

        protected override Misp.Kernel.Ui.Base.ToolBar getNewToolBar() {
            BrowserToolBar bar = (BrowserToolBar)base.getNewToolBar();
            bar.NewButton.Visibility = System.Windows.Visibility.Hidden;
            return bar; 
        }

        

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            
            if (this.SideBar == null || this.Service == null) return;
           
            ((PostingSideBar)SideBar).EntityGroup.ModelService = GetPostingService().ModelService;
            ((PostingSideBar)SideBar).EntityGroup.InitializeTreeViewDatas();

            PeriodName rootPeriodName = GetPostingService().periodNameService.getRootPeriodName();
            ((PostingSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(rootPeriodName);
            GetPostingBrowser().form.filterForm.filterPTForm.periodFilter.DefaultPeriodName = rootPeriodName.getDefaultPeriodName();
            GetPostingBrowser().form.filterForm.filterPTForm.periodFilter.DisplayPeriod(null);
        }
        

        protected override void initializeSideBarHandlers()
        {
            ((PostingSideBar)SideBar).EntityGroup.EntityTreeview.SelectionChanged += onSelectTargetFromSidebar;
            ((PostingSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.SelectionChanged += onSelectPeriodNameFromSidebar;
            ((PostingSideBar)SideBar).EntityGroup.EntityTreeview.ExpandAttribute += OnExpandAttribute;

            GetPostingBrowser().form.toolBar.resetRecoButton.Click += OnSelectionChange;
            GetPostingBrowser().form.toolBar.deleteButton.Click += OnSelectionChange;
        }

        private void OnSelectionChange(object sender, System.Windows.RoutedEventArgs e)
        {
            initializeSideBarData();
        }


        private void OnExpandAttribute(object sender)
        {
            if (sender != null && sender is Kernel.Domain.Attribute)
            {
                Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
                if (!attribute.LoadValues)
                {
                    List<Kernel.Domain.AttributeValue> values = GetPostingService().ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                    attribute.valueListChangeHandler.Items.Clear();
                    foreach (Kernel.Domain.AttributeValue value in values)
                    {
                        attribute.valueListChangeHandler.Items.Add(value);
                    }
                    attribute.LoadValues = true;
                }
            }
        }

        private void onSelectPeriodNameFromSidebar(object sender)
        {
            GetPostingBrowser().form.onSelectPeriodNameFromSidebar(sender);
        }

        private void onSelectTargetFromSidebar(object sender)
        {
            GetPostingBrowser().form.onSelectTargetFromSidebar(sender);
        }




    }
}
