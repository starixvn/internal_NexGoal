namespace NexGoal.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Model.Report.EPASummary;
    using NexGoal.Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class ReportRepository : IReportRepository
    {
        public IEnumerable<MasterReportModel> GetMaster(ICriteria criteria)
        {
            IEnumerable<MasterReportModel> employees = new List<MasterReportModel>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    employees = multi.Read<MasterReportModel>();

                    var nominations = multi.Read<NominationReportModel>();

                    GroupMasterReport(employees, nominations);
                }
            }

            return employees;
        }

        private IEnumerable<MasterReportModel> GroupMasterReport(
            IEnumerable<MasterReportModel> reports,
            IEnumerable<NominationReportModel> nominations)
        {
            foreach (var employee in reports)
            {
                var nominationOfEmployee = nominations.Where(i => i.NomineeId == employee.PAFocus.Employee.EmployeeId);
                employee.Nominations.AddRange(nominationOfEmployee);
            }

            return reports;
        }

        public IEnumerable<EPADetailModel> GetEPADetail(ICriteria criteria)
        {
            IEnumerable<EPADetailModel> epaDetail = new List<EPADetailModel>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaDetail = multi.Read<EPADetailModel, EPAEmployeeModel, EPAStatusModel, EPAPeriodModel, EPADetailModel>(
                    (info, employee, status, period) =>
                    {
                        info.Employee = employee;
                        info.Status = status;
                        info.Period = period;

                        return info;
                    },
                    "EmployeeId, StatusId, PeriodId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(epaDetail, processes);
                    DecryptAllScore(epaDetail);
                }
            }

            return epaDetail;
        }

        private void GroupPeriodByEmployee(IEnumerable<EPASummaryEmployeeModel> epaSummaries, IList<EPASummaryPeriodModel> periods)
        {
            foreach (EPASummaryEmployeeModel epaSummary in epaSummaries)
            {
                foreach (EPASummaryPeriodModel period in periods.Where(x => epaSummary.EmployeeId.Equals(x.EmployeeId)))
                {
                    epaSummary.Periods.Add(period);
                }
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

        private void DecryptTotalScore(IList<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                process.Decrypt();
            }
        }
    }
}