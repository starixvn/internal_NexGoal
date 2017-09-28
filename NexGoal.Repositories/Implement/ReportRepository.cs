namespace NexGoal.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.Department;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.Employee;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.Nomination;
    using NexGoal.Model.PAFocus;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class ReportRepository : IReportRepository
    {
        public IEnumerable<MasterReportModel> GetMaster(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<MasterReportModel, PAFocusModel, EmployeeModel, JobTitle, Group, MasterReportModel>(
                        (report, focus, emp, job, gr) =>
                        {
                            report.PAFocus = focus;
                            focus.Employee = emp;
                            emp.JobTitle = job;
                            emp.Group = gr;

                            return report;
                        },
                        "PAFocusId,EmployeeId,JobTitleId,GroupId");

                    var evaluatorComments = multi.Read<EvaluatorCommentReportModel>();
                    var nominations = multi.Read<NominationReportModel>();
                    var nominators = multi.Read<NominatorReportModel>();

                    GroupMasterReport(employees, evaluatorComments, nominations, nominators);

                    return employees;
                }
            }
        }

        public IEnumerable<EPADetailModel> GetEPADetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var epaDetail = multi.Read<EPADetailModel, EPAEmployeeModel, EPAStatusModel, EPAPeriodModel, EPADetailModel>(
                        (info, employee, status, period) =>
                        {
                            info.Employee = employee;
                            info.Status = status;
                            info.Period = period;

                            return info;
                        },
                        "EmployeeId, StatusId, PeriodId");

                    var processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(epaDetail, processes);
                    DecryptAllScore(epaDetail);

                    return epaDetail;
                }
            }
        }

        public IEnumerable<NominationModel> GetNomination(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var nominations =
                    db.Query<NominationModel, EOQYModel, NomineeModel, DepartmentModel, Group, JobTitle, NomineeModel, NominationModel>(
                        (nomination, period, nominee, dept, gr, jb, nominator) =>
                        {
                            nomination.Period = period;
                            nominee.Department = dept;
                            nominee.Group = gr;
                            nominee.JobTitle = jb;
                            nomination.Nominee = nominee;
                            nomination.Nominator = nominator;

                            return nomination;
                        },
                        "EOQYId, EmployeeId, DepartmentId, GroupId, JobTitleId, NominatorId",
                        criteria.GetSpParams());

                return nominations;
            }
        }

        private void GroupMasterReport(
            IEnumerable<MasterReportModel> reports,
            IEnumerable<EvaluatorCommentReportModel> evaluatorComments,
            IEnumerable<NominationReportModel> nominations,
            IEnumerable<NominatorReportModel> nominators)
        {
            foreach (var employee in reports)
            {
                var evaluatorCommentOfEmployee = evaluatorComments.Where(i => i.EmployeeId == employee.PAFocus.Employee.EmployeeId);
                var nominationsOfEmployee = nominations.Where(i => i.NomineeId == employee.PAFocus.Employee.EmployeeId);
                foreach (var nomination in nominationsOfEmployee)
                {
                    var nominatorsOfEmployee = nominators.Where(i => i.NomineeId == nomination.NomineeId);
                    nomination.Nominators.AddRange(nominatorsOfEmployee);
                }

                employee.Nominations.AddRange(nominationsOfEmployee);
                employee.EvaluatorComments.AddRange(evaluatorCommentOfEmployee);
            }
        }

        private void GroupEPAProcessByEPA(IEnumerable<EPADetailModel> epaInfos, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPADetailModel epaInfo in epaInfos)
            {
                foreach (EPAProcessModel process in processes.Where(x => x.EPAId == epaInfo.EPAId))
                {
                    switch (process.ProcessId)
                    {
                        case (int)ProcessType.Self:
                            epaInfo.Process.Self = process;
                            break;

                        case (int)ProcessType.Evaluator:
                            epaInfo.Process.Evaluators.Add(process);
                            break;

                        case (int)ProcessType.Reviewer:
                            epaInfo.Process.Reviewers.Add(process);
                            break;
                    }
                }
            }
        }

        private void DecryptAllScore(IEnumerable<EPADetailModel> epaInfos)
        {
            foreach (EPADetailModel epaInfo in epaInfos)
            {
                epaInfo.Decrypt();
            }
        }
    }
}