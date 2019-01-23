namespace NexGoal.Controllers.UnitTests
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PAControllerTests
    {
        private ISessionService sessionService;
        private IPARecordService paRecordService;

        #region GetById

        [Fact]
        public async Task GetById_DoNotPermission_ReturnNotAllowed()
        {
            // Arrange
            var controller = GetPAController();
            paRecordService.GetPermissionOnPerformanceAppraisalAsync(1, sessionService.EmployeeId).Returns(Policy.None);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, jsonResult.StatusCode);
        }

        [Fact]
        public async Task GetById_RecordIsNotExist_ReturnNotFound()
        {
            // Arrange
            PARecordModel record = null;
            var controller = GetPAController();
            paRecordService.GetPermissionOnPerformanceAppraisalAsync(1, sessionService.EmployeeId).Returns(Policy.Read);
            paRecordService.GetPARecordByIdAsync(1, sessionService.EmployeeId).Returns(record);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, jsonResult.StatusCode);
        }

        [Fact]
        public async Task GetById_RecordIsExist_ReturnAsExpectedRecord()
        {
            // Arrange
            var expected = new PARecordModel
            {
                PARecordId = 1,
                Employee = new Employee { EmployeeId = 1, NickName = "tim" },
                Grade = new PerformanceGradeModel { GradeId = 1, Name = "E" }
            };
            var controller = GetPAController();
            paRecordService.GetPermissionOnPerformanceAppraisalAsync(1, sessionService.EmployeeId).Returns(Policy.Read);
            paRecordService.GetPARecordByIdAsync(1, sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<PARecordModel>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #endregion GetById

        #region Helpers

        private PAController GetPAController()
        {
            sessionService = Helpers.FakeSessionService();
            paRecordService = Substitute.For<IPARecordService>();
            var controller = new PAController(sessionService, paRecordService);

            return controller;
        }

        #endregion Helpers
    }
}