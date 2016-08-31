namespace NexGoal.Tools
{
    public static class Constants
    {
        public const string ConnectionStringTemplate = @"packet size=4096;Data Source={0};TrustServerCertificate=true;Connect Timeout=60;
            Asynchronous Processing=true;Initial Catalog={1};integrated security=false; user id={2}; password={3};";
    }
}