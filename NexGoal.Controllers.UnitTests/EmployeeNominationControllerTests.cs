namespace NexGoal.Controllers.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class EmployeeNominationControllerTests
    {
        private IEmployeeNominationService employeeNominationService;
        private ISessionService sessionService;

        [Fact]
        public void Create_WhenCalled_ReturnExpectedViewResult()
        {
            var controller = GetController();

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task GetNominationPeriodsForNominator_WhenCalled_ReturnExpectedResult()
        {
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
            employeeNominationService.GetNominationPeriodsForNominator(sessionService.EmployeeId).Returns(periods);

            var result = await controller.GetNominationPeriodsForNominator();

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task Create_ValidData_ReturnCommandResult()
        {
            var controller = GetController();
            employeeNominationService.Create(Arg.Any<CreateEmployeeNominationModel>()).Returns(new CommandResult { Code = 0 });

            var result = await controller.Create(new CreateEmployeeNominationViewModel());

            var jsonResult = Assert.IsType<JsonResult>(result);
            var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
            Assert.True(commandResult.IsSuccessful);
        }

        [Fact]
        public async Task Create_InvalidData_ReturnErrors()
        {
            var controller = GetController();
            controller.ModelState.AddModelError(nameof(CreateEmployeeNominationViewModel.NomineeId), "Nominee is required");

            var result = await controller.Create(new CreateEmployeeNominationViewModel());

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        private EmployeeNominationController GetController()
        {
            employeeNominationService = Substitute.For<IEmployeeNominationService>();
            sessionService = Substitute.For<ISessionService>();
            sessionService.EmployeeId = 1;

            return new EmployeeNominationController(employeeNominationService, sessionService);
        }
    }
}