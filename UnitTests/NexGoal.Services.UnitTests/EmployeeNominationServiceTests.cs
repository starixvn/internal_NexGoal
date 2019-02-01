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
    using NexGoal.Repositories;
    using NSubstitute;
    using Xunit;

    public class EmployeeNominationServiceTests
    {
        private IBaseService baseService;
        private ICommonService commonService;
        private IEmployeeNominationRepository nominationRepository;

        [Fact]
        public async Task CreateAsync_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeNominationCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.CreateAsync(new CreateEmployeeNominationModel());

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task DeleteAsync_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Any<DeleteNominationCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.DeleteAsync(1, 1);

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task EditAsync_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Any<EditEmployeeNominationCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.EditAsync(new EditEmployeeNominationModel());

            Assert.True(result.IsSuccessful);
        }

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
        public async Task GetNominationResultAsync_WhenCalled_ReturnAsExpectedNominations()
        {
            // Arrange
            var nominations = new List<EmployeeNominationModel>
            {
                new EmployeeNominationModel { Period = new NominationPeriodModel { NominationPeriodId = 1, Quarter = 4, Year = 2018 } }
            };
            var service = GetService();
            nominationRepository.GetNominationResultAsync(Arg.Any<GetNominationResultCriteria>()).Returns(nominations);

            // Act
            var actual = await service.GetNominationResultAsync(1);

            // Assert
            Assert.Equal(nominations, actual);
        }

        [Fact]
        public async Task GetMemberNominationResultAsync_WhenCalled_ReturnAsExpectedNominations()
        {
            // Arrange
            var nominations = new List<EmployeeNominationModel>
            {
                new EmployeeNominationModel { Period = new NominationPeriodModel { NominationPeriodId = 1, Quarter = 4, Year = 2018 } }
            };
            var service = GetService();
            nominationRepository.GetNominationResultAsync(Arg.Any<GetMemberNominationResultCriteria>()).Returns(nominations);

            // Act
            var actual = await service.GetMemberNominationResultAsync(1);

            // Assert
            Assert.Equal(nominations, actual);
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
            commonService.GetEmployeeByIdAsync(performanceRecord.CreatorId).Returns(new Employee { EmployeeId = 1 });
            commonService.GetEmployeeByIdAsync(2).Returns(new Employee { EmployeeId = 2 });

            var result = await service.GetNominationByRecordIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(result.CreatorId, result.Creator.EmployeeId);
            Assert.Equal(result.EmployeeIds, result.Employees.FirstOrDefault().EmployeeId.ToString());
        }

        [Fact]
        public async Task GetMemberNominationAsync_ValidData_ReturnExpectedResult()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 2, NickName = "elton" },
                new Employee { EmployeeId = 3, NickName = "oliver" }
            };
            var performanceRecords = new List<PerformanceRecordModel> {
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 1,
                    NominationPeriod = new NominationPeriodModel { NominationPeriodId = 1, Quarter = 4, Year = 2018 },
                    TypeId = (int)PerformanceRecordType.Nomination
                },
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 2,
                    Employees = new List<Employee>{ new Employee { EmployeeId = 2, NickName = "elton" } },
                    NominationPeriod = new NominationPeriodModel { NominationPeriodId = 2, Year = 2018 },
                    NominationStatusId = 2,
                    TypeId = (int)PerformanceRecordType.SystemNomination
                },
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 3,
                    Employees = new List<Employee>{ new Employee { EmployeeId = 3, NickName = "oliver" } },
                    NominationPeriod = new NominationPeriodModel { NominationPeriodId = 2, Year = 2018 },
                    NominationStatusId = 2,
                    TypeId = (int)PerformanceRecordType.SystemNomination
                }
            };
            var service = GetService();
            nominationRepository.GetMemberNominationAsync(Arg.Any<GetMemberNominationCriteria>()).Returns(performanceRecords);

            // Act
            var result = await service.GetMemberNominationAsync(1);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(performanceRecords[0], result.First());
            Assert.Equal(performanceRecords[1], result.ElementAt(1));
            Assert.Collection(result.ElementAt(1).Employees.ToList(),
                elem1 =>
                {
                    Assert.Equal(employees[0].EmployeeId, elem1.EmployeeId);
                    Assert.Equal(employees[0].NickName, elem1.NickName);
                },
                elem2 =>
                {
                    Assert.Equal(employees[1].EmployeeId, elem2.EmployeeId);
                    Assert.Equal(employees[1].NickName, elem2.NickName);
                }
            );
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

            commonService.GetEmployeesAsync(isEnabled: null).Returns(new List<Employee>
            {
                new Employee { EmployeeId = 1 },
                new Employee { EmployeeId = 2 }
            });

            var result = await service.GetNominationsByPeriodIdsAsync("1", null);

            Assert.NotEmpty(result);
            Assert.Collection(result, item =>
            {
                Assert.Equal(item.NomineeId, item.Nominee.EmployeeId);
                Assert.NotEmpty(item.Nominators);
            });
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
        public async Task GetMemberNominationByIdAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var performanceRecord = new PerformanceRecordModel
            {
                PerformanceRecordId = 1,
                EmployeeIds = "2",
                CreatorId = 1
            };

            nominationRepository.GetMemberNominationAsync(Arg.Any<GetMemberNominationByIdCriteria>())
                .Returns(new List<PerformanceRecordModel> { performanceRecord });

            var result = await service.GetMemberNominationByIdAsync(1, 2);
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetCurrentNominationPeriodForComittee_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var model = new NominationPeriodModel
            {
                NominationPeriodId = 1,
                Quarter = 2
            };

            baseService.GetDataEntityAsync<NominationPeriodModel>(Arg.Any<GetNominationPeriodsForComitteeCriteria>())
                .Returns(model);

            var result = await service.GetCurrentNominationPeriodForComittee(1, true);
            Assert.NotNull(result);
            Assert.True(model.Quarter.HasValue);
            Assert.Equal(model.NominationPeriodId, result.NominationPeriodId);
        }

        private EmployeeNominationService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();
            nominationRepository = Substitute.For<IEmployeeNominationRepository>();

            var service = new EmployeeNominationService(baseService, commonService, nominationRepository);

            return service;
        }
    }
}