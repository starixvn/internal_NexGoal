namespace NexGoal.BackgroundJob
{
    public static class JobHelper
    {
        public static JobSetting GetJobConfig(IEnumerable<JobSetting> jobSettings, string jobName)
        {
            return jobSettings.FirstOrDefault(job => job.JobName.Equals(jobName));
        }

        public static bool IsEnabledJob(IEnumerable<JobSetting> jobConfigs, string jobName)
        {
            var settings = GetJobConfig(jobConfigs, jobName);

            return settings != null && settings.IsEnabled;
        }
    }
}
