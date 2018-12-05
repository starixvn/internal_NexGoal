namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class EmployeeControllerTests
    {
        private IEmployeeService employeeService;
        private ISessionService sessionService;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewResult()
        {
            var controller = GetEmployeeController();
            sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host).Returns(true);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<bool>(viewResult.Model);
            Assert.True(viewModel);
        }

        [Fact]
        public async Task GetEmployees_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var expected = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel
                {
                    EmployeeId = 1,
                    Code = "123",
                }
            };
            var param = new GetEmployeeParam();
            var controller = GetEmployeeController();
            employeeService.GetEmployeesAsync(param).Returns(expected);

            // Act
            var result = await controller.GetEmployees(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<EmployeeBaseModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeById_WhenCalled_ReturnExpectedData()
        {
            var expectedResult = new EmployeeBaseModel
            {
                EmployeeId = 1,
                Code = "123",
            };

            var controller = GetEmployeeController();
            employeeService.GetEmployeeByIdAsync(1).Returns(expectedResult);

            var result = await controller.GetEmployeeById(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var employees = Assert.IsAssignableFrom<EmployeeBaseModel>(jsonResult.Value);
            Assert.Equal(expectedResult, employees);
        }

        [Fact]
        public async Task GetRolesByEmployeeId_WhenCalled_ReturnExpectedData()
        {
            var expectedResult = new List<EmployeeRoleModel>
            {
                new EmployeeRoleModel
                {
                    RoleId = 1,
                    HasRole = true
                }
            };

            var controller = GetEmployeeController();
            employeeService.GetRolesByEmployeeIdAsync(1).Returns(expectedResult);

            var result = await controller.GetRolesByEmployeeId(1);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var roles = Assert.IsAssignableFrom<IEnumerable<EmployeeRoleModel>>(jsonResult.Value);
            Assert.Equal(expectedResult, roles);
        }

        [Fact]
        public async Task EditEmployeeRoles_ValidModel_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetEmployeeController();
            sessionService.EmployeeId = 1;
            employeeService.EditEmployeeRolesAsync(Arg.Any<EditEmployeeRolesModel>()).Returns(expected);

            // Act
            var result = await controller.EditEmployeeRoles(new EditEmployeeRolesModel
            {
                EmployeeId = 1,
                SelectedRoleIds = "1,2"
            });

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        private EmployeeController GetEmployeeController()
        {
            employeeService = Substitute.For<IEmployeeService>();
            sessionService = Substitute.For<ISessionService>();

            return new EmployeeController(employeeService, sessionService);
        }
    }
}