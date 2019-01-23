namespace NexGoal.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using NSubstitute;
    using Xunit;

    public class PAFocusRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public PAFocusRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetPAFocusAsync_WhenCalled_ReturnAsExpectedPAFocuses()
        {
            // Arrange
            var pafcuses = new List<PAFocusModel>
            {
                new PAFocusModel
                {
                    PAFocusId = 1,
                    Employee = new Employee { EmployeeId = 1, NickName = "tim" }
                }
            };
            var criteria = Helpers.FakeCriteria();
            var repository = new PAFocusRepository();
            objectDb.QueryAsync(Arg.Any<Func<PAFocusModel, Employee, PAFocusModel>>(),
                "EmployeeId",
                criteria).Returns(pafcuses);

            // Act
            var actual = await repository.GetPAFocusAsync(criteria);

            // Assert
            Assert.Equal(pafcuses, actual);
        }
    }
}