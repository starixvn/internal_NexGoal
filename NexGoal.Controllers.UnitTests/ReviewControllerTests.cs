namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class ReviewControllerTests
    {
        private ISessionService sessionService;
        private IPARecordService paRecordService;

        [Fact]
        public void Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            // Arrange
            var controller = GetReviewController();

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetPeriods_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var periods = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "1H 2018" }
            };
            var controller = GetReviewController();
            paRecordService.GetPeriodByActorAsync(sessionService.EmployeeId).Returns(periods);

            // Act
            var actual = await controller.GetPeriods();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<PeriodBaseModel>>(jsonResult.Value);
            Assert.Equal(periods, result);
        }

        [Fact]
        public async Task GetGroups_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var groups = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Name = "Nexus" }
            };
            var controller = GetReviewController();
            paRecordService.GetGroupByActorAsync(sessionService.EmployeeId, 1).Returns(groups);

            // Act
            var actual = await controller.GetGroups(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<GroupBaseModel>>(jsonResult.Value);
            Assert.Equal(groups, result);
        }

        [Fact]
        public async Task GetPARecords_WhenCalled_ReturnAsExpectedPARecords()
        {
            // Arrange
            var param = new GetPARecordParam
            {
                PeriodId = 1,
                GroupIds = "1,2"
            };
            var records = new List<PARecordModel>
            {
                new PARecordModel { PARecordId = 1, EmployeeId = 1 }
            };
            var controller = GetReviewController();
            paRecordService.GetPARecordByActorAsync(Arg.Any<GetPARecordParam>()).Returns(records);

            // Act
            var actual = await controller.GetPARecords(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<PARecordGroupModel>>(jsonResult.Value);
        }

        #region Edit

        [Fact]
        public async Task Edit_PARecordIsNotExist_ReturnRedirectToPageNotFound()
        {
            // Arrange
            PARecordModel paRecord = null;
            var controller = GetReviewController();
            paRecordService.GetPARecordByIdAsync(1, sessionService.EmployeeId).Returns(paRecord);

            // Act
            var actual = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Edit_PARecordIsExist_ReturnAsExpectedViewResult()
        {
            // Arrange
            var paRecord = new PARecordModel { PARecordId = 1, EmployeeId = 1, StatusId = 3 };
            var controller = GetReviewController();
            paRecordService.GetPARecordByIdAsync(1, sessionService.EmployeeId).Returns(paRecord);
            sessionService.HasPermissionOnFunction("Review", Policy.Host).Returns(true);

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
            var controller = GetReviewController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var actual = await controller.Edit(new EditPARecordByReviewerModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task EditPost_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var paRecord = new EditPARecordByReviewerModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetReviewController();
            paRecordService.EditPARecordByReviewerAsync(sessionService.EmployeeId, paRecord).Returns(expected);

            // Act
            var actual = await controller.Edit(paRecord);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        [Fact]
        public async Task QuickEditPost_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var paRecordId = 10;
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetReviewController();
            paRecordService.QuickEditPARecordByReviewerAsync(sessionService.EmployeeId, paRecordId).Returns(expected);

            // Act
            var actual = await controller.QuickEdit(paRecordId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #endregion Edit

        #region Details

        [Fact]
        public async Task Details_PARecordIsNotExist_ReturnRedirectToPageNotFound()
        {
            // Arrange
            PARecordModel paRecord = null;
            var controller = GetReviewController();
            paRecordService.GetPARecordByIdAsync(1, sessionService.EmployeeId).Returns(paRecord);

            // Act
            var actual = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Details_PARecordIsExist_ReturnAsExpectedViewResult()
        {
            // Arrange
            var paRecord = new PARecordModel { PARecordId = 1, EmployeeId = 1, StatusId = 3 };
            var controller = GetReviewController();
            paRecordService.GetPARecordByIdAsync(1, sessionService.EmployeeId).Returns(paRecord);
            sessionService.HasPermissionOnFunction("Review", Policy.Host).Returns(true);

            // Act
            var actual = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(1, viewResult.Model);
        }

        #endregion Details

        [Fact]
        public async Task GetById_WhenCalled_ReturnAsExpectedJsonResult()
        {
            // Arrange
            var paRecord = new PARecordModel { PARecordId = 1, EmployeeId = 1, StatusId = 3 };
            var controller = GetReviewController();
            paRecordService.GetPARecordByIdAsync(1, sessionService.EmployeeId).Returns(paRecord);

            // Act
            var actual = await controller.GetPARecordById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<PARecordModel>(jsonResult.Value);
            Assert.Equal(paRecord, result);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private ReviewController GetReviewController()
        {
            sessionService = FakeSessionService();
            paRecordService = Substitute.For<IPARecordService>();
            var controller = new ReviewController(sessionService, paRecordService);

            return controller;
        }

        #endregion Helpers
    }
}