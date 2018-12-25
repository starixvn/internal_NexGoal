namespace NexGoal.Services
{
    using System;
    using Microsoft.Extensions.Options;
    using NexGoal.Core.Models;

    public class AvatarService : IAvatarService
    {
        private readonly IOptionsSnapshot<AppSettings> configuration;

        public AvatarService(IOptionsSnapshot<AppSettings> configuration)
        {
            this.configuration = configuration;
        }

        public string GetFilePath(string code)
        {
            var imgUrl = new Uri($"{configuration.Value.ImageUrl.ToString()}/{code}.jpg");

            return imgUrl.ToString();
        }
    }
}