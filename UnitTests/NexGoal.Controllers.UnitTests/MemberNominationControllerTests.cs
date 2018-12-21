namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class MemberNominationControllerTests
    {
        private IEmployeeNominationService nominationService;
        private ISessionService sessionService;

        [Fact]
        public async Task GetNominationPeriodsForNominator_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var periods = new List<NominationPeriodModel>
            {
                new NominationPeriodModel
                {
                    NominationPeriodId = 1,
                    Quarter = 1,
                    Year = 2018
                }
            };
            var controller = GetController();
            nominationService.GetNominationPeriodsForNominatorAsync(sessionService.EmployeeId).Returns(periods);

            // Act
            var result = await controller.GetNominationPeriodsForNominator();

            // Assert
            Assert.IsType<JsonResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<NominationPeriodModel>>(jsonResult.Value);
            Assert.Equal(actual, periods);
        }

        #region Create

        [Fact]
        public async Task Create_InvalidData_ReturnErrors()
        {
            // Arrange
            var controller = GetController();
            controller.ModelState.AddModelError(nameof(CreateEmployeeNominationViewModel.NomineeId), "Nominee is required");

            // Act
            var result = await controller.Create(new CreateEmployeeNominationViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Create_ValidData_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetController();
            nominationService.CreateAsync(Arg.Any<CreateEmployeeNominationModel>()).Returns(expected);

            // Act
            var actual = await controller.Create(new CreateEmployeeNominationViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        #endregion Create

        #region Edit

        [Fact]
        public async Task Edit_ModelStateIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetController();
            controller.ModelState.AddModelError("name", "required");

            // Act
            var result = await controller.Edit(new EditEmployeeNominationViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Edit_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetController();
            nominationService.EditAsync(Arg.Any<EditEmployeeNominationModel>()).Returns(expected);

            // Act
            var result = await controller.Edit(new EditEmployeeNominationViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #endregion Edit

        [Fact]
        public async Task Delete_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetController();
            nominationService.DeleteAsync(1, sessionService.EmployeeId).Returns(expected);

            // Act
            var actual = await controller.Delete(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        [Fact]
        public async Task GetMemberNominations_WhenCalled_ReturnAsExpectedPerformanceRecords()
        {
            // Arrange
            var expected = new List<PerformanceRecordModel>
            {
                new PerformanceRecordModel
                {
                    PerformanceRecordId = 1,
                    Employees = new List<EmployeeBaseModel>
                    {
                        new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
                    },
                    Comment = "test"
                }
            };
            var controller = GetController();
            nominationService.GetMemberNominationsAsync(1, sessionService.EmployeeId).Returns(expected);

            // Act
            var result = await controller.GetMemberNominations(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<PerformanceRecordModel>>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetByRecordId_WhenCalled_ReturnAsExpectedPerformanceRecord()
        {
            // Arrange
            var expected = new PerformanceRecordModel
            {
                PerformanceRecordId = 1,
                Employees = new List<EmployeeBaseModel>
                {
                    new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
                },
                Comment = "test"
            };
            var controller = GetController();
            nominationService.GetNominationByRecordIdAsync(1).Returns(expected);

            // Act
            var result = await controller.GetByRecordId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<PerformanceRecordModel>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private MemberNominationController GetController()
        {
            nominationService = Substitute.For<IEmployeeNominationService>();
            sessionService = Substitute.For<ISessionService>();
            sessionService.EmployeeId = 1;

            return new MemberNominationController(nominationService, sessionService);
        }

        #endregion Helpers
    }
}