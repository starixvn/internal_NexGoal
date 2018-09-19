namespace NexGoal.AutoService.Services
{
    using NexGoal.AutoService.Criterias.PerformanceRecord;
    using NexGoal.AutoService.Models;

    public class PerformanceRecordService : IPerformanceRecordService
    {
        private readonly IBaseService baseService;

        public PerformanceRecordService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public CommandResult EditStatus()
        {
            var criteria = new EditStatusCriteria();

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}