namespace NexGoal.Core.Models
{
    using System;

    public class AppSettings
    {
        public string ApplicationDomain { get; set; }
        public Uri ImageUrl { get; set; }
        public string CertFilePath { get; set; }
        public string PasswordToSignCertFile { get; set; }
        public bool EnableEncrypt { get; set; }
    }
}