namespace NexGoal.Web.Core.Filters
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using NexGoal.Core.Enumerations;
    using NexGoal.Web.Core.Services;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NexGoalPermissionAttribute : Attribute, IAuthorizationFilter
    {
        public string FunctionKey { get; set; }

        public Policy Policy { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context?.HttpContext == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var currentSession = new SessionService(context.HttpContext.Session);

                if (currentSession.EmployeeId <= 0 || !currentSession.HasPermissionOnFunction(FunctionKey, Policy))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                }
            }
        }
    }
}