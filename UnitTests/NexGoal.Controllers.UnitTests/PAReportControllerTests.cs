namespace NexGoal.Controllers.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
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
            var controller = new PAReportController(sessionService, reportService, exportService);

            return controller;
        }

        #endregion Helpers
    }
}