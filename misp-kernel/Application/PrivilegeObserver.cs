using Misp.Kernel.Domain;
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

        protected List<Right> Rights;

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
            this.user = ApplicationManager.Instance.User;
            if (user.IsAdmin()) return;
            UserService service = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetUserService();
            this.Rights = service.getConnectedUserRights();
            if (this.Rights == null) this.Rights = new List<Right>(0);
	    }

        public List<Right> GetRights(String code)
        {
            List<Right> rights = new List<Right>(0);
            foreach(Right right in this.Rights){
                if(right.functionnality.Equals(code)) rights.Add(right);
            }
            return rights;
        }


        /// <summary>
        /// has Privilege?
        /// </summary>
        /// <param name="code">Functionaliti code</param>
        /// <returns></returns>
        public bool hasPrivilege(String code, RightType? type = null)
        {
            if (user.IsAdmin()) return true;
            if (String.IsNullOrWhiteSpace(code)) return false;
            Functionality functionality = ApplicationManager.Instance.FunctionalityFactory.Get(code, type);
            return hasPrivilege(functionality, type);
        }

        /// <summary>
        /// has Privilege?
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool hasPrivilege(Functionality functionality, RightType? type = null)
        {
            if (!user.active.HasValue || !user.active.Value) return false;
            if (user.IsAdmin()) return true;
            if (functionality == null) return false;
            if (containsPrivilege(functionality.Code, type)) return true;
            return hasPrivilege(functionality.Parent);
        }

        /// <summary>
        /// has Privilege?
        /// </summary>
        /// <param name="code">Functionaliti code</param>
        /// <returns></returns>
        public bool hasAcendentPrivilege(String code)
        {
            if (user.IsAdmin()) return true;
            if (String.IsNullOrWhiteSpace(code)) return false;
            Functionality functionality = ApplicationManager.Instance.FunctionalityFactory.Get(code);
            if (functionality == null) return false;
            return hasPrivilege(functionality.Parent);
        }


        /// <summary>
        /// has Privilege?
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool containsPrivilege(String code, RightType? type = null)
        {
            if (user.IsAdmin()) return true;
            if (String.IsNullOrWhiteSpace(code)) return false;
            return GetRight(code, type) != null;
        }

        /// <summary>
        /// has Privilege?
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private Right GetRight(String code, RightType? type = null)
        {
            foreach (Right right in Rights)
            {
                if (right.functionnality == null) continue;
                if(right.functionnality.Equals(code)) {
                    if (type.HasValue)
                    {
                        if (!String.IsNullOrWhiteSpace(right.rightType) && right.rightType.Equals(type.Value.ToString())) return right;
                    }
                    else if (String.IsNullOrWhiteSpace(right.rightType)) return right;
                }
            }
            return null;
        }


    }
}
