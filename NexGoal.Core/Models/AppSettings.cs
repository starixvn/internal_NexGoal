namespace NexGoal.Core.Models
{
    using System;

    public class AppSettings
    {
        public string ApplicationName { get; set; }
        public string ApplicationDomain { get; set; }
        public Uri ImageUrl { get; set; }
        public string RSAPublicKeyPath { get; set; }
        public string RSAPrivateKeyPath { get; set; }
        public bool EnableEncrypt { get; set; }
    }
}