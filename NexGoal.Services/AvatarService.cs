namespace NexGoal.Services
{
    using System;
    using System.Net;
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
            bool exist;
            var imgUrl = new Uri($"{configuration.Value.ImageUrl.ToString()}/{code}.jpg");

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imgUrl);
                request.Method = "HEAD";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                exist = response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                exist = false;
            }

            if (exist)
            {
                return imgUrl.ToString();
            }

            return "/dist/avatars/avatar.jpg";
        }
    }
}