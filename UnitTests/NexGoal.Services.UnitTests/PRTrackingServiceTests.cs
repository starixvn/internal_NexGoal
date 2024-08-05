namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Criteria.PRTracking;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;
    using NexGoal.Repositories;
    using NSubstitute;
    using Xunit;

    public class PRTrackingServiceTests
    {
        private IPRTrackingRepository repository;

        [Fact]
        public async Task GetAsync_ValidData_ReturnExpecetedResult()
        {
            var criteria = new GetPRCriteria();
            var expected = (new PeriodModel { PeriodId = 1, Name = "1st" }, new List<PRTrackingModel>());

            var service = GetPRTrackingService();

            repository.GetAsync(Arg.Do<GetPRCriteria>(x => criteria = x)).Returns(expected);

            var actual = await service.GetAsync(1, "367", 1);

            Assert.Equal(1, criteria.ActorId);
            Assert.Equal("367", criteria.EmployeeIds);
            Assert.Equal((byte)1, criteria.NoPR);
            Assert.Equal(expected.Item1.PeriodId, actual.Item1.PeriodId);
            Assert.Equal(expected.Item1.Name, actual.Item1.Name);
        }

        private PRTrackingService GetPRTrackingService()
        {
            repository = Substitute.For<IPRTrackingRepository>();
            var service = new PRTrackingService(repository);

            return service;
        }
    }
}