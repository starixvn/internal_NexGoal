namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeNomination;
    using NexGoal.Models.EmployeeNomination;
    using NSubstitute;
    using Xunit;

    public class EmployeeNominationServiceTests
    {
        private IBaseService baseService;
        private ICommonService commonService;
        private IAvatarService avatarService;

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

            var result = await service.GetNominationPeriodsForNominator(1);

            Assert.Equal(expectedPeriods.Count, result.Count());
        }

        [Fact]
        public async Task Create_ValidData_ReturnExpectedCommandResult()
        {
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateEmployeeNominationCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.Create(new CreateEmployeeNominationModel());

            Assert.True(result.IsSuccessful);
        }

        private EmployeeNominationService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();
            avatarService = Substitute.For<IAvatarService>();

            var service = new EmployeeNominationService(baseService, avatarService, commonService);

            return service;
        }
    }
}