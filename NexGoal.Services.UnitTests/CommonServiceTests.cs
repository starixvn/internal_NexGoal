namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Criteria.Common;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NSubstitute;
    using Xunit;

    public class CommonServiceTests
    {
        private IBaseService baseService;
        private IAvatarService avatarService;

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

        [Fact]
        public async Task GetEmployeesAsync_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, Code = "NEX", NickName = "Nexus" }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<EmployeeBaseModel>(Arg.Any<SearchEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetEmployeesAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WhenCalled_ReturnAsExpectedEmployee()
        {
            // Arrange
            var employee = new EmployeeBaseModel { EmployeeId = 1, Code = "NEX", NickName = "Nexus" };
            var service = GetCommonService();
            baseService.GetDataEntityAsync<EmployeeBaseModel>(Arg.Any<SearchEmployeeCriteria>()).Returns(employee);
            avatarService.GetBase64Image(Arg.Any<string>()).Returns("base64string");

            // Act
            var actual = await service.GetEmployeeByIdAsync(1);

            // Assert
            Assert.Equal(employee.EmployeeId, actual.EmployeeId);
            Assert.Equal(employee.Code, actual.Code);
            Assert.Equal(employee.NickName, actual.NickName);
            Assert.Equal("base64string", actual.Base64Image);
        }

        [Fact]
        public async Task GetGroupsAsync_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var expected = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Code = "NEX", Name = "Nexus" }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<GroupBaseModel>(Arg.Any<GetGroupsCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetGroupsAsync();

            // Assert
            Assert.Equal(expected, actual);
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

        private CommonService GetCommonService()
        {
            baseService = Substitute.For<IBaseService>();
            avatarService = Substitute.For<IAvatarService>();
            var service = new CommonService(baseService, avatarService);

            return service;
        }

        #endregion Helpers
    }
}