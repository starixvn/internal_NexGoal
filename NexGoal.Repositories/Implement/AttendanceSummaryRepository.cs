namespace NexGoal.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model.AttendanceSummary;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;
    using NexGoal.Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class AttendanceSummaryRepository : IAttendanceSummaryRepository
    {
        public IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<AttendanceSummaryModel, Employee, Group, JobTitle, AttendanceSummaryModel>(
                    (att, emp, gr, job) =>
                    {
                        att.Employee = emp;
                        att.Group = gr;
                        att.JobTitle = job;
                        return att;
                    },
                    "EmployeeId, GroupId, JobTitleId",
                    criteria.GetSpParams());
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Need to review here.")]
        public RawAttendanceDetailModel GetAttendanceDetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<EmployeeModel, Group, JobTitle, EmployeeModel>(
                        (emp, gr, job) =>
                        {
                            emp.JobTitle = job;
                            emp.Group = gr;
                            return emp;
                        },
                        "GroupId, JobTitleId");

                    var firstViolations =
                        multi.Read<AttendanceDetailModel, ViolationModel, ViolationModel, ViolationModel, ViolationModel, AttendanceDetailModel>(
                        (attD, missInO, tot, laI, earO) =>
                        {
                            attD.MissingCheckInOut = missInO;
                            attD.TotalHours = tot;
                            attD.LateIn = laI;
                            attD.EarlyOut = earO;
                            return attD;
                        },
                        "MissingCheckInOutId, TotalHoursId, LateInId, EarlyOutId");

                    var lastViolations =
                        multi.Read<AttendanceDetailModel, ViolationModel, ViolationModel, ViolationModel, ViolationModel, AttendanceDetailModel>(
                        (attD, ab, up, ch, upa) =>
                        {
                            attD.Absent = ab;
                            attD.UnpaidLeave = up;
                            attD.UnpaidLeaveAbsent = upa;
                            attD.ChangeShift = ch;
                            return attD;
                        },
                        "AbsentId, UnpaidLeaveId, ChangeShiftId, UnpaidLeaveAbsentId");

                    var commentViolations = multi.Read<AttendanceDetailModel>();
                    IEnumerable<AttendanceDetailModel> violations = new List<AttendanceDetailModel>();

                    if (firstViolations != null && firstViolations.Any())
                    {
                        violations = violations.Concat(firstViolations);
                    }

                    if (lastViolations != null && lastViolations.Any())
                    {
                        violations = violations.Concat(lastViolations);
                    }

                    var rawAttendanceDetail = new RawAttendanceDetailModel();

                    if (employees != null && employees.Any())
                    {
                        rawAttendanceDetail.Employees = employees;
                        rawAttendanceDetail.Violations = violations.ToList();
                        rawAttendanceDetail.CommentViolations = commentViolations;
                    }

                    return rawAttendanceDetail;
                }
            }
        }
    }
}