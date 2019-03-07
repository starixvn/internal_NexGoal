namespace NexGoal.Web.Core.UnitTests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Auth;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class SessionServiceTests
    {
        private ISession _session;
        private SessionService sessionService;

        #region EmployeeId

        [Fact]
        public void EmployeeId_SessionServiceWithSession_GetAndSetCorrectly()
        {
            // Arrange
            var employeeId = 1;
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.EmployeeId = employeeId;

            // Act
            var actual = sessionService.EmployeeId;

            // Assert
            Assert.Equal(employeeId, actual);
        }

        [Fact]
        public void EmployeeId_SessionServiceWithHttpContextAccessor_GetAndSetCorrectly()
        {
            // Arrange
            var employeeId = 1;
            var accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext.Returns(new DefaultHttpContext
            {
                Session = new MockSession()
            });
            sessionService = GetSessionService(accessor);
            sessionService.EmployeeId = employeeId;

            // Act
            var actual = sessionService.EmployeeId;

            // Assert
            Assert.Equal(employeeId, actual);
        }

        #endregion EmployeeId

        [Fact]
        public void NickName_WhenCalled_GetAndSetCorrectly()
        {
            // Arrange
            var nickName = "tim.trinh";
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.NickName = nickName;

            // Act
            var actual = sessionService.NickName;

            // Assert
            Assert.Equal(nickName, actual);
        }

        [Fact]
        public void FullName_WhenCalled_GetAndSetCorrectly()
        {
            // Arrange
            var fullName = "tim";
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.FullName = fullName;

            // Act
            var actual = sessionService.FullName;

            // Assert
            Assert.Equal(fullName, actual);
        }

        [Fact]
        public void MenuItems_WhenCalled_GetAndSetCorrectly()
        {
            // Arrange
            IEnumerable<MenuItemSession> menuItems = new List<MenuItemSession>
            {
                new MenuItemSession { MenuId = 1, Name = "Employee Management" }
            };
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.MenuItems = menuItems;

            // Act
            var actual = sessionService.MenuItems;

            // Assert
            Assert.Equal(menuItems.Count(), actual.Count());
            Assert.Equal(menuItems.First().MenuId, actual.First().MenuId);
            Assert.Equal(menuItems.First().Name, actual.First().Name);
            Assert.Equal(menuItems.First().Description, actual.First().Description);
            Assert.Equal(menuItems.First().Link, actual.First().Link);
            Assert.Equal(menuItems.First().ParentId, actual.First().ParentId);
            Assert.Equal(menuItems.First().DisplayOrder, actual.First().DisplayOrder);
        }

        [Fact]
        public void Functions_WhenCalled_GetAndSetCorrectly()
        {
            // Arrange
            IEnumerable<FunctionSession> functions = new List<FunctionSession>
            {
                new FunctionSession { FunctionId = 1, FunctionKey = "EmployeeManagement" }
            };
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = functions;

            // Act
            var actual = sessionService.Functions;

            // Assert
            Assert.Equal(functions.Count(), actual.Count());
            Assert.Equal(functions.First().FunctionId, actual.First().FunctionId);
            Assert.Equal(functions.First().Name, actual.First().Name);
            Assert.Equal(functions.First().FunctionKey, actual.First().FunctionKey);
        }

        #region HasPermissionOnFunction

        [Fact]
        public void HasPermissionOnFunction_FunctionKeyIsNotExisted_ReturnFalse()
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionSession>
            {
                new FunctionSession { FunctionId = 1, FunctionKey = "EmployeeManagement" }
            };

            // Act
            var actual = sessionService.HasPermissionOnFunction("test", Policy.Read);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData(Policy.Read)]
        [InlineData(Policy.Host)]
        public void HasPermissionOnFunction_HasNotPermissionOnFunction_ReturnFalse(Policy policy)
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionSession>
            {
                new FunctionSession { FunctionId = 1, FunctionKey = "EmployeeManagement" }
            };

            // Act
            var actual = sessionService.HasPermissionOnFunction("EmployeeManagement", policy);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void HasPermissionOnFunction_HasReadPermissionOnFunction_ReturnTrue()
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionSession>
            {
                new FunctionSession { FunctionId = 1, FunctionKey = "EmployeeManagement", Policy = Policy.Read }
            };

            // Act
            var actual = sessionService.HasPermissionOnFunction("EmployeeManagement", Policy.Read);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(Policy.Read)]
        [InlineData(Policy.Host)]
        public void HasPermissionOnFunction_HasHostPermissionOnFunction_ReturnTrue(Policy policy)
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.Functions = new List<FunctionSession>
            {
                new FunctionSession { FunctionId = 1, FunctionKey = "EmployeeManagement", Policy = Policy.Host }
            };

            // Act
            var actual = sessionService.HasPermissionOnFunction("EmployeeManagement", policy);

            // Assert
            Assert.True(actual);
        }

        #endregion HasPermissionOnFunction

        [Fact]
        public void ClearAll_WhenCalled_ExpectedRemoveAllEntriesInSession()
        {
            // Arrange
            _session = new MockSession();
            sessionService = GetSessionService(_session);
            sessionService.EmployeeId = 1;

            // Act
            sessionService.ClearAll();

            // Assert
            Assert.Equal(0, sessionService.EmployeeId);
        }

        #region Helpers

        private static SessionService GetSessionService(ISession session)
        {
            var service = new SessionService(session);

            return service;
        }

        private static SessionService GetSessionService(IHttpContextAccessor accessor)
        {
            var service = new SessionService(accessor);

            return service;
        }

        #endregion Helpers
    }
}