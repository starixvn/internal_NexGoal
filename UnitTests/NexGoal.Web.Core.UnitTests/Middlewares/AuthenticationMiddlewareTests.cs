using Microsoft.Extensions.Configuration;

namespace NexGoal.Web.Core.UnitTests.Middlewares
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using NexGoal.Core.Constants;
    using NexGoal.Models.Auth;
    using NexGoal.Services;
    using NexGoal.Web.Core.Middlewares;
    using NSubstitute;
    using Xunit;

    public class AuthenticationMiddlewareTests
    {
        private RequestDelegate next;
        private IAuthService authService;
        private LdapSettings ldapSetting;
        private IConfiguration configuration;

        [Fact]
        public async Task InvokeAsync_Authorized_ReturnAsExpected()
        {
            // Arrange
            var httpContext = new DefaultHttpContext
            {
                Session = new MockSession()
            };
            httpContext.Session.SetInt32(SessionKey.InternalEmployeeId, 1);
            var middleware = GetAuthenticationMiddleware();

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            await authService.DidNotReceive().GetEmployeeByDomainUsernameAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task InvokeAsync_Unauthorized_ReturnAsExpected()
        {
            // Arrange
            var employee = new EmployeeSession
            {
                EmployeeId = 1,
                NickName = "tim",
                FullName = "thang trinh"
            };
            var functions = new List<FunctionSession> {
                new FunctionSession { FunctionId = 1, FunctionKey = "Configuration", Name = "Configuration" }
            };
            var menus = new List<MenuItemSession> {
                new MenuItemSession { MenuId = 1, Name = "Dasboard" }
            };
            var httpContext = new DefaultHttpContext
            {
                Session = new MockSession(),
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "tim.trinh")
                }))
            };
            var middleware = GetAuthenticationMiddleware();
            authService.GetEmployeeByDomainUsernameAsync(Arg.Any<string>()).Returns(employee);
            authService.GetFunctionByEmployeeIdAsync(employee.EmployeeId).Returns(functions);
            authService.GetMenuByEmployeeIdAsync(employee.EmployeeId).Returns(menus);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            Assert.Equal(employee.EmployeeId, httpContext.Session.GetInt32(SessionKey.InternalEmployeeId));
            Assert.Equal(employee.NickName, httpContext.Session.GetString(SessionKey.InternalNickName));
            Assert.Equal(employee.FullName, httpContext.Session.GetString(SessionKey.InternalFullName));

            var actualMenus = httpContext.Session.Get<List<MenuItemSession>>(SessionKey.InternalMenuItemList);
            Assert.Equal(menus.Count, actualMenus.Count);
            Assert.Collection(actualMenus,
                element =>
                {
                    element.MenuId = menus[0].MenuId;
                    element.Name = menus[0].Name;
                    element.Link = menus[0].Link;
                    element.ParentId = menus[0].ParentId;
                    element.Childs = menus[0].Childs;
                });

            var actualFunctions = httpContext.Session.Get<List<FunctionSession>>(SessionKey.InternalFunctionList);
            Assert.Equal(functions.Count, actualFunctions.Count);
            Assert.Collection(actualFunctions,
                element =>
                {
                    element.FunctionId = functions[0].FunctionId;
                    element.Name = functions[0].Name;
                    element.FunctionKey = functions[0].FunctionKey;
                    element.Policy = functions[0].Policy;
                });
        }

        private AuthenticationMiddleware GetAuthenticationMiddleware()
        {
            next = Substitute.For<RequestDelegate>();
            authService = Substitute.For<IAuthService>();
            // IConfiguration configuration
            configuration = Substitute.For<IConfiguration>();
            ldapSetting = new LdapSettings
            {
                Domain = "domain",
                LDAPUrl = "server"
            };
            return new AuthenticationMiddleware(next, authService, ldapSetting, configuration);
        }
    }
}