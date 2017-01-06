namespace NexGoal.Core.Attribute
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly ISessionService sessionService;
        
        public ApiAuthorizeAttribute()
        {
            this.sessionService = new SessionService();
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