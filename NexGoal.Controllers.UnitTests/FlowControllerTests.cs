namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Models.FlowTemplate;
    using NexGoal.Services;
    using NexGoal.ViewModels.FlowTemplate;
    using NSubstitute;
    using Xunit;

    public class FlowControllerTests
    {
        private ISessionService sessionService;
        private ICommonService commonService;
        private IFlowTemplateService flowService;

        [Fact]
        public void Index_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var controller = GetFlowController();
            sessionService.HasPermissionOnFunction(FunctionKey.EvaluationFlow, Policy.Host).Returns(true);

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<FlowTemplateViewModel>(viewResult.Model);
            Assert.Null(viewModel.TargetTypeId);
            Assert.True(viewModel.IsActive);
            Assert.True(viewModel.IsHost);
        }

        [Fact]
        public async Task GetAll_WhenCalled_ReturnAsExpectedFlowTemplates()
        {
            // Arrange
            var flowTemplates = new List<FlowTemplateModel>
            {
                new FlowTemplateModel { FlowTemplateId = 1, Name = "Semi-annual" }
            };
            var controller = GetFlowController();
            flowService.GetAllAsync().Returns(flowTemplates);

            // Act
            var actual = await controller.GetAll();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<FlowTemplateModel>>(jsonResult.Value);
            Assert.Equal(flowTemplates, result);
        }

        #region Create

        [Fact]
        public async Task Create_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetFlowController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var actual = await controller.Create(new CreateFlowTemplateModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Create_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var flowTemplate = new CreateFlowTemplateModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetFlowController();
            flowService.CreateAsync(flowTemplate).Returns(expected);

            // Act
            var actual = await controller.Create(flowTemplate);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #endregion Create

        #region Edit

        [Fact]
        public async Task Edit_FlowTemplateIsNotExist_ReturnRedirectToPageNotFound()
        {
            // Arrange
            FlowTemplateModel flowTemplate = null;
            var controller = GetFlowController();
            flowService.GetByIdAsync(1).Returns(flowTemplate);

            // Act
            var actual = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Edit_FlowTemplateIsExist_ReturnAsExpectedViewResult()
        {
            // Arrange
            var flowTemplate = new FlowTemplateModel { FlowTemplateId = 1, Name = "Semi-annual" };
            var controller = GetFlowController();
            flowService.GetByIdAsync(1).Returns(flowTemplate);
            sessionService.HasPermissionOnFunction(FunctionKey.EvaluationFlow, Policy.Host).Returns(true);

            // Act
            var actual = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(1, viewResult.Model);
            Assert.True((bool)viewResult.ViewData["IsHost"]);
        }

        [Fact]
        public async Task EditPost_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetFlowController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var actual = await controller.Edit(new EditFlowTemplateModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task EditPost_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var flowTemplate = new EditFlowTemplateModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetFlowController();
            flowService.EditAsync(flowTemplate).Returns(expected);

            // Act
            var actual = await controller.Edit(flowTemplate);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #endregion Edit

        [Fact]
        public async Task EditStatus_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetFlowController();
            flowService.EditStatusAsync(1, true).Returns(expected);

            // Act
            var actual = await controller.EditStatus(1, true);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        [Fact]
        public async Task Delete_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetFlowController();
            flowService.DeleteAsync(1).Returns(expected);

            // Act
            var actual = await controller.Delete(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #region Details

        [Fact]
        public async Task Details_FlowTemplateIsNotExist_ReturnRedirectToPageNotFound()
        {
            // Arrange
            FlowTemplateModel flowTemplate = null;
            var controller = GetFlowController();
            flowService.GetByIdAsync(1).Returns(flowTemplate);

            // Act
            var actual = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Details_FlowTemplateIsExist_ReturnAsExpectedViewResult()
        {
            // Arrange
            var flowTemplate = new FlowTemplateModel { FlowTemplateId = 1, Name = "Semi-annual" };
            var controller = GetFlowController();
            flowService.GetByIdAsync(1).Returns(flowTemplate);

            // Act
            var actual = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(1, viewResult.Model);
        }

        #endregion Details

        [Fact]
        public async Task GetById_WhenCalled_ReturnAsExpectedFlowTemplate()
        {
            // Arrange
            var flowTemplate = new FlowTemplateModel();
            var controller = GetFlowController();
            flowService.GetByIdAsync(1).Returns(flowTemplate);

            // Act
            var actual = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<FlowTemplateModel>(jsonResult.Value);
            Assert.Equal(flowTemplate, result);
        }

        [Fact]
        public async Task GetEmployeeById_WhenCalled_ReturnAsExpectedEmployee()
        {
            // Arrange
            var employee = new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" };
            var controller = GetFlowController();
            commonService.GetEmployeeByIdAsync(1).Returns(employee);

            // Act
            var actual = await controller.GetEmployeeById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<EmployeeBaseModel>(jsonResult.Value);
            Assert.Equal(employee, result);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private FlowController GetFlowController()
        {
            sessionService = FakeSessionService();
            commonService = Substitute.For<ICommonService>();
            flowService = Substitute.For<IFlowTemplateService>();
            var controller = new FlowController(sessionService, commonService, flowService);

            return controller;
        }

        #endregion Helpers
    }
}