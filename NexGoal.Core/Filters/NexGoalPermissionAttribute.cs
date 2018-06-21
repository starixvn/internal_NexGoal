namespace NexGoal.Core.Filters
{
    using System;
    using Enumerations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Services;

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

            var currentSession = new SessionService(context.HttpContext.Session);

            if (currentSession.EmployeeId <= 0 || !currentSession.HasPermissionOnFunction(FunctionKey, Policy))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
            }
        }
    }
}