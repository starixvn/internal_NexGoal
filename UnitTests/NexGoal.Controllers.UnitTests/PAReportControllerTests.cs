namespace NexGoal.Controllers.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Report;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;
    using NSubstitute;
    using Xunit;

    public class PAReportControllerTests
    {
        private ISessionService sessionService;
        private IPAReportService reportService;
        private IExportPAReportService exportService;
        private IPARecordService paRecordService;

        [Fact]
        public void Index_WhenCalled_ReturnExpectView()
        {
            // Arrange
            var controller = GetReportController();

            // Act
            var actual = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public async Task GetPARecords_WhenCalled_ReturnAsExpectedPARecords()
        {
            // Arrange
            var param = new GetPARecordMultiPeriodParam
            {
                PeriodIds = "1,2,3,4",
                GroupIds = "1,2"
            };
            var records = FakeDataListPA();
            var controller = GetReportController();

            reportService.GetPAReportAsync(Arg.Any<GetPARecordMultiPeriodParam>()).Returns(records);

            // Act
            var actual = await controller.GetPARecords(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.IsAssignableFrom<IEnumerable<PARecordGroupByPeriod>>(jsonResult.Value);
        }

        [Fact]
        public async Task GetGroupAndMember_WhenCalled_ReturnList()
        {
            var groups = new List<Group> { new Group { GroupId = 1, Code = "NEX", Name = "Nexus" } };
            var employees = new List<Employee> { new Employee { NickName = "Oliver", EmployeeId = 363 } };
            var values = new Tuple<IEnumerable<Group>, IEnumerable<Employee>>(groups, employees);

            var expected = new
            {
                Groups = groups,
                Members = employees
            };

            var controller = GetReportController();
            reportService.GetGroupAndMemberAsync(sessionService.EmployeeId).Returns(values);

            // Act
            var actual = await controller.GetGroupAndMember();

            // Assert
            var result = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected.ToString(), result.Value.ToString());
        }

        [Fact]
        public async Task ExportPAReport_WhenCalled_ReturnAsExpectedFile()
        {
            // Arrange
            var param = new GetPARecordMultiPeriodParam
            {
                PeriodIds = "1,2,3,4",
                GroupIds = "1,2",
                EmployeeIds = "1,2,3,4"
            };

            var controller = GetReportController();

            byte[] bytes = Convert.FromBase64String("text");
            exportService.ExportExcelAsync(Arg.Any<GetPARecordMultiPeriodParam>()).Returns(bytes);

            // Act
            var actual = await controller.ExportExcel(param);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal(MimeType.XLSX, fileResult.ContentType);
        }

        [Fact]
        public async Task ExportPdf_WhenCalled_DonotHavePermission()
        {
            var paRecordId = 1;

            var controller = GetReportController();

            paRecordService.GetPermissionOnPerformanceAppraisalAsync(sessionService.EmployeeId, paRecordId).Returns(Policy.None);

            // Act
            var actual = await controller.ExportPdf(paRecordId);

            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("AccessDenied", result.ActionName);
        }

        [Fact]
        public async Task ExportPdf_WhenCalled_ExportFilePDF()
        {
            var paRecordId = 1;
            var paDetails = new PAReportDetail
            {
                Employee = new Employee { NickName = "Oliver" },
                PARecord = new PARecordModel
                {
                    Employee = new Employee { NickName = "Oliver" }
                }
            };
            byte[] bytes = Convert.FromBase64String("text");
            var controller = GetReportController();

            paRecordService.GetPermissionOnPerformanceAppraisalAsync(sessionService.EmployeeId, paRecordId).Returns(Policy.Host);
            reportService.GetPAReportDetailAsync(sessionService.EmployeeId, paRecordId).Returns(paDetails);

            exportService.ExportPdfAsync(paDetails).Returns(new Tuple<string, byte[]>("PA Detail", bytes));
            // Act
            var actual = await controller.ExportPdf(paRecordId);

            var result = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(bytes, result.FileContents);
            Assert.Equal("PA Detail", result.FileDownloadName);
            Assert.Equal(MimeType.PDF, result.ContentType);
        }

        [Fact]
        public async Task ExportPdf_WhenCalled_NoRecordFound()
        {
            var paRecordId = 1;
            var paDetails = new PAReportDetail
            {
                Employee = new Employee { NickName = "Oliver" }
            };
            byte[] bytes = Convert.FromBase64String("text");
            var controller = GetReportController();

            paRecordService.GetPermissionOnPerformanceAppraisalAsync(sessionService.EmployeeId, paRecordId).Returns(Policy.Host);
            reportService.GetPAReportDetailAsync(sessionService.EmployeeId, paRecordId).Returns(paDetails);

            exportService.ExportPdfAsync(paDetails).Returns(new Tuple<string, byte[]>("PA Detail", bytes));
            // Act
            var actual = await controller.ExportPdf(paRecordId);

            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("PageNotFound", result.ActionName);
        }

        #region Helpers

        private IEnumerable<PARecordGroupByPeriod> FakeDataListPA()
        {
            var listPARecords = new List<PARecordModel>
            {
                new PARecordModel
                {
                    EmployeeId = 363,
                    Toeic = 760,
                    Employee = new Employee { NickName = "Tim"}
                }
            };
            var listGroup = new List<PAReportModel>
            {
                new PAReportModel
                {
                    Group = new Group{ Name = "Nexus" },
                    PARecords = listPARecords
                }
            };
            var records = new List<PARecordGroupByPeriod>
            {
               new PARecordGroupByPeriod
               {
                   PeriodId = 1,
                   Groups = listGroup,
                   Period = new PeriodBaseModel { Name="1H-2019"}
               }
            };

            return records;
        }

        private PAReportController GetReportController()
        {
            sessionService = Helpers.FakeSessionService();
            reportService = Substitute.For<IPAReportService>();
            exportService = Substitute.For<IExportPAReportService>();
            paRecordService = Substitute.For<IPARecordService>();
            var controller = new PAReportController(sessionService, reportService, exportService, paRecordService);

            return controller;
        }

        #endregion Helpers
    }
}