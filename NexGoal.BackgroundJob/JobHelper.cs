namespace NexGoal.BackgroundJob
{
    public static class JobHelper
    {
        public static JobSetting? GetJobConfig(IEnumerable<JobSetting> jobSettings, string jobName)
        {
            var jobSetting = jobSettings.FirstOrDefault(job => job.JobName.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            return jobSetting;
        }

        public static bool IsEnabledJob(IEnumerable<JobSetting> jobConfigs, string jobName)
        {
            var settings = GetJobConfig(jobConfigs, jobName);

            return settings != null && settings.IsEnabled;
        }
    }
}
