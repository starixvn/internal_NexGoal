namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Employee;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PARecord;
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

        [Fact]
        public async Task GetRoleAccordingToMemberAsync_WhenCalled_ReturnAsExpectedRole()
        {
            // Arrange
            var expected = 1;
            var service = GetEmployeeService();
            baseService.GetDataEntityAsync<int>(Arg.Any<GetRoleAccordingToMemberCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetRoleAccordingToMemberAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        #region GetPerformanceAppraisalResultAsync

        [Fact]
        public async Task GetPerformanceAppraisalResultAsync_DataAreNotExist_ReturnEmpty()
        {
            // Arrange
            var performanceAppraisals = new List<PARecordModel>();
            var periods = new List<PeriodBaseModel>();
            var tuple = new Tuple<IEnumerable<PARecordModel>, IEnumerable<PeriodBaseModel>>(performanceAppraisals, periods);
            var service = GetEmployeeService();
            baseService.FetchMultipleAsync<PARecordModel, PeriodBaseModel>(Arg.Any<GetPerformanceAppraisalResult>()).Returns(tuple);

            // Act
            var actual = await service.GetPerformanceAppraisalResultAsync(1);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetPerformanceAppraisalResultAsync_MappingData_ReturnAsExpectedPerformanceAppraisals()
        {
            // Arrange
            var performanceAppraisals = new List<PARecordModel>
            {
                new PARecordModel { PARecordId = 1, GradeId = "cipher text", PeriodId = 1 }
            };
            var periods = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" }
            };
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
            var tuple = new Tuple<IEnumerable<PARecordModel>, IEnumerable<PeriodBaseModel>>(performanceAppraisals, periods);
            var service = GetEmployeeService();
            baseService.FetchMultipleAsync<PARecordModel, PeriodBaseModel>(Arg.Any<GetPerformanceAppraisalResult>()).Returns(tuple);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await service.GetPerformanceAppraisalResultAsync(1);

            // Assert
            Assert.Equal(performanceAppraisals, actual);
            Assert.Equal(performanceGrades.First(), actual.First().Grade);
            Assert.Equal(periods.First(), actual.First().Period);
        }

        #endregion GetPerformanceAppraisalResultAsync

        #region GetMemberPerformanceAppraisalResultAsync

        [Fact]
        public async Task GetMemberPerformanceAppraisalResultAsync_DataAreNotExist_ReturnEmpty()
        {
            // Arrange
            var performanceAppraisals = new List<PARecordModel>();
            var periods = new List<PeriodBaseModel>();
            var tuple = new Tuple<IEnumerable<PARecordModel>, IEnumerable<PeriodBaseModel>>(performanceAppraisals, periods);
            var service = GetEmployeeService();
            baseService.FetchMultipleAsync<PARecordModel, PeriodBaseModel>(Arg.Any<GetMemberPerformanceAppraisalResult>()).Returns(tuple);

            // Act
            var actual = await service.GetMemberPerformanceAppraisalResultAsync(1);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetMemberPerformanceAppraisalResultAsync_MappingData_ReturnAsExpectedPerformanceAppraisals()
        {
            // Arrange
            var performanceAppraisals = new List<PARecordModel>
            {
                new PARecordModel { PARecordId = 1, GradeId = "cipher text", EmployeeId = 1, PeriodId = 1 }
            };
            var periods = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "2h 2018" }
            };
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
            };
            IEnumerable<PerformanceGradeModel> performanceGrades = GetPerformanceGradeForTest();
            var tuple = new Tuple<IEnumerable<PARecordModel>, IEnumerable<PeriodBaseModel>>(performanceAppraisals, periods);
            var service = GetEmployeeService();
            baseService.FetchMultipleAsync<PARecordModel, PeriodBaseModel>(Arg.Any<GetMemberPerformanceAppraisalResult>()).Returns(tuple);
            commonService.GetEmployeesAsync().Returns(employees);
            commonService.GetPerformanceGradeAsync().Returns(performanceGrades);
            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await service.GetMemberPerformanceAppraisalResultAsync(1);

            // Assert
            Assert.Equal(performanceAppraisals, actual);
            Assert.Equal(employees.First(), actual.First().Employee);
            Assert.Equal(performanceGrades.First(), actual.First().Grade);
            Assert.Equal(periods.First(), actual.First().Period);
        }

        #endregion GetMemberPerformanceAppraisalResultAsync

        #region GetNominationResultAsync

        [Fact]
        public async Task GetNominationResultAsync_DataAreNotExist_ReturnEmpty()
        {
            // Arrange
            var nominations = new List<EmployeeNominationModel>();
            var periods = new List<NominationPeriodModel>();
            var tuple = new Tuple<IEnumerable<EmployeeNominationModel>, IEnumerable<NominationPeriodModel>>(nominations, periods);
            var service = GetEmployeeService();
            baseService.FetchMultipleAsync<EmployeeNominationModel, NominationPeriodModel>(Arg.Any<GetNominationResultCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetNominationResultAsync(1);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetNominationResultAsync_MappingData_ReturnAsExpectedNominations()
        {
            // Arrange
            var nominations = new List<EmployeeNominationModel>
            {
                new EmployeeNominationModel { NominationPeriodId = 1 }
            };
            var periods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel { NominationPeriodId = 1, Quarter = 4, Year = 2018 }
            };
            var tuple = new Tuple<IEnumerable<EmployeeNominationModel>, IEnumerable<NominationPeriodModel>>(nominations, periods);
            var service = GetEmployeeService();
            baseService.FetchMultipleAsync<EmployeeNominationModel, NominationPeriodModel>(Arg.Any<GetNominationResultCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetNominationResultAsync(1);

            // Assert
            Assert.Equal(nominations, actual);
            Assert.Equal(periods.First(), actual.First().Period);
        }

        #endregion GetNominationResultAsync

        #region GetMemberNominationResultAsync

        [Fact]
        public async Task GetMemberNominationResultAsync_DataAreNotExist_ReturnEmpty()
        {
            // Arrange
            var nominations = new List<EmployeeNominationModel>();
            var periods = new List<NominationPeriodModel>();
            var tuple = new Tuple<IEnumerable<EmployeeNominationModel>, IEnumerable<NominationPeriodModel>>(nominations, periods);
            var service = GetEmployeeService();
            baseService.FetchMultipleAsync<EmployeeNominationModel, NominationPeriodModel>(Arg.Any<GetMemberNominationResultCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetMemberNominationResultAsync(1);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetMemberNominationResultAsync_MappingData_ReturnAsExpectedNominations()
        {
            // Arrange
            var nominations = new List<EmployeeNominationModel>
            {
                new EmployeeNominationModel { NominationPeriodId = 1 }
            };
            var periods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel { NominationPeriodId = 1, Quarter = 4, Year = 2018 }
            };
            var tuple = new Tuple<IEnumerable<EmployeeNominationModel>, IEnumerable<NominationPeriodModel>>(nominations, periods);
            var service = GetEmployeeService();
            baseService.FetchMultipleAsync<EmployeeNominationModel, NominationPeriodModel>(Arg.Any<GetMemberNominationResultCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetMemberNominationResultAsync(1);

            // Assert
            Assert.Equal(nominations, actual);
            Assert.Equal(periods.First(), actual.First().Period);
        }

        #endregion GetMemberNominationResultAsync

        #region Helpers

        private IEnumerable<PerformanceGradeModel> GetPerformanceGradeForTest()
        {
            var performanceGrades = new List<PerformanceGradeModel>
            {
                new PerformanceGradeModel { GradeId = 1, Name = "EE" },
                new PerformanceGradeModel { GradeId = 2, Name = "EE+" }
            };

            return performanceGrades;
        }

        private EmployeeService GetEmployeeService()
        {
            baseService = Substitute.For<IBaseService>();
            cryptoService = Substitute.For<ICryptoService>();
            commonService = Substitute.For<ICommonService>();
            var service = new EmployeeService(baseService, cryptoService, commonService);

            return service;
        }

        #endregion Helpers
    }
}