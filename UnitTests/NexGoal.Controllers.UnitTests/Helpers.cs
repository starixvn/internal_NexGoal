namespace NexGoal.Controllers.UnitTests
{
    using NexGoal.Web.Core.Services;
    using NSubstitute;

    public static class Helpers
    {
        public static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }
    }
}