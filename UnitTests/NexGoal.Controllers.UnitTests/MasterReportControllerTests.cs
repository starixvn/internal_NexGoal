namespace NexGoal.Controllers.UnitTests
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class MasterReportControllerTests
    {
        private IMasterReportService masterReportService;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewResult()
        {
            var controller = GetContronller();

            var actual = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Export_ValiData_ReturnExpectedFileResult()
        {
            var controller = GetContronller();

            masterReportService.ExportAsync(1, "1", "1,2,3").Returns(new byte[10]);

            var actual = await controller.Export(1, "1", "1,2,3");

            var fileContentResult = Assert.IsType<FileContentResult>(actual);
            Assert.Equal("Master Report.xlsx", fileContentResult.FileDownloadName);
            Assert.NotEmpty(fileContentResult.FileContents);
        }

        private MasterReportController GetContronller()
        {
            masterReportService = Substitute.For<IMasterReportService>();

            var controller = new MasterReportController(masterReportService);

            return controller;
        }
    }
}