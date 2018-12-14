namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.EOQY;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class EOYControllerTests
    {
        private ISessionService sessionService;
        private IEOYService eoyService;

        [Fact]
        public async Task GetAll_WhenCalled_ReturnAsExpectedEOQs()
        {
            // Arrange
            var eoys = new List<EOQYModel>
            {
                new EOQYModel { EOQYId = 1, Quarter = 1, Year = 2018 }
            };
            var controller = GetEOYController();
            eoyService.GetAllAsync(2018).Returns(eoys);

            // Act
            var result = await controller.GetAll(2018);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<EOQYModel>>(jsonResult.Value);
            Assert.Equal(eoys, actual);
        }

        #region Create

        [Fact]
        public async Task Create_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetEOYController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var actual = await controller.Create(new CreateEOYModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Create_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var eoy = new CreateEOYModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetEOYController();
            eoyService.CreateAsync(eoy).Returns(expected);

            // Act
            var actual = await controller.Create(eoy);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #endregion Create

        #region Edit

        [Fact]
        public async Task Edit_EOYIsNotExist_ReturnRedirectToPageNotFound()
        {
            // Arrange
            EOQYModel eoy = null;
            var controller = GetEOYController();
            eoyService.GetByIdAsync(1).Returns(eoy);

            // Act
            var actual = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Edit_EOYIsExist_ReturnAsExpectedViewResult()
        {
            // Arrange
            var eoy = new EOQYModel { Year = 2017 };
            var controller = GetEOYController();
            eoyService.GetByIdAsync(1).Returns(eoy);
            sessionService.HasPermissionOnFunction("NominationManagement", Core.Enumerations.Policy.Host).Returns(true);

            // Act
            var actual = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(1, viewResult.Model);
            Assert.False((bool)viewResult.ViewData["IsHost"]);
        }

        [Fact]
        public async Task EditPost_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetEOYController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var actual = await controller.Edit(new EditEOYModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task EditPost_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var eoy = new EditEOYModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetEOYController();
            eoyService.EditAsync(eoy).Returns(expected);

            // Act
            var actual = await controller.Edit(eoy);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #endregion Edit

        #region Details

        [Fact]
        public async Task Details_EOYIsNotExist_ReturnRedirectToPageNotFound()
        {
            // Arrange
            EOQYModel eoy = null;
            var controller = GetEOYController();
            eoyService.GetByIdAsync(1).Returns(eoy);

            // Act
            var actual = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Details_EOYIsExist_ReturnAsExpectedViewResult()
        {
            // Arrange
            var eoy = new EOQYModel { Year = 2017 };
            var controller = GetEOYController();
            eoyService.GetByIdAsync(1).Returns(eoy);
            sessionService.HasPermissionOnFunction("NominationManagement", Core.Enumerations.Policy.Host).Returns(true);

            // Act
            var actual = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(1, viewResult.Model);
        }

        #endregion Details

        [Fact]
        public async Task GetById_WhenCalled_ReturnAsExpectedEOY()
        {
            // Arrange
            var eoy = new EOQYModel();
            var controller = GetEOYController();
            eoyService.GetByIdAsync(1).Returns(eoy);

            // Act
            var actual = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<EOQYModel>(jsonResult.Value);
            Assert.Equal(eoy, result);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private EOYController GetEOYController()
        {
            sessionService = FakeSessionService();
            eoyService = Substitute.For<IEOYService>();
            var controller = new EOYController(sessionService, eoyService);

            return controller;
        }

        #endregion Helpers
    }
}