namespace NexGoal.Core.UnitTests.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using Xunit;

    public class NexGoalPermissionAttributeTests
    {
        [Fact]
        public void OnAuthorization_ContextIsNull_ThrowException()
        {
            // Arrange
            var filter = new NexGoalPermissionAttribute();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => filter.OnAuthorization(null));
            Assert.Equal("Value cannot be null.\r\nParameter name: context", ex.Message);
        }

        [Fact]
        public void OnAuthorization_EmployeeIdIsInvalid_ExpectedRedirectToAccessDenied()
        {
            // Arrange
            var httpContext = new DefaultHttpContext
            {
                Session = new MockSession()
            };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var authContext = new AuthorizationFilterContext(actionContext, Enumerable.Empty<IFilterMetadata>().ToList());
            var filter = new NexGoalPermissionAttribute();

            // Act
            filter.OnAuthorization(authContext);

            // Assert
            var redirectResult = Assert.IsAssignableFrom<RedirectToActionResult>(authContext.Result);
            Assert.Equal("error", redirectResult.ControllerName, ignoreCase: true);
            Assert.Equal("AccessDenied", redirectResult.ActionName, ignoreCase: true);
        }

        [Fact]
        public void OnAuthorization_EmployeeIdIsValidAndHasPermissionOnFunction_ExpectedRequestSucceeded()
        {
            // Arrange
            var functionKey = "EmployeeManagement";
            var session = new MockSession();
            session.SetInt32(SessionKey.InternalEmployeeId, 1);
            session.Set<IEnumerable<FunctionSession>>(SessionKey.InternalFunctionList, new List<FunctionSession>
            {
                new FunctionSession { FunctionId = 1, FunctionKey = functionKey, Policy = Policy.Read }
            });
            var httpContext = new DefaultHttpContext
            {
                Session = session
            };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var authContext = new AuthorizationFilterContext(actionContext, Enumerable.Empty<IFilterMetadata>().ToList());
            var filter = new NexGoalPermissionAttribute
            {
                FunctionKey = functionKey,
                Policy = Policy.Read
            };

            // Act
            filter.OnAuthorization(authContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, authContext.HttpContext.Response.StatusCode);
        }
    }
}