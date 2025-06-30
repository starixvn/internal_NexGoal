namespace NexGoal.BackgroundJob.UnitTests
{
    using Microsoft.AspNetCore.Hosting;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NSubstitute;

    public static class Helpers
    {
        public static IBaseService FakeBaseService() => Substitute.For<IBaseService>();

        public static IBaseMailService FakeBaseMailService() => Substitute.For<IBaseMailService>();

        public static ICryptoService FakeCryptoService() => Substitute.For<ICryptoService>();

        public static IWebHostEnvironment FakeHostingEnvironment() => Substitute.For<IWebHostEnvironment>();

        public static IChatbotService FakeChatbotService() => Substitute.For<IChatbotService>();

        public static AppSettings FakeAppSettings()
        {
            var appSettings = new AppSettings
            {
                ApplicationDomain = "https://goal.nexcel.vn"
            };

            return appSettings;
        }
    }
}