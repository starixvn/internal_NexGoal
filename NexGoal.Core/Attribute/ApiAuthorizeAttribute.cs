namespace NexGoal.Core.Attribute
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private ISessionService sessionService;

        public ApiAuthorizeAttribute(string functionKey, Policy policy)
            : this(functionKey, policy, null)
        {
        }

        public ApiAuthorizeAttribute(
            string functionKey,
            Policy policy,
            ISessionService sessionService)
        {
            this.FunctionKey = functionKey;
            this.Policy = policy;
            this.sessionService = sessionService ?? new SessionService();
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
            var user = this.sessionService.CurrentUser;

            return user == null ? false : user.HasPermission(FunctionKey, Policy);
        }
    }
}