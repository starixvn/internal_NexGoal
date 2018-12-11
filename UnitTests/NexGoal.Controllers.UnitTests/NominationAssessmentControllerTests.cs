namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
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
        public async Task GetNominationPeriodsForComittee_WhenCalled_ReturnExpecetedJsonResult()
        {
            var controller = GetNominationAssessmentController();
            employeeNominationService.GetNominationPeriodsForComitteeAsync(sessionService.EmployeeId).Returns(new List<NominationPeriodModel>
            {
                new NominationPeriodModel { NominationPeriodId = 1, Year = 2019, Quarter =1 }
            });

            var actual = await controller.GetNominationPeriodsForComittee();

            Assert.IsType<JsonResult>(actual);
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