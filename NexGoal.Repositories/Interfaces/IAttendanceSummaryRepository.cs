namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model.AttendanceSummary;

    public interface IAttendanceSummaryRepository
    {
        IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(ICriteria criteria);

        RawAttendanceDetailModel GetAttendanceDetail(ICriteria criteria);
    }
}