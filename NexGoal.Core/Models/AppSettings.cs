namespace NexGoal.Core.Models
{
    public class AppSettings
    {
        public string CertFilePath { get; set; }
        public string PasswordToSignCertFile { get; set; }
        public bool EnableEncrypt { get; set; }
        public string AvatarSourcePath { get; set; }
    }
}