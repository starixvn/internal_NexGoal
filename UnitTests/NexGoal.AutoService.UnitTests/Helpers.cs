namespace NexGoal.AutoService.UnitTests
{
    using Microsoft.Extensions.Options;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NSubstitute;

    public static class Helpers
    {
        public static IBaseService FakeBaseService() => Substitute.For<IBaseService>();

        public static IBaseMailService FakeBaseMailService() => Substitute.For<IBaseMailService>();

        public static ICryptoService FakeCryptoService() => Substitute.For<ICryptoService>();

        public static IOptionsSnapshot<AppSettings> FakeOptionsSnapshot()
        {
            var appSettings = new AppSettings
            {
                ApplicationDomain = "https://goal.nexcel.vn"
            };

            var config = Substitute.For<IOptionsSnapshot<AppSettings>>();
            config.Value.Returns(appSettings);

            return config;
        }
    }
}