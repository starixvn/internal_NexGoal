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
    using NexGoal.ViewModels.Employee;
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
            var expectedResult = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel
                {
                    EmployeeId = 1,
                    Code = "123",
                }
            };

            var controller = GetEmployeeController();
            employeeService.GetEmployeesAsync(string.Empty, null, null, false).Returns(expectedResult);

            var result = await controller.GetEmployees(string.Empty, null, null, false);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsAssignableFrom<IEnumerable<EmployeeBaseModel>>(jsonResult.Value);
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
            var controller = GetEmployeeController();
            sessionService.EmployeeId = 1;
            employeeService.EditEmployeeRolesAsync(Arg.Any<EditEmployeeRolesModel>()).Returns(new CommandResult
            {
                Code = 0
            });

            var result = await controller.EditEmployeeRoles(new EditEmployeeRolesViewModel
            {
                EmployeeId = 1,
                SelectedRoleIds = "1,2"
            });

            var jsonResult = Assert.IsType<JsonResult>(result);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.True(commandResult.IsSuccessful);
        }

        private EmployeeController GetEmployeeController()
        {
            employeeService = Substitute.For<IEmployeeService>();
            sessionService = Substitute.For<ISessionService>();

            return new EmployeeController(employeeService, sessionService);
        }
    }
}