namespace NexGoal.Services.UnitTests
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models;
    using NSubstitute;

    public static class Helpers
    {
        public static IEnumerable<PerformanceGrade> GetPerformanceGrades()
        {
            var performanceGrades = new List<PerformanceGrade>
            {
                new PerformanceGrade { GradeId = 1, Name = "EE" },
                new PerformanceGrade { GradeId = 2, Name = "EE+" }
            };

            return performanceGrades;
        }

        public static IBaseService FakeBaseService() => Substitute.For<IBaseService>();

        public static IBaseMailService FakeBaseMailService() => Substitute.For<IBaseMailService>();

        public static ICryptoService FakeCryptoService() => Substitute.For<ICryptoService>();

        public static ICommonService FakeCommonService() => Substitute.For<ICommonService>();

        public static IWebHostEnvironment FakeHostingEnvironment() => Substitute.For<IWebHostEnvironment>();

        public static ISkypeNotiService FakeSkypeNotiService() => Substitute.For<ISkypeNotiService>();

        public static AppSettings FakeAppSettings()
        {
            var appSettings = new AppSettings
            {
                ApplicationDomain = "https://app-domain.dev"
            };

            return appSettings;
        }
    }
}