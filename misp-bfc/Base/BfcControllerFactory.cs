using Misp.Bfc.Advisements;
using Misp.Bfc.Model;
using Misp.Bfc.Review;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Base
{
    public class BfcControllerFactory : ControllerFactory
    {

        /// <summary>
        /// Build a new instance of ReconciliationControllerFactory.
        /// </summary>
        /// <param name="applicationManager">Application manager</param>
        public BfcControllerFactory(ApplicationManager applicationManager)
            : base(applicationManager)
        {
            this.ServiceFactory = new BfcServiceFactory(applicationManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonctionality"></param>
        /// <returns></returns>
        public override Controllable GetController(string fonctionality, ViewType? viewType = null, EditionMode? editionMode = null)
        {
            if (fonctionality == BfcFunctionalitiesCode.REVIEW)
            {
                ReviewBrowserController reviewController = new ReviewBrowserController();
                reviewController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                reviewController.ApplicationManager = this.ApplicationManager;
                //reviewController.Service = ((BfcServiceFactory)ServiceFactory).GetReviewService();
                return reviewController;
            }

            if (fonctionality == BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT)
            {
                AdvisementEditorController advisementEditorController = new AdvisementEditorController(AdvisementType.PREFUNDING);
                advisementEditorController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementEditorController.ApplicationManager = this.ApplicationManager;
                advisementEditorController.Service = ((BfcServiceFactory)ServiceFactory).GetAdvisementService();
                return advisementEditorController;
            }
            if (fonctionality == BfcFunctionalitiesCode.MEMBER_ADVISEMENT)
            {
                AdvisementEditorController advisementEditorController = new AdvisementEditorController(AdvisementType.MEMBER);
                advisementEditorController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementEditorController.ApplicationManager = this.ApplicationManager;
                advisementEditorController.Service = ((BfcServiceFactory)ServiceFactory).GetAdvisementService();
                return advisementEditorController;
            }


            if (fonctionality == BfcFunctionalitiesCode.EXCEPTIONAL_ADVISEMENT)
            {
                AdvisementEditorController advisementEditorController = new AdvisementEditorController(AdvisementType.EXCEPTIONAL);
                advisementEditorController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementEditorController.ApplicationManager = this.ApplicationManager;
                advisementEditorController.Service = ((BfcServiceFactory)ServiceFactory).GetAdvisementService();
                return advisementEditorController;
            }

            if (fonctionality == BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT) 
            {
                AdvisementEditorController advisementEditorController = new AdvisementEditorController(AdvisementType.SETTLEMENT);
                advisementEditorController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementEditorController.ApplicationManager = this.ApplicationManager;
                advisementEditorController.Service = ((BfcServiceFactory)ServiceFactory).GetAdvisementService();
                return advisementEditorController;
            }

            if (fonctionality == BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT_LIST)
            {
                AdvisementBrowserController advisementBrowserController = new AdvisementBrowserController(AdvisementType.PREFUNDING);
                advisementBrowserController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementBrowserController.ApplicationManager = this.ApplicationManager;
                advisementBrowserController.Service = ((BfcServiceFactory)ServiceFactory).GetAdvisementService();
                return advisementBrowserController;
            }

            if (fonctionality == BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT_LIST)
            {
                AdvisementBrowserController advisementBrowserController = new AdvisementBrowserController(AdvisementType.SETTLEMENT);
                advisementBrowserController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementBrowserController.ApplicationManager = this.ApplicationManager;
                advisementBrowserController.Service = ((BfcServiceFactory)ServiceFactory).GetAdvisementService();
                return advisementBrowserController;
            }

            if (fonctionality == BfcFunctionalitiesCode.EXCEPTIONAL_ADVISEMENT_LIST)
            {
                AdvisementBrowserController advisementBrowserController = new AdvisementBrowserController(AdvisementType.EXCEPTIONAL);
                advisementBrowserController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementBrowserController.ApplicationManager = this.ApplicationManager;
                advisementBrowserController.Service = ((BfcServiceFactory)ServiceFactory).GetAdvisementService();
                return advisementBrowserController;
            }

            if (fonctionality == BfcFunctionalitiesCode.MEMBER_ADVISEMENT_LIST)
            {
                AdvisementBrowserController advisementBrowserController = new AdvisementBrowserController(AdvisementType.MEMBER);
                advisementBrowserController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementBrowserController.ApplicationManager = this.ApplicationManager;
                advisementBrowserController.Service = ((BfcServiceFactory)ServiceFactory).GetAdvisementService();
                return advisementBrowserController;
            }

            return null;
        }

    }
}
