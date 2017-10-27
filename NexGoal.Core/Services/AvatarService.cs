namespace NexGoal.Core.Services
{
    using System;
    using System.Drawing;
    using System.IO;

    public class AvatarService : IAvatarService
    {
        public string GetAvatar(string code)
        {
            string fileName = $"{code}.jpg";
            string base64String = string.Empty;

            if (File.Exists(AppSetting.AvatarSourcePath + fileName))
            {
                using (var image = Image.FromFile(AppSetting.AvatarSourcePath + fileName))
                {
                    using (var m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        base64String = string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(imageBytes));
                    }
                }
            }

            return base64String;
        }
    }
}