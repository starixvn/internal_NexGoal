namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.FlowSetting;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowSetting;
    using NSubstitute;
    using Xunit;

    public class EmployeeFlowSettingServiceTests
    {
        private IBaseService baseService;
        private ICommonService commonService;

        [Fact]
        public async Task GetEmployeeHaveSettingAsync_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<Employee>
            {
                new Employee { EmployeeId = 1, NickName = "Tim" }
            };
            var service = GetEmployeeFlowSettingService();
            baseService.GetDataAsync<Employee>(Arg.Any<GetEmployeeHaveSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetEmployeeHaveSettingAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeUnsettingAsync_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<Employee>
            {
                new Employee { EmployeeId = 1, NickName = "Tim" }
            };
            var service = GetEmployeeFlowSettingService();
            baseService.GetDataAsync<Employee>(Arg.Any<GetEmployeeUnsettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetEmployeeUnsettingAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeFlowSettingAsync_WhenCalled_ReturnAsExpectedFlowSettings()
        {
            // Arrange
            var employeeFlowSettings = new List<EmployeeFlowSettingModel>
            {
                new EmployeeFlowSettingModel { EmployeeId = 1, NickName = "Tim" }
            };
            var flowSettings = new List<FlowSettingModel>
            {
                new FlowSettingModel { ProcessId = 4, EmployeeId = 1, EvaluatorId = 2, NickName = "Quinn" },
                new FlowSettingModel { ProcessId = 5, EmployeeId = 1, EvaluatorId = 3, NickName = "Anders" }
            };
            var tuple = new Tuple<IEnumerable<EmployeeFlowSettingModel>, IEnumerable<FlowSettingModel>>(employeeFlowSettings, flowSettings);
            var service = GetEmployeeFlowSettingService();
            baseService.FetchMultipleAsync<EmployeeFlowSettingModel, FlowSettingModel>(Arg.Any<GetEmployeeFlowSettingCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetEmployeeFlowSettingAsync();

            // Assert
            Assert.Single(actual);
            Assert.Equal(1, actual.First().EmployeeId);
            Assert.Equal("Tim", actual.First().NickName);

            Assert.Equal(flowSettings[0], actual.First().Evaluator);
            Assert.Equal(flowSettings[1], actual.First().Reviewer);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_ReturnAsExpectedFlowTemplate()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, NickName = "Tim" };
            var flowSettings = new List<FlowSettingModel>
            {
                new FlowSettingModel { ProcessId = 4, EvaluatorId = 2, NickName = "Quinn" },
                new FlowSettingModel { ProcessId = 5, EvaluatorId = 3, NickName = "Anders" }
            };
            var service = GetEmployeeFlowSettingService();
            baseService.GetDataAsync<FlowSettingModel>(Arg.Any<GetEmployeeFlowSettingByIdCriteria>()).Returns(flowSettings);
            commonService.GetEmployeeByIdAsync(1).Returns(employee);

            // Act
            var actual = await service.GetByIdAsync(1);

            // Assert
            Assert.Equal(employee.EmployeeId, actual.EmployeeId);
            Assert.Equal(employee.NickName, actual.NickName);
            Assert.NotNull(actual.Evaluator);
            Assert.NotNull(actual.Reviewer);
        }

        [Fact]
        public async Task EditAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var settings = new List<FlowSettingModel>
            {
                new FlowSettingModel { ProcessId = 1, EvaluatorId = 1 }
            };
            var service = GetEmployeeFlowSettingService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditEmployeeFlowSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditAsync(1, 1, settings);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetEmployeeFlowSettingService();
            baseService.ExecuteNonQueryAsync(Arg.Any<DeleteEmployeeFlowSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.DeleteAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private EmployeeFlowSettingService GetEmployeeFlowSettingService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();
            var service = new EmployeeFlowSettingService(baseService, commonService);

            return service;
        }

        #endregion Helpers
    }
}