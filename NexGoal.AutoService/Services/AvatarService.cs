namespace NexGoal.AutoService.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Models;

    public class AvatarService : IAvatarService
    {
        private readonly IOptionsSnapshot<AppSettings> configuration;

        public AvatarService(IOptionsSnapshot<AppSettings> configuration)
        {
            this.configuration = configuration;
        }

        public void Update()
        {
            DateTime lastModifiedDate = GetLastModifiedDate();

            DirectoryInfo sourceDirectory = new DirectoryInfo(configuration.Value.AvatarSourcePath);
            IEnumerable<FileInfo> files = sourceDirectory.GetFiles().Where(file => file.LastWriteTime >= lastModifiedDate
                && file.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase));

            foreach (FileInfo file in files)
            {
                var fullPath = configuration.Value.AvatarTargetPath + "\\" + file.Name;
                File.WriteAllBytes(fullPath, File.ReadAllBytes(file.FullName));
            }
        }

        private DateTime GetLastModifiedDate()
        {
            var lastModifiedDate = new DateTime(2010, 01, 01);

            DirectoryInfo directory = new DirectoryInfo(configuration.Value.AvatarTargetPath);
            FileInfo lastModifiedFile = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

            if (lastModifiedFile != null)
            {
                lastModifiedDate = lastModifiedFile.LastWriteTime;
            }

            return lastModifiedDate;
        }
    }
}