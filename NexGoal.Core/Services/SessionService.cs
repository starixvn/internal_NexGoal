using NexGoal.Core.Model;
using NexGoal.Core.Services.Interfaces;
using System.Web;

namespace NexGoal.Core.Services
{
    public class SessionService : ISessionService
    {
        public UserSession CurrentUser
        {
            get
            {
                UserSession currentUser = null;
                var session = HttpContext.Current.Session;
                if (session[Constants.Constants.CurrentSessionKey] != null)
                {
                    currentUser = (UserSession)session[Constants.Constants.CurrentSessionKey];
                }

                return currentUser;
            }

            set
            {
                HttpContext.Current.Session[Constants.Constants.CurrentSessionKey] = value;
            }
        }
    }
}
