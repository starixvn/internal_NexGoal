namespace NexGoal.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.Period;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Model.Report.EPASummary;
    using NexGoal.Model.ScoreRatingType;
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

        public EPASummaryModel GetEPASummary(ICriteria criteria)
        {
            EPASummaryModel epaSummary = new EPASummaryModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaSummary.PeriodSelecteds = multi.Read<PeriodModel>().ToList();

                    epaSummary.EPAInfos = multi.Read<EPASummaryEmployeeModel, Group, JobTitle, EPASummaryEmployeeModel>(
                        (res, gr, job) =>
                        {
                            res.Group = gr;
                            res.JobTitle = job;

                            return res;
                        },
                        "GroupId, JobTitleId").ToList();

                    IList<EPASummaryPeriodModel> periods = multi.Read<EPASummaryPeriodModel, EPAStatusModel, EPASummaryPeriodModel>(
                        (res, status) =>
                        {
                            res.Status = status;

                            return res;
                        },
                        "StatusId").ToList();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaSummary.TotalItem = totalItem;

                    GroupPeriodByEmployee(epaSummary.EPAInfos, periods);
                    DecodeFinalScoreAndGrade(epaSummary.EPAInfos);
                }
            }

            return epaSummary;
        }

        public EPADetailModel GetEPADetail(ICriteria criteria)
        {
            EPADetailModel epaDetail = new EPADetailModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaDetail.MemberEPAs = multi.Read<EPAModel, EPAEmployeeModel, EPAStatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                    (info, employee, status, period, scoreRatingType) =>
                    {
                        info.Employee = employee;
                        info.Status = status;
                        info.Period = period;
                        info.ScoreRatingType = scoreRatingType;

                        return info;
                    },
                    "EmployeeId, StatusId, PeriodId, ScoreRatingTypeId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaDetail.TotalItem = totalItem;

                    GroupEPAProcessByEPA(epaDetail.MemberEPAs, processes);
                    DecryptAllScore(epaDetail.MemberEPAs);
                }
            }

            return epaDetail;
        }

        public EPADetailModel GetMonthlyEPADetail(ICriteria criteria)
        {
            EPADetailModel epaDetail = new EPADetailModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaDetail.PhaseSelecteds = multi.Read<EvaluationPhaseModel>();

                    epaDetail.MemberEPAs = multi.Read<EPAModel, EPAEmployeeModel, EPAStatusModel, EPAPeriodModel, EPAModel>(
                    (info, employee, status, period) =>
                    {
                        info.Employee = employee;
                        info.Status = status;
                        info.Period = period;

                        return info;
                    },
                    "EmployeeId, StatusId, PeriodId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaDetail.TotalItem = totalItem;

                    GroupEPAProcessByEPA(epaDetail.MemberEPAs, processes);
                    DecryptAllScore(epaDetail.MemberEPAs);
                }
            }

            return epaDetail;
        }

        public EPASummaryModel GetMonthlyEPASummary(ICriteria criteria)
        {
            EPASummaryModel epaSummary = new EPASummaryModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaSummary.PeriodSelecteds = multi.Read<PeriodModel>().ToList();

                    epaSummary.EPAInfos = multi.Read<EPASummaryEmployeeModel, Group, JobTitle, EPASummaryEmployeeModel>(
                        (res, gr, job) =>
                        {
                            res.Group = gr;
                            res.JobTitle = job;

                            return res;
                        },
                        "GroupId, JobTitleId").ToList();

                    IList<EPASummaryPeriodModel> periods = multi.Read<EPASummaryPeriodModel, EPAStatusModel, EPASummaryPeriodModel>(
                        (res, status) =>
                        {
                            res.Status = status;

                            return res;
                        },
                        "StatusId").ToList();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaSummary.TotalItem = totalItem;

                    GroupPeriodByEmployee(epaSummary.EPAInfos, periods);
                    DecodeFinalScoreAndGrade(epaSummary.EPAInfos);
                }
            }

            return epaSummary;
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

        private void GroupEPAProcessByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAModel epaInfo in epaInfos)
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

        private void DecodeFinalScoreAndGrade(IEnumerable<EPASummaryEmployeeModel> epaSummaries)
        {
            foreach (EPASummaryPeriodModel period in epaSummaries.SelectMany(x => x.Periods))
            {
                period.Decrypt();
            }
        }

        private void DecryptAllScore(IEnumerable<EPAModel> epaInfos)
        {
            foreach (EPAModel epaInfo in epaInfos)
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