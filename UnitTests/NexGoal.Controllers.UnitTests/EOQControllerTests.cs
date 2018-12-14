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

    public class EOQControllerTests
    {
        private ISessionService sessionService;
        private IEOQService eoqService;

        [Fact]
        public async Task GetAll_WhenCalled_ReturnAsExpectedEOQs()
        {
            // Arrange
            var eoqs = new List<EOQYModel>
            {
                new EOQYModel { EOQYId = 1, Quarter = 1, Year = 2018 }
            };
            var controller = GetEOQController();
            eoqService.GetAllAsync(2018).Returns(eoqs);

            // Act
            var result = await controller.GetAll(2018);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<EOQYModel>>(jsonResult.Value);
            Assert.Equal(eoqs, actual);
        }

        [Fact]
        public async Task GetYears_WhenCalled_ReturnAsExpectedYears()
        {
            // Arrange
            IEnumerable<int> years = new List<int> { 2018, 2019, 2020 };
            var controller = GetEOQController();
            eoqService.GetYearsAsync().Returns(years);

            // Act
            var actual = await controller.GetYears();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<int>>(jsonResult.Value);
            Assert.Equal(years, result);
        }

        #region Create

        [Fact]
        public async Task Create_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetEOQController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var actual = await controller.Create(new CreateEOQModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Create_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var eoq = new CreateEOQModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetEOQController();
            eoqService.CreateAsync(eoq).Returns(expected);

            // Act
            var actual = await controller.Create(eoq);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #endregion Create

        #region Edit

        [Fact]
        public async Task Edit_EOQIsNotExist_ReturnRedirectToPageNotFound()
        {
            // Arrange
            EOQYModel eoq = null;
            var controller = GetEOQController();
            eoqService.GetByIdAsync(1).Returns(eoq);

            // Act
            var actual = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Edit_EOQIsExist_ReturnAsExpectedViewResult()
        {
            // Arrange
            var eoq = new EOQYModel { Year = 2017 };
            var controller = GetEOQController();
            eoqService.GetByIdAsync(1).Returns(eoq);
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
            var controller = GetEOQController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var actual = await controller.Edit(new EditEOQModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task EditPost_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var eoq = new EditEOQModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetEOQController();
            eoqService.EditAsync(eoq).Returns(expected);

            // Act
            var actual = await controller.Edit(eoq);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #endregion Edit

        #region Details

        [Fact]
        public async Task Details_EOQIsNotExist_ReturnRedirectToPageNotFound()
        {
            // Arrange
            EOQYModel eoq = null;
            var controller = GetEOQController();
            eoqService.GetByIdAsync(1).Returns(eoq);

            // Act
            var actual = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Details_EOQIsExist_ReturnAsExpectedViewResult()
        {
            // Arrange
            var eoq = new EOQYModel { Year = 2017 };
            var controller = GetEOQController();
            eoqService.GetByIdAsync(1).Returns(eoq);
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
        public async Task GetById_WhenCalled_ReturnAsExpectedEOQ()
        {
            // Arrange
            var eoq = new EOQYModel();
            var controller = GetEOQController();
            eoqService.GetByIdAsync(1).Returns(eoq);

            // Act
            var actual = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<EOQYModel>(jsonResult.Value);
            Assert.Equal(eoq, result);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private EOQController GetEOQController()
        {
            sessionService = FakeSessionService();
            eoqService = Substitute.For<IEOQService>();
            var controller = new EOQController(sessionService, eoqService);

            return controller;
        }

        #endregion Helpers
    }
}