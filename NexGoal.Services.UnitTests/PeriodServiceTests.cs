namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models.Period;
    using NSubstitute;
    using Xunit;

    public class PeriodServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICommonService commonService;

        [Fact]
        public async Task GetPeriods_WhenCalled_ReturnExpectedResult()
        {
            var expectedPeriods = new List<PeriodModel>
            {
                new PeriodModel
                {
                    PeriodId = 1,
                    TargetTypeId = 1
                }
            };

            var service = GetPeriodService();
            baseService.GetDataAsync<PeriodModel>(Arg.Any<GetPeriodsCriteria>()).Returns(expectedPeriods);

            var result = await service.GetPeriods(null);

            Assert.Equal(expectedPeriods.Count, result.Count());
        }

        [Fact]
        public async Task GetPeriod_WhenCalled_ReturnExpectedResult()
        {
            var expectedPeriod = new PeriodModel
            {
                PeriodId = 1,
                TargetTypeId = 1
            };

            var service = GetPeriodService();
            baseService.GetDataEntityAsync<PeriodModel>(Arg.Any<GetPeriodCriteria>()).Returns(expectedPeriod);

            var result = await service.GetPeriod(1);

            Assert.Equal(expectedPeriod.PeriodId, result.PeriodId);
            Assert.Equal(expectedPeriod.TargetTypeId, result.TargetTypeId);
        }

        [Fact]
        public async Task Create_ValidModel_ReturnExpectedCommandResult()
        {
            var model = new CreatePeriodModel
            {
                Name = "",
                TargetTypeId = 1
            };

            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreatePeriodCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.Create(model);

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task EditStatus_ValidModel_ReturnExpectedCommandResult()
        {
            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdatePeriodStatusCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.EditStatus(1, true);

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task Edit_ValidModel_ReturnExpectedCommandResult()
        {
            var model = new EditPeriodModel
            {
                PeriodId = 1,
                Name = ""
            };

            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditPeriodCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.Edit(model);

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task EditProcessSetting_ValidModel_ReturnExpectedCommandResult()
        {
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

            var service = GetPeriodService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditProcessSettingCriteria>()).Returns(new CommandResult { Code = 0 });

            var result = await service.EditProcessSetting(1, settings);

            Assert.True(result.IsSuccessful);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private PeriodService GetPeriodService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            commonService = Substitute.For<ICommonService>();
            var service = new PeriodService(baseService, sessionService, commonService);

            return service;
        }

        #endregion Helpers
    }
}