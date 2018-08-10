namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.FlowTemplate;
    using NexGoal.Models.FlowTemplate;
    using NSubstitute;
    using Xunit;

    public class FlowTemplateServiceTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnAsExpectedFlowTemplates()
        {
            // Arrange
            var expected = new List<FlowTemplateModel>
            {
                new FlowTemplateModel { FlowTemplateId = 1, Name = "Semi-annual" }
            };
            var service = GetFlowTemplateService();
            baseService.GetDataAsync<FlowTemplateModel>(Arg.Any<GetFlowTemplateCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetAllAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_ReturnAsExpectedFlowTemplate()
        {
            // Arrange
            var expected = new FlowTemplateModel { FlowTemplateId = 1, Name = "Semi-annual" };
            var service = GetFlowTemplateService();
            baseService.GetDataEntityAsync<FlowTemplateModel>(Arg.Any<GetFlowTemplateByIdCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetByIdAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var flowTemplate = new CreateFlowTemplateModel
            {
                Name = "Semi-annual"
            };
            var service = GetFlowTemplateService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateFlowTemplateCriteria>()).Returns(expected);

            // Act
            var actual = await service.CreateAsync(flowTemplate);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var flowTemplate = new EditFlowTemplateModel
            {
                FlowTemplateId = 1,
                Name = "Semi-annual"
            };
            var service = GetFlowTemplateService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditFlowTemplateCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditAsync(flowTemplate);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditStatusAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetFlowTemplateService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditStatusFlowTemplateCriteria>()).Returns(expected);

            // Act
            var actual = await service.EditStatusAsync(1, true);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var service = GetFlowTemplateService();
            baseService.ExecuteNonQueryAsync(Arg.Any<DeleteFlowTemplateCriteria>()).Returns(expected);

            // Act
            var actual = await service.DeleteAsync(1);

            // Assert
            Assert.Equal(expected, actual);
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

        private FlowTemplateService GetFlowTemplateService()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            var service = new FlowTemplateService(baseService, sessionService);

            return service;
        }

        #endregion Helpers
    }
}