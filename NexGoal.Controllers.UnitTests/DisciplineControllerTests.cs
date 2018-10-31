namespace NexGoal.Controllers.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Models.Discipline;
    using NexGoal.Services;
    using NSubstitute;
    using Xunit;

    public class DisciplineControllerTests
    {
        private IDisciplineService disciplineService;
        private ISessionService sessionService;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewResult()
        {
            var controller = GetDisciplineController();
            sessionService.HasPermissionOnFunction(FunctionKey.DisciplineManagement, Policy.Host).Returns(true);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<bool>(viewResult.Model);
            Assert.True(viewModel);
        }

        [Fact]
        public async Task GetDisciplines_WhenCalled_ReturnExpectedData()
        {
            var expectedResult = new List<DisciplineModel>
            {
                new DisciplineModel
                {
                    DisciplineId = 1,
                }
            };

            var controller = GetDisciplineController();
            disciplineService.GetDisciplines(DateTime.Today, DateTime.Today, null, null, null).Returns(expectedResult);

            var result = await controller.GetDisciplines(DateTime.Today, DateTime.Today, null, null, null);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var json = Assert.IsAssignableFrom<IEnumerable<DisciplineModel>>(jsonResult.Value);
            Assert.Equal(expectedResult.Count, json.Count());
        }

        private DisciplineController GetDisciplineController()
        {
            disciplineService = Substitute.For<IDisciplineService>();
            sessionService = Substitute.For<ISessionService>();

            return new DisciplineController(disciplineService, sessionService);
        }
    }
}