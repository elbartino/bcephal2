﻿using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.AutomaticTargetViews
{
    public class AutomaticTargetForm : AutomaticSourcingForm
    {
        #region Constructors

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            this.AutomaticTablePropertiesPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.AutomaticSourcingPanel.CustomizeForTarget();
        }

        protected override bool isAutomaticTarget()
        {
            return true;
        }

        #endregion
    }
}
