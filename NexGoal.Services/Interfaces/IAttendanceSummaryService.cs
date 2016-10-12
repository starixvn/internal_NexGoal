namespace NexGoal.Services.Interfaces
{
    using NexGoal.Core.Model;
    using NexGoal.Criteria.AttendanceSummary;
    using NexGoal.Model.AttendanceSummary;
    using System.Collections.Generic;

    public interface IAttendanceSummaryService
    {
        IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(GetAttendanceSummaryCriteria criteria);

        IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsOfEmployeeCriteria criteria);

        CommandResult InsertOtherViolation(InsertOtherViolationsCriteria criteria);

        CommandResult DeleteOtherViolation(DeleteOtherViolationCriteria criteria);

        IEnumerable<ViolationModel> GetViolationType(GetViolationTypeCriteria criteria);

        IEnumerable<AttendanceDetailModel> GetAttendanceDetail(GetDetailAttendanceCriteria criteria);

        CommandResult UpdateIgnoredViolation(UpdateIgnoredViolationCriteria criteria);

        IEnumerable<AttendanceScoreModel> GetAttendanceScoreByEPAId(GetAttendanceScoreByEPAIdCriteria criteria);

        void ExportAttendanceSummaryReport(GetAttendanceSummaryCriteria criteria, IEnumerable<AttendanceSummaryModel> attendanceEmployees, string templatePath);

        void ExportAttendanceDetailReport(GetDetailAttendanceCriteria criteria, IEnumerable<AttendanceDetailModel> attendanceViolations, string templatePath);

        IList<OtherViolationTypeModel> GetOtherViolationType(GetOtherViolationTypeCriteria criteria);
    }
}