namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class NominationAssessmentControllerTests
    {
        private ISessionService sessionService;

        private IEmployeeNominationService employeeNominationService;

        [Fact]
        public void Index_WhenCalled_ReturnExpecetedViewResult()
        {
            var controller = GetNominationAssessmentController();

            var actual = controller.Index();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public async Task GetCurrentNominationPeriodForComittee_WhenCalled_ReturnExpecetedJsonResult()
        {
            var controller = GetNominationAssessmentController();
            employeeNominationService.GetCurrentNominationPeriodForComittee(sessionService.EmployeeId, true).Returns(
                new NominationPeriodModel { NominationPeriodId = 1, Year = 2019, Quarter = 1 }
            );

            var actual = await controller.GetCurrentNominationPeriodForComittee(true);

            Assert.IsType<JsonResult>(actual);
        }

        [Fact]
        public async Task GetNominationsByPeriodIds_WhenCalled_ReturnExpecetedJsonResult()
        {
            var controller = GetNominationAssessmentController();
            var nominationPeriodIds = "1";
            employeeNominationService.GetNominationsByPeriodIdsAsync(nominationPeriodIds, null).Returns(new List<NominationModel>
            {
                new NominationModel { NominationPeriodId = 1}
            });

            var actual = await controller.GetNominationsByPeriodIds(nominationPeriodIds, null);

            Assert.IsType<JsonResult>(actual);
        }

        [Fact]
        public async Task EditStatus_ValidData_ReturnExpectedCommandResult()
        {
            var controller = GetNominationAssessmentController();
            employeeNominationService.EditStatusAsync(1, 1, sessionService.EmployeeId).Returns(new CommandResult { Code = 0 });

            var actual = await controller.EditStatus(1, 1).ConfigureAwait(false);

            var json = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsType<CommandResult>(json.Value);
            Assert.True(commandResult.IsSuccessful);
        }

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private IEmployeeNominationService FakeEmployeeNominationService()
        {
            var service = Substitute.For<IEmployeeNominationService>();

            return service;
        }

        private NominationAssessmentController GetNominationAssessmentController()
        {
            sessionService = FakeSessionService();
            employeeNominationService = FakeEmployeeNominationService();

            var controller = new NominationAssessmentController(sessionService, employeeNominationService);

            return controller;
        }
    }
}