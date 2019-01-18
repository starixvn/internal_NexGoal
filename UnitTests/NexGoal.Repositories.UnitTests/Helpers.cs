namespace NexGoal.Repositories.UnitTests
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NSubstitute;

    public static class Helpers
    {
        public static ICriteria FakeCriteria()
        {
            var criteria = Substitute.For<ICriteria>();
            criteria.GetSettingKey().Returns("NexGoal_Test");

            return criteria;
        }

        public static IMultiResultReader FakeMultipleResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }
    }
}