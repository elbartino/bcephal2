﻿using Misp.Bfc.Advisements;
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
            if (fonctionality == BfcFunctionalitiesCode.REVIEW_PF_ACCOUNT)
            {
                ReviewBrowserController reviewController = new ReviewBrowserController();
                reviewController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                reviewController.ApplicationManager = this.ApplicationManager;
                reviewController.DefaultActiveTab = 0;
                return reviewController;
            }

            if (fonctionality == BfcFunctionalitiesCode.REVIEW_SETTLEMENT_EVOLUTION)
            {
                ReviewBrowserController reviewController = new ReviewBrowserController();
                reviewController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                reviewController.ApplicationManager = this.ApplicationManager;
                reviewController.DefaultActiveTab = 1;
                return reviewController;
            }

            if (fonctionality == BfcFunctionalitiesCode.REVIEW_AGEING_BALANCE)
            {
                ReviewBrowserController reviewController = new ReviewBrowserController();
                reviewController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                reviewController.ApplicationManager = this.ApplicationManager;
                reviewController.DefaultActiveTab = 2;
                return reviewController;
            }

            if (fonctionality == BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT)
            {
                AdvisementEditorController advisementEditorController = new AdvisementEditorController(AdvisementType.PREFUNDING);
                advisementEditorController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementEditorController.ApplicationManager = this.ApplicationManager;
                advisementEditorController.Service = ((BfcServiceFactory)ServiceFactory).GetPrefundingAdvisementService();
                return advisementEditorController;
            }
            if (fonctionality == BfcFunctionalitiesCode.MEMBER_ADVISEMENT)
            {
                AdvisementEditorController advisementEditorController = new AdvisementEditorController(AdvisementType.MEMBER);
                advisementEditorController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementEditorController.ApplicationManager = this.ApplicationManager;
                advisementEditorController.Service = ((BfcServiceFactory)ServiceFactory).GetMemberAdvisementService();
                return advisementEditorController;
            }


            if (fonctionality == BfcFunctionalitiesCode.REPLENISHMENT_INSTRUCTION_ADVISEMENT)
            {
                AdvisementEditorController advisementEditorController = new AdvisementEditorController(AdvisementType.REPLENISHMENT);
                advisementEditorController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementEditorController.ApplicationManager = this.ApplicationManager;
                advisementEditorController.Service = ((BfcServiceFactory)ServiceFactory).GetExceptionalAdvisementService();
                return advisementEditorController;
            }

            if (fonctionality == BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT) 
            {
                AdvisementEditorController advisementEditorController = new AdvisementEditorController(AdvisementType.SETTLEMENT);
                advisementEditorController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementEditorController.ApplicationManager = this.ApplicationManager;
                advisementEditorController.Service = ((BfcServiceFactory)ServiceFactory).GetSettlementAdvisementService();
                return advisementEditorController;
            }

            if (fonctionality == BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT_LIST)
            {
                AdvisementBrowserController advisementBrowserController = new AdvisementBrowserController(AdvisementType.PREFUNDING);
                advisementBrowserController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementBrowserController.ApplicationManager = this.ApplicationManager;
                advisementBrowserController.Service = ((BfcServiceFactory)ServiceFactory).GetPrefundingAdvisementService();
                return advisementBrowserController;
            }

            if (fonctionality == BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT_LIST)
            {
                AdvisementBrowserController advisementBrowserController = new AdvisementBrowserController(AdvisementType.SETTLEMENT);
                advisementBrowserController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementBrowserController.ApplicationManager = this.ApplicationManager;
                advisementBrowserController.Service = ((BfcServiceFactory)ServiceFactory).GetSettlementAdvisementService();
                return advisementBrowserController;
            }

            if (fonctionality == BfcFunctionalitiesCode.REPLENISHMENT_INSTRUCTION_ADVISEMENT_LIST)
            {
                AdvisementBrowserController advisementBrowserController = new AdvisementBrowserController(AdvisementType.REPLENISHMENT);
                advisementBrowserController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementBrowserController.ApplicationManager = this.ApplicationManager;
                advisementBrowserController.Service = ((BfcServiceFactory)ServiceFactory).GetExceptionalAdvisementService();
                return advisementBrowserController;
            }

            if (fonctionality == BfcFunctionalitiesCode.MEMBER_ADVISEMENT_LIST)
            {
                AdvisementBrowserController advisementBrowserController = new AdvisementBrowserController(AdvisementType.MEMBER);
                advisementBrowserController.FunctionalityCode = Misp.Bfc.PlugIn.MODULE_NAME;
                advisementBrowserController.ApplicationManager = this.ApplicationManager;
                advisementBrowserController.Service = ((BfcServiceFactory)ServiceFactory).GetMemberAdvisementService();
                return advisementBrowserController;
            }

            return null;
        }

    }
}
