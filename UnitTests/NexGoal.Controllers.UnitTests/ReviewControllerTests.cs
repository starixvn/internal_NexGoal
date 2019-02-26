namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class ReviewControllerTests
    {
        private ISessionService sessionService;
        private IPARecordService paRecordService;

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
            var groups = new List<Group>
            {
                new Group { GroupId = 1, Name = "Nexus" }
            };
            var controller = GetReviewController();
            paRecordService.GetGroupByActorAsync(sessionService.EmployeeId, 1).Returns(groups);

            // Act
            var actual = await controller.GetGroups(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<Group>>(jsonResult.Value);
            Assert.Equal(groups, result);
        }

        [Fact]
        public async Task GetPARecords_WhenCalled_ReturnAsExpectedPARecords()
        {
            // Arrange
            var param = new GetPARecordParam
            {
                PeriodId = 1,
                GroupIds = "1,2",
                IsRevised = true
            };
            var records = new List<PARecordGroupModel>
            {
                new PARecordGroupModel { GroupName = "Nexus", PerformanceAppraisals = new List<PARecordModel>() }
            };
            var controller = GetReviewController();
            paRecordService.GetPARecordByReviewerAsync(Arg.Any<GetPARecordParam>()).Returns(records);

            // Act
            var actual = await controller.GetPARecords(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.IsAssignableFrom<IEnumerable<PARecordGroupModel>>(jsonResult.Value);
        }

        #region Edit

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
            paRecordService.QuickEditPARecordByReviewerAsync(sessionService.EmployeeId, paRecordId, true).Returns(expected);

            // Act
            var actual = await controller.QuickEdit(paRecordId, true);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #endregion Edit

        #region Helpers

        private ReviewController GetReviewController()
        {
            sessionService = Helpers.FakeSessionService();
            paRecordService = Substitute.For<IPARecordService>();
            var controller = new ReviewController(sessionService, paRecordService);

            return controller;
        }

        #endregion Helpers
    }
}