namespace NexGoal.Core.Attribute
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public ApiAuthorizeAttribute(string functionKey, Policy policy)
        {
            this.FunctionKey = functionKey;
            this.Policy = policy;
        }

        public string FunctionKey { get; set; }

        public Policy Policy { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                bool hasPermission = CheckUserPermission();
                if (!hasPermission)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
                }
            }
            else
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }

        private bool CheckUserPermission()
        {
            var user = (UserSession)HttpContext.Current.Session[Constants.CurrentSessionKey];

            var hasPermission = user.HasPermission(FunctionKey, Policy);

            return hasPermission;
        }
    }
}