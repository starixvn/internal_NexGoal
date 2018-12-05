namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Employee;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NSubstitute;
    using Xunit;

    public class EmployeeServiceTests
    {
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private ICommonService commonService;

        [Fact]
        public async Task GetEmployeesAsync_WhenCalled_ReturnExpectedEmployees()
        {
            // Arrange
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel
                {
                    EmployeeId = 1,
                    FullName = "Eha",
                    NickName ="Hea"
                }
            };
            var param = new GetEmployeeParam();
            var service = GetEmployeeService();
            baseService.GetDataAsync<EmployeeBaseModel>(Arg.Any<GetEmployeesCriteria>()).Returns(employees);

            // Act
            var result = await service.GetEmployeesAsync(param);

            // Assert
            Assert.Equal(employees, result);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WhenCalled_ReturnExpectedEmployee()
        {
            var employee = new EmployeeBaseModel
            {
                EmployeeId = 1,
                FullName = "Eha",
                NickName = "Hea"
            };

            var service = GetEmployeeService();
            baseService.GetDataEntityAsync<EmployeeBaseModel>(Arg.Any<GetEmployeeByIdCriteria>()).Returns(employee);

            var result = await service.GetEmployeeByIdAsync(1);

            Assert.Equal(employee, result);
        }

        [Fact]
        public async Task GetRolesByEmployeeIdAsync_WhenCalled_ReturnExpectedResults()
        {
            var expectedRoles = new List<EmployeeRoleModel>
            {
                new EmployeeRoleModel
                {
                    RoleId = 1,
                    Role= "HR",
                    HasRole = true
                },
                new EmployeeRoleModel
                {
                    RoleId = 2,
                    Role= "SA",
                    HasRole = false
                }
            };

            var service = GetEmployeeService();
            baseService.GetDataAsync<EmployeeRoleModel>(Arg.Any<GetRolesByEmployeeIdCriteria>()).Returns(expectedRoles);

            var result = await service.GetRolesByEmployeeIdAsync(1);

            Assert.Equal(expectedRoles.Count, result.Count());
        }

        [Fact]
        public async Task EditEmployeeRolesAsync_ValidModel_ReturnExpectedCommandResult()
        {
            var expectedCommandResult = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };

            var service = GetEmployeeService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditEmployeeRolesCriteria>()).Returns(expectedCommandResult);

            var result = await service.EditEmployeeRolesAsync(new EditEmployeeRolesModel { EmployeeId = 1, SelectedRoleIds = "1,2,3", ModifiedBy = 2 });

            Assert.Equal(expectedCommandResult.Code, result.Code);
        }

        [Fact]
        public async Task GetMemberAsync_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
            };
            var service = GetEmployeeService();
            baseService.GetDataAsync<EmployeeBaseModel>(Arg.Any<GetMemberByActorIdCriteria>()).Returns(employees);

            // Act
            var actual = await service.GetMemberAsync(1);

            // Assert
            Assert.Equal(employees, actual);
        }

        [Fact]
        public async Task GetGroupAsync_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var groups = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Name = "Nexus" }
            };
            var service = GetEmployeeService();
            baseService.GetDataAsync<GroupBaseModel>(Arg.Any<GetGroupByActorIdCriteria>()).Returns(groups);

            // Act
            var actual = await service.GetGroupAsync(1);

            // Assert
            Assert.Equal(groups, actual);
        }

        private EmployeeService GetEmployeeService()
        {
            baseService = Substitute.For<IBaseService>();
            cryptoService = Substitute.For<ICryptoService>();
            commonService = Substitute.For<ICommonService>();
            var service = new EmployeeService(baseService, cryptoService, commonService);

            return service;
        }
    }
}