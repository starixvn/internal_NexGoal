namespace NexGoal.Core
{
    using System.Configuration;

    public static class AppSetting
    {
        public static bool EnableEncrypt
        {
            get
            {
                string enableEnCrypt = ConfigurationManager.AppSettings["EnableEncrypt"];

                return string.IsNullOrEmpty(enableEnCrypt) ? true : bool.Parse(enableEnCrypt);
            }
        }

        public static string CertificateFilePath
        {
            get
            {
                string path = ConfigurationManager.AppSettings["CertificateFilePath"];

                return path;
            }
        }

        public static string PasswordToSignTheCertificateFile
        {
            get
            {
                string password = ConfigurationManager.AppSettings["PasswordToSignTheCertificateFile"];

                return password;
            }
        }

        public static string ApplicationDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationDomain"];
            }
        }

        public static string AvatarSourcePath
        {
            get
            {
                return ConfigurationManager.AppSettings["AvatarSourcePath"];
            }
        }
    }
}