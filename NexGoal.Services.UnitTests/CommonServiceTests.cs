namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Models;
    using NSubstitute;
    using Xunit;

    public class CommonServiceTests
    {
        private IBaseService baseService;

        #region GetReferenceResourcesByNames

        [Fact]
        public async Task GetReferenceResourcesByNames_ParamIsNull_ReturnEmptyListReferenceResources()
        {
            // Arrange
            var service = GetCommonService();

            // Act
            var actual = await service.GetReferenceResourcesByNamesAsync(null);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetReferenceResourcesByNames_ParamIsNotNull_ReturnAsExpectedReferenceResources()
        {
            // Arrange
            var expected = GetRefResourcesForTest();
            var refNames = new[]
            {
                "SystemRole"
            };

            var service = GetCommonService();
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetReferenceResourcesByNamesAsync(refNames);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetReferenceResourcesByNames

        [Fact]
        public void GetSelectList_WhenCalled_ReturnAsExpectedSelectList()
        {
            // Arrange
            var refResources = GetRefResourcesForTest();
            var service = GetCommonService();

            // Act
            var actual = service.GetSelectList(
                refResources,
                nameof(ReferenceResourceModel.ResourceId),
                nameof(ReferenceResourceModel.Value));

            // Assert
            Assert.Equal(refResources.Count, actual.Count());
        }

        #region Helpers

        private List<ReferenceResourceModel> GetRefResourcesForTest()
        {
            return new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    ResourceId = 1,
                    RefName = "SystemRole",
                    Value = "SA",
                    IsActive = true
                }
            };
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private CommonService GetCommonService()
        {
            baseService = FakeBaseService();
            var avatarService = Substitute.For<IAvatarService>();
            var service = new CommonService(baseService, avatarService);

            return service;
        }

        #endregion Helpers
    }
}