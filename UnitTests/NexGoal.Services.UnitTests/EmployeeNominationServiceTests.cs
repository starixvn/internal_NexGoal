namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
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
        private IEmployeeNominationRepository nominationRepository;
        private ICommonService commonService;

        [Fact]
        public async Task CreateAsync_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeNominationCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.CreateAsync(new CreateEmployeeNominationModel());

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task CreateEoyAsync_ValidData_ReturnExpectedCommandResult()
        {
            var fixture = new Fixture();
            var model = fixture.Create<CreateEmployeeNominationModel>();
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeNominationEoyCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.CreateEoyAsync(model);

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
        public async Task EditEoyAsync_ValidData_ReturnExpectedCommandResult()
        {
            var fixture = new Fixture();
            var model = fixture.Create<EditEmployeeNominationModel>();
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Any<EditEmployeeNominationCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.EditEoyAsync(model);

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
        public async Task GetNominationByRecordIdAsync_ValidData_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            var performanceRecord = new PerformanceRecord
            {
                PerformanceRecordId = 1,
                Employees = new List<Employee> { new Employee { EmployeeId = 1, NickName = "tim" } },
                Creator = new Employee { EmployeeId = 2, NickName = "quinn" },
                NominationPeriod = new NominationPeriodModel { NominationPeriodId = 1, Year = 2018 }
            };
            nominationRepository.GetNominationByRecordIdAsync(Arg.Any<GetNominationByRecordIdCriteria>()).Returns(performanceRecord);

            // Act
            var actual = await service.GetNominationByRecordIdAsync(1);

            // Assert
            Assert.Equal(performanceRecord, actual);
        }

        [Fact]
        public async Task GetNominationByRecordIdEoyAsync_ValidData_ReturnExpectedResult()
        {
            // Arrange
            var fixture = new Fixture();
            var performanceRecord = fixture.Create<PerformanceRecord>();
            var service = GetService();

            nominationRepository.GetNominationByRecordIdAsync(Arg.Any<GetNominationByRecordIdCriteria>()).Returns(performanceRecord);

            // Act
            var actual = await service.GetNominationByRecordIdAsync(1);

            // Assert
            Assert.Equal(performanceRecord, actual);
        }

        [Fact]
        public async Task GetMemberNominationAsync_ValidData_ReturnExpectedResult()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecord> {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    NominationPeriod = new NominationPeriodModel { NominationPeriodId = 1, Quarter = 4, Year = 2018 },
                    TypeId = (byte)PerformanceRecordType.Nomination
                }
            };

            var performanceRecordEoys = new List<PerformanceRecord> {
                new PerformanceRecord
                {
                    PerformanceRecordId = 2,
                    NominationPeriod = new NominationPeriodModel { NominationPeriodId = 1, Quarter = 4, Year = 2022 },
                    TypeId = (byte)PerformanceRecordType.Nomination
                }
            };

            var service = GetService();
            nominationRepository.GetMemberNominationAsync(Arg.Any<GetMemberNominationCriteria>()).Returns(performanceRecords);
            nominationRepository.GetMemberNominationAsync(Arg.Any<GetMemberNominationEoyCriteria>()).Returns(performanceRecordEoys);

            // Act
            var result = await service.GetMemberNominationAsync(1);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetNominationsByPeriodIdsAsync_ValidData_ReturnExpectedResult()
        {
            // Arrange
            var nominations = new List<NominationModel>
            {
                new NominationModel
                {
                    NominationId = 1,
                    Period = new NominationPeriodModel { NominationPeriodId = 1, Year = 2019 },
                    Nominee = new Employee { EmployeeId = 1, NickName = "tim" }
                }
            };
            var service = GetService();
            nominationRepository.GetNominationByPeriodIdsAsync(Arg.Any<GetNominationsByPeriodIdsCriteria>()).Returns(nominations);

            // Act
            var actual = await service.GetNominationsByPeriodIdsAsync("1", null);

            // Assert
            Assert.Equal(nominations, actual);
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

            var performanceRecord = new PerformanceRecord
            {
                PerformanceRecordId = 1,
                Employees = new List<Employee> { new Employee { EmployeeId = 1, NickName = "tim" } },
                Creator = new Employee { EmployeeId = 2, NickName = "quinn" },
                NominationPeriod = new NominationPeriodModel { NominationPeriodId = 1, Year = 2018 }
            };

            nominationRepository.GetMemberNominationAsync(Arg.Any<GetMemberNominationByIdCriteria>())
                .Returns(new List<PerformanceRecord> { performanceRecord });

            var result = await service.GetMemberNominationByIdAsync(1, 2);
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetCurrentNominationPeriodForComitteeAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var model = new NominationPeriodModel
            {
                NominationPeriodId = 1,
                Quarter = 2
            };

            baseService.GetDataEntityAsync<NominationPeriodModel>(Arg.Any<GetNominationPeriodsForComitteeCriteria>())
                .Returns(model);

            var result = await service.GetCurrentNominationPeriodForComitteeAsync(1, true);
            Assert.NotNull(result);
            Assert.True(model.Quarter.HasValue);
            Assert.Equal(model.NominationPeriodId, result.NominationPeriodId);
        }

        private EmployeeNominationService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            nominationRepository = Substitute.For<IEmployeeNominationRepository>();
            commonService = Substitute.For<ICommonService>();

            var service = new EmployeeNominationService(baseService, nominationRepository, commonService);

            return service;
        }
    }
}