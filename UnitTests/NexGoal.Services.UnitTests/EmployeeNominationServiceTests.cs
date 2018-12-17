namespace NexGoal.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeNomination;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
    using NSubstitute;
    using Xunit;

    public class EmployeeNominationServiceTests
    {
        private IBaseService baseService;
        private ICommonService commonService;

        [Fact]
        public async Task GetNominationPeriodsForNominator_WhenCalled_ReturnExpectedResult()
        {
            var expectedPeriods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel
                {
                    NominationPeriodId = 1,
                    Quarter = 1,
                    Year = 2018
                }
            };

            var service = GetService();
            baseService.GetDataAsync<NominationPeriodModel>(Arg.Any<GetNominationPeriodsForNominatorCriteria>()).Returns(expectedPeriods);

            var result = await service.GetNominationPeriodsForNominatorAsync(1);

            Assert.Equal(expectedPeriods.Count, result.Count());
        }

        [Fact]
        public async Task Create_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeNominationCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.CreateAsync(new CreateEmployeeNominationModel());

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task IsNominator_ValidData_WhenCalledReturnExpectedResult()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<bool>(Arg.Any<CheckIsNominatorCriteria>()).Returns(true);

            var result = await service.IsNominator(125);

            Assert.True(result);
        }

        [Fact]
        public async Task GetNominationByRecordIdAsync_InvalidData_ReturnNull()
        {
            var service = GetService();

            baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetNominationByRecordIdCriteria>())
                .Returns(new Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>(
                    Enumerable.Empty<PerformanceRecordModel>(), Enumerable.Empty<NominationPeriodModel>()));

            var result = await service.GetNominationByRecordIdAsync(0);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetNominationByRecordIdAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var performanceRecord = new PerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "2",
                CreatorId = 1
            };

            baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetNominationByRecordIdCriteria>())
                .Returns(new Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>(
                    new List<PerformanceRecordModel> { performanceRecord }, Enumerable.Empty<NominationPeriodModel>()));
            commonService.GetEmployeeByIdAsync(performanceRecord.CreatorId).Returns(new EmployeeBaseModel { EmployeeId = 1 });
            commonService.GetEmployeeByIdAsync(2).Returns(new EmployeeBaseModel { EmployeeId = 2 });

            var result = await service.GetNominationByRecordIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(result.CreatorId, result.Creator.EmployeeId);
            Assert.Equal(result.EmployeeIds, result.Employees.FirstOrDefault().EmployeeId.ToString());
        }

        [Fact]
        public async Task GetMemberNominationsAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var performanceRecord = new PerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "2",
                CreatorId = 1,
                NominationPeriodId = 1,
                TypeId = (int)PerformanceRecordType.Nomination,
            };

            var systemNominations = new List<PerformanceRecordModel> {
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 2,
                    EmployeeIds = "2",
                    CreatorId = -1,
                    NominationPeriodId = 1,
                    NominationStatusId = 2,
                    TypeId = (int)PerformanceRecordType.SystemNomination,
                },
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 2,
                    EmployeeIds = "3",
                    CreatorId = -1,
                    NominationPeriodId = 1,
                    NominationStatusId = 2,
                    TypeId = (int)PerformanceRecordType.SystemNomination,
                }
            };

            baseService.FetchMultipleAsync<PerformanceRecordModel, NominationPeriodModel>(Arg.Any<GetMemberNominationCriteria>())
                .Returns(new Tuple<IEnumerable<PerformanceRecordModel>, IEnumerable<NominationPeriodModel>>(
                    new List<PerformanceRecordModel>(systemNominations) { performanceRecord },
                    new List<NominationPeriodModel> { new NominationPeriodModel { NominationPeriodId = 1 } }
                    ));

            commonService.GetEmployeesAsync(isEnabled: null).Returns(new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1 },
                new EmployeeBaseModel { EmployeeId = 2 }
            });

            var result = await service.GetMemberNominationsAsync(1, null);

            Assert.NotEmpty(result);
            Assert.All(result, record =>
            {
                Assert.Equal(record.CreatorId, record.Creator.EmployeeId);
                Assert.Equal(record.NominationPeriodId, record.NominationPeriod.NominationPeriodId);
            });
        }

        [Fact]
        public async Task GetNominationsByPeriodIdsAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var performanceRecord = new PerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "2",
                CreatorId = 1,
                NominationPeriodId = 1
            };

            var nomination = new NominationModel
            {
                NominationId = 1,
                NomineeId = 2,
                NominationPeriodId = 1
            };

            baseService.FetchMultipleAsync<NominationModel, PerformanceRecordModel>(Arg.Any<GetNominationsByPeriodIdsCriteria>())
                .Returns(new Tuple<IEnumerable<NominationModel>, IEnumerable<PerformanceRecordModel>>(
                        new List<NominationModel> { nomination },
                        new List<PerformanceRecordModel> { performanceRecord }
                    ));

            commonService.GetEmployeesAsync(isEnabled: null).Returns(new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1 },
                new EmployeeBaseModel { EmployeeId = 2 }
            });

            var result = await service.GetNominationsByPeriodIdsAsync("1");

            Assert.NotEmpty(result);
            Assert.Collection(result, item =>
            {
                Assert.Equal(item.NomineeId, item.Nominee.EmployeeId);
                Assert.NotEmpty(item.Nominators);
            });
        }

        [Fact]
        public async Task Edit_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Any<EditEmployeeNominationCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.EditAsync(new EditEmployeeNominationModel());

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task EditStatus_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Any<EditNominationStatusCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.EditStatusAsync(1, 2, 367);

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task Delete_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Any<DeleteNominationCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.DeleteAsync(1, 1);

            Assert.True(result.IsSuccessful);
        }

        private EmployeeNominationService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();

            var service = new EmployeeNominationService(baseService, commonService);

            return service;
        }
    }
}