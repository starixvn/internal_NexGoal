namespace NexGoal.Controllers.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Models;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using NexGoal.Services;
    using NexGoal.ViewModels.PAFocus;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PAFocusControllerTests
    {
        private ISessionService sessionService;
        private ICommonService commonService;
        private IPAFocusService focusService;
        private IExportPAFocusService exportExcelService;

        [Fact]
        public void Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            // Arrange
            var controller = GetPAFocusController();

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetFocusEvaluationLevel_WhenCalled_ReturnAsExpectedEvaluationLevels()
        {
            // Arrange
            var referenceResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel { ResourceId = 1, Value = "Disagree" }
            };
            var controller = GetPAFocusController();
            commonService.GetReferenceResourcesByNamesAsync(Arg.Any<string[]>()).Returns(referenceResources);

            // Act
            var actual = await controller.GetFocusEvaluationLevel();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<FocusEvaluationLevelModel>>(jsonResult.Value);
            Assert.Equal(referenceResources.Count, result.Count());
        }

        [Fact]
        public async Task GetPeriod_WhenCalled_ReturnAsExpectedPeriods()
        {
            // Arrange
            var periods = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "1h-2018" }
            };
            var controller = GetPAFocusController();
            focusService.GetPeriodAsync().Returns(periods);

            // Act
            var actual = await controller.GetPeriod();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<PeriodBaseModel>>(jsonResult.Value);
            Assert.Equal(periods, result);
        }

        [Fact]
        public async Task GetGroup_WhenCalled_ReturnAsExpectedGroups()
        {
            // Arrange
            var groups = new List<GroupBaseModel>
            {
                new GroupBaseModel { GroupId = 1, Code = "NEX", Name = "Nexus" }
            };
            var controller = GetPAFocusController();
            focusService.GetGroupAsync(sessionService.EmployeeId).Returns(groups);

            // Act
            var actual = await controller.GetGroup();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<GroupBaseModel>>(jsonResult.Value);
            Assert.Equal(groups, result);
        }

        [Fact]
        public async Task GetMember_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var employees = new List<EmployeeBaseModel>
            {
                new EmployeeBaseModel { EmployeeId = 1, NickName = "Tim" }
            };
            var controller = GetPAFocusController();
            focusService.GetMemberAsync(sessionService.EmployeeId).Returns(employees);

            // Act
            var actual = await controller.GetMember();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<EmployeeBaseModel>>(jsonResult.Value);
            Assert.Equal(employees, result);
        }

        [Fact]
        public async Task GetPAFocus_WhenCalled_ReturnAsExpectedPAFocuses()
        {
            // Arrange
            var pafocuses = new List<PAFocusModel>
            {
                new PAFocusModel
                {
                    PAFocusId = 1,
                    Employee = new EmployeeBaseModel { EmployeeId = 1, NickName = "tim", GroupCode = "NEX", GroupName = "Nexus" }
                }
            };
            var controller = GetPAFocusController();
            focusService.GetPAFocusAsync(Arg.Any<GetPAFocusParam>()).Returns(pafocuses);

            // Act
            var result = await controller.GetPAFocus(new GetPAFocusParam());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = (IEnumerable<PAFocusViewModel>)jsonResult.Value.GetType().GetProperty("paFocuses").GetValue(jsonResult.Value);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isHost").GetValue(jsonResult.Value));
            Assert.Single(actual);
        }

        [Fact]
        public async Task Edit_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var paFocuses = new List<EditPAFocusModel>();
            var expected = new CommandResult
            {
                Code = 0
            };
            var controller = GetPAFocusController();
            focusService.EditPAFocusAsync(sessionService.EmployeeId, paFocuses).Returns(expected);

            // Act
            var actual = await controller.Edit(paFocuses);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, commandResult);
        }

        [Fact]
        public async Task ExportToExcel_WhenCalled_ReturnAsExpectedFile()
        {
            // Arrange
            var param = new GetPAFocusParam
            {
                PeriodId = 1
            };
            var periods = new List<PeriodBaseModel>
            {
                new PeriodBaseModel { PeriodId = 1, Name = "1h-2018" }
            };
            var paFocuses = new List<PAFocusModel>
            {
                new PAFocusModel { PAFocusId = 1, BonusGiven = "1" }
            };
            byte[] bytes = Convert.FromBase64String("text");
            var controller = GetPAFocusController();
            focusService.GetPeriodAsync().Returns(periods);
            focusService.GetPAFocusAsync(param).Returns(paFocuses);
            exportExcelService.Export(periods.First(), paFocuses).Returns(bytes);

            // Act
            var actual = await controller.ExportToExcel(param);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal(MimeType.XLSX, fileResult.ContentType);
            Assert.Equal($"PA Focus of {periods.First().Name}.xlsx", fileResult.FileDownloadName);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private PAFocusController GetPAFocusController()
        {
            sessionService = FakeSessionService();
            commonService = Substitute.For<ICommonService>();
            focusService = Substitute.For<IPAFocusService>();
            exportExcelService = Substitute.For<IExportPAFocusService>();
            var controller = new PAFocusController(sessionService, commonService, focusService, exportExcelService);

            return controller;
        }

        #endregion Helpers
    }
}