namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.Discipline;
    using NexGoal.Services;
    using NexGoal.ViewModels.Discipline;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class DisciplineControllerTests
    {
        private IDisciplineService disciplineService;
        private ISessionService sessionService;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            var controller = GetDisciplineController();
            sessionService.HasPermissionOnFunction(FunctionKey.DisciplineManagement, Policy.Host).Returns(true);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<bool>(viewResult.Model);
            Assert.True(viewModel);
        }

        [Fact]
        public async Task GetDisciplines_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var param = new GetDisciplineParam();
            var expected = new List<DisciplineModel>
            {
                new DisciplineModel { DisciplineId = 1 }
            };
            var controller = GetDisciplineController();
            disciplineService.GetDisciplineAsync(param).Returns(expected);

            // Act
            var result = await controller.GetDisciplines(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<DisciplineModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditStatus_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetDisciplineController();
            disciplineService.EditStatusAsync(1, true, sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.EditStatus(1, true);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region Create

        [Fact]
        public async Task Create_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetDisciplineController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var result = await controller.Create(new AddDisciplineViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Create_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetDisciplineController();
            disciplineService.CreateAsync(Arg.Any<AddDisciplineModel>()).Returns(expected);

            // Act
            var result = await controller.Create(new AddDisciplineViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #endregion Create

        #region Edit

        [Fact]
        public async Task Edit_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetDisciplineController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var actual = await controller.Edit(new EditDisciplineViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Edit_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetDisciplineController();
            disciplineService.EditAsync(Arg.Any<EditDisciplineModel>()).Returns(expected);

            // Act
            var result = await controller.Edit(new EditDisciplineViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #endregion Edit

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private DisciplineController GetDisciplineController()
        {
            disciplineService = Substitute.For<IDisciplineService>();
            sessionService = FakeSessionService();

            return new DisciplineController(disciplineService, sessionService);
        }

        #endregion Helpers
    }
}