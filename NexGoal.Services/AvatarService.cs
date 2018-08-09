namespace NexGoal.Services
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Options;
    using NexGoal.Core.Models;

    public class AvatarService : IAvatarService
    {
        private readonly IOptionsSnapshot<AppSettings> configuration;
        private readonly IHostingEnvironment hostingEnvironment;

        public AvatarService(
            IOptionsSnapshot<AppSettings> configuration,
            IHostingEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        public string GetBase64Image(string code)
        {
            string fileName = $"{code}.jpg";
            string base64String = string.Empty;
            string filePath = $"{hostingEnvironment.WebRootPath}\\sources\\styles\\images\\avatar.jpg";

            if (File.Exists(configuration.Value.AvatarSourcePath + fileName))
            {
                filePath = configuration.Value.AvatarSourcePath + fileName;
            }

            byte[] imageBytes = File.ReadAllBytes(filePath);
            base64String = string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(imageBytes));

            return base64String;
        }
    }
}