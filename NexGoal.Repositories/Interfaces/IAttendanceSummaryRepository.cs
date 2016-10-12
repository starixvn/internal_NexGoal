namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model.AttendanceSummary;
    using System.Collections.Generic;

    public interface IAttendanceSummaryRepository
    {
        IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(ICriteria criteria);

        RawAttendanceDetailModel GetAttendanceDetail(ICriteria criteria);
    }
}