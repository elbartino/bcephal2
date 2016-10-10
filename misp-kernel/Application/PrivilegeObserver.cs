﻿using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Application
{
    public class PrivilegeObserver
    {
        /// <summary>
        /// Connected user
        /// </summary>
        public User user;

        protected List<String> privileges;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="user"></param>
        public PrivilegeObserver() {
            initializePrivileges(ApplicationManager.Instance.User);
        }

        /// <summary>
        /// Initialize user rigths
        /// </summary>
        /// <param name="user"></param>
        public void initializePrivileges(User user) {
            privileges = new List<String>(0);
            this.user = ApplicationManager.Instance.User;
            if (user.IsAdmin()) return;
            UserService service = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetUserService();
            List<Right> rights = service.getConnectedUserRights();
		    foreach (Right right in rights) {
                privileges.Add(right.functionnality);
		    }
	    }

        /// <summary>
        /// has Privilege?
        /// </summary>
        /// <param name="code">Functionaliti code</param>
        /// <returns></returns>
        public bool hasPrivilege(String code)
        {
            if (user.IsAdmin()) return true;
            if (String.IsNullOrWhiteSpace(code)) return false;
            Functionality functionality = FunctionalityFactory.Instance.Get(code);
            return hasPrivilege(functionality);
        }

        /// <summary>
        /// has Privilege?
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool hasPrivilege(Functionality functionality)
        {
            if (!user.active.HasValue || !user.active.Value) return false;
            if (user.IsAdmin()) return true;
            if (functionality == null) return false;
            if (containsPrivilege(functionality.Code)) return true;
            return hasPrivilege(functionality.Parent);
        }

        /// <summary>
        /// has Privilege?
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool containsPrivilege(String code)
        {
            if (user.IsAdmin()) return true;
            if (String.IsNullOrWhiteSpace(code)) return false;
            return privileges.Contains(code);
        }


    }
}