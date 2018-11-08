namespace NexGoal.Services
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;

    public class AvatarService : IAvatarService
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public AvatarService(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public string GetFilePath(string code)
        {
            string filePath = $"{hostingEnvironment.WebRootPath}\\dist\\avatars\\{code}.jpg";

            if (File.Exists(filePath))
            {
                return $"dist/avatars/{code}.jpg";
            }

            return "dist/avatars/avatar.jpg";
        }
    }
}