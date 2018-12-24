namespace NexGoal.Core.Models
{
    public class AppSettings
    {
        public string ApplicationDomain { get; set; }
        public string CertFilePath { get; set; }
        public string PasswordToSignCertFile { get; set; }
        public bool EnableEncrypt { get; set; }
        public string AvatarSourcePath { get; set; }
        public string AvatarTargetPath { get; set; }
    }
}