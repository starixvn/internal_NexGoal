namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;
    using NSubstitute;
    using Xunit;

    public class PeriodServiceTests
    {
        private IBaseService baseService;
        private ICommonService commonService;
        private ICryptoService cryptoService;

        [Fact]
        public async Task GetPeriodAsync_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var expected = new List<PeriodModel>
            {
                new PeriodModel
                {
                    PeriodId = 1,
                    TargetTypeId = 1
                }
            };
            var service = GetPeriodService();
            baseService.GetDataAsync<PeriodModel>(Arg.Any<GetPeriodsCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetPeriodAsync(null);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var expected = new PeriodModel
            {
                PeriodId = 1,
                TargetTypeId = 1
            };

            var service = GetPeriodService();
            baseService.GetDataEntityAsync<PeriodModel>(Arg.Any<GetPeriodCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetByIdAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPeriodProcessSettingAsync_WhenCalled_ReturnAsExpectedProcessSettings()
        {
            // Arrange
            var processSettings = new List<PeriodProcessSettingModel>
            {
                new PeriodProcessSettingModel { PeriodId = 1, ProcessId = 1, Duration = 5 }
            };
            var service = GetPeriodService();
            baseService.GetDataAsync<PeriodProcessSettingModel>(Arg.Any<GetPeriodProcessSettingCriteria>()).Returns(processSettings);

            // Act
            var actual = await service.GetPeriodProcessSettingAsync(1);

            // Assert
            Assert.Equal(processSettings, actual);
        }

        [Fact]
        public async Task CreateAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var model = new CreatePeriodModel
            {
                Name = "",
                TargetTypeId = 1
            };
            var expected = new CommandResult { Code = 0 };
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreatePeriodCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreateAsync(1, model);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditStatusAsync_ValidModel_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult { Code = 0 };
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdatePeriodStatusCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditStatusAsync(1, 1, true);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region EditAsync

        [Fact]
        public async Task EditAsync_IsUnsuccessful_ReturnExpectedCommandResult()
        {
            // Arrange
            var model = new EditPeriodModel
            {
                PeriodId = 1,
                Name = ""
            };
            var expected = new CommandResult();
            var criteria = new EditPeriodCriteria();
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Do<EditPeriodCriteria>(x => criteria = x)).Returns(expected);
            commonService.RemoveInvalidXmlChars(model.Description).Returns(criteria.Description);

            // Act
            var actual = await service.EditAsync(1, model);

            // Assert
            Assert.Equal(expected, actual);

            Assert.Equal(model.PeriodId, criteria.PeriodId);
            Assert.Equal(model.Name, criteria.Name);
            Assert.Equal(model.StartDate, criteria.StartDate);
            Assert.Equal(model.EndDate, criteria.EndDate);
            Assert.Equal(model.DisciplineStartDate, criteria.DisciplineStartDate);
            Assert.Equal(model.DisciplineEndDate, criteria.DisciplineEndDate);
            Assert.Equal(model.FocusStartDate, criteria.FocusStartDate);
            Assert.Equal(model.FocusEndDate, criteria.FocusEndDate);
            Assert.Equal(model.Description, criteria.Description);
            Assert.Equal(model.IsActive, criteria.IsActive);
            Assert.Equal(1, criteria.ModifiedBy);
        }

        #endregion EditAsync

        [Fact]
        public async Task DeleteAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult { Code = 0 };
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<DeletePeriodCriteria>()).Returns(expected);

            // Act
            var actual = await service.DeleteAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditProcessSettingAsync_ValidModel_ReturnExpectedCommandResult()
        {
            // Arrange
            var settings = new List<EditProcessSettingModel>
            {
                new EditProcessSettingModel
                {
                    PeriodId = 1,
                    Duration = 1,
                    NoticeAtBegin = true,
                    RemindDay = 1
                }
            };
            var expected = new CommandResult { Code = 0 };
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditProcessSettingCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditProcessSettingAsync(1, 1, settings);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeSettingAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var settings = (
                new List<PARecordModel>
                {
                    new PARecordModel
                    {
                        PARecordId = 1,
                        EmployeeId = 1,
                        PeriodId = 1,
                        StatusId = 1
                    }
                },
                new List<PAProcessModel>
                {
                    new PAProcessModel
                    {
                        PARecordId = 1,
                        ProcessId = 2,
                        EvaluatorId = 1,
                        StatusId = 1
                    }
                });

            var service = GetPeriodService();
            baseService.FetchMultipleAsync<PARecordModel, PAProcessModel>(Arg.Any<GetEmployeeSettingsCriteria>()).Returns(settings);
            commonService.GetEmployeesAsync(isEnabled: null).Returns(new List<Employee>());

            // Act
            var result = await service.GetEmployeeSettingAsync(1, null, null, null);

            // Assert
            Assert.Single(result);
            Assert.Single(result.FirstOrDefault().Processes);
        }

        [Fact]
        public async Task CreateEmployeeSettingAsync_ValidData_ReturnExpectedCommanResult()
        {
            // Arrange
            var expected = new CommandResult { Code = 0 };
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeSettingsCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreateEmployeeSettingAsync(1, 1, "1", null);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region EditEmployeeSettingAsync

        [Fact]
        public async Task EditEmployeeSettingAsync_IsUnsuccessful_ReturnExpectedCommanResult()
        {
            // Arrange
            var expected = new CommandResult();
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditEmployeeSettingsCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditEmployeeSettingAsync(1, 1, new[] { 1, 2 }, new List<EditEmployeeSettingModel>());

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion EditEmployeeSettingAsync

        [Fact]
        public async Task EditEmployeeSettingStatusAsync_ValidData_ReturnExpectedCommanResult()
        {
            // Arrange
            var expected = new CommandResult { Code = 0 };
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditEmployeeSettingStatusCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditEmployeeSettingStatusAsync(1, 1, true);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeSettingByIdAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var settings = (
                new List<PARecordModel>
                {
                    new PARecordModel
                    {
                        PARecordId = 1,
                        EmployeeId = 1,
                        PeriodId = 1,
                        StatusId = 1
                    }
                },
                new List<PAProcessModel>
                {
                    new PAProcessModel
                    {
                        PARecordId = 1,
                        ProcessId = 2,
                        EvaluatorId = 1,
                        StatusId = 1
                    }
                });

            var service = GetPeriodService();
            baseService.FetchMultipleAsync<PARecordModel, PAProcessModel>(Arg.Any<GetEmployeeSettingByIdCriteria>()).Returns(settings);
            commonService.GetEmployeesAsync(isEnabled: null).Returns(new List<Employee>());

            // Act
            var result = await service.GetEmployeeSettingByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Processes);
        }

        [Fact]
        public async Task GeneratePAAsync_IsUnsuccessful_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<GeneratePACriteria>()).Returns(expected);

            // Act
            var actual = await service.GeneratePAAsync(1, "1", 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private PeriodService GetPeriodService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();
            cryptoService = Substitute.For<ICryptoService>();
            var service = new PeriodService(baseService, commonService, cryptoService);

            return service;
        }

        #endregion Helpers
    }
}