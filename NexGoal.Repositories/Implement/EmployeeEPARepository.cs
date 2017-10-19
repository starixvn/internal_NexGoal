namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.Dashboard;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.ScoreRatingType;
    using NexGoal.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public IEnumerable<EPAKPIGroupModel> GetLastRating(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var groups = multi.Read<EPAKPIGroupModel>();
                    var kpis = multi.Read<EPAKPIModel>();
                    var scoreByKPIs = multi.Read<EvaluationModel>();

                    GroupKPIByKPIGroup(groups, kpis);

                    foreach (EPAKPIGroupModel kpiGroup in groups)
                    {
                        GroupEvaluationOnEachKPI(kpiGroup.KPIs, scoreByKPIs);
                        kpiGroup.Decrypt();
                    }

                    return groups;
                }
            }
        }

        public IEnumerable<EPAModel> GetEPA(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var epaInfos = multi.Read<EPAModel, EPAEmployeeModel, RatingTypeModel, EPAStatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                    (epaInfo, employee, ratingType, status, period, scoreRatingType) =>
                    {
                        epaInfo.Employee = employee;
                        epaInfo.RatingType = ratingType;
                        epaInfo.ScoreRatingType = scoreRatingType;
                        epaInfo.Status = status;
                        epaInfo.Period = period;

                        return epaInfo;
                    },
                    "EmployeeId, RatingTypeId, StatusId, PeriodId, ScoreRatingTypeId");

                    var processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(epaInfos, processes);
                    foreach (EPAModel epaInfo in epaInfos)
                    {
                        epaInfo.Decrypt();
                    }

                    return epaInfos;
                }
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Need to review here.")]
        public EPAModel GetEPAInfo(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var epaInfo = multi.Read<EPAModel, EPAEmployeeModel, RatingTypeModel, EPAStatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                    (info, employee, ratingType, status, period, scoreRatingType) =>
                    {
                        info.Employee = employee;
                        info.RatingType = ratingType;
                        info.ScoreRatingType = scoreRatingType;
                        info.Status = status;
                        info.Period = period;

                        return info;
                    },
                    "EmployeeId, RatingTypeId, StatusId, PeriodId, ScoreRatingTypeId").FirstOrDefault();

                    if (epaInfo != null)
                    {
                        var processes = multi.Read<EPAProcessModel>();

                        epaInfo.KPIGroups = multi.Read<EPAKPIGroupModel>().ToList();

                        var kpis = multi.Read<EPAKPIModel>();
                        var evaluations = multi.Read<EvaluationModel>();

                        GroupEPAProcessByEPA(epaInfo, processes);
                        GroupKPIByKPIGroup(epaInfo.KPIGroups, kpis);

                        foreach (EPAKPIGroupModel kpiGroup in epaInfo.KPIGroups)
                        {
                            GroupEvaluationOnEachKPI(kpiGroup.KPIs, evaluations);
                        }

                        epaInfo.Decrypt();
                    }

                    return epaInfo;
                }
            }
        }

        public IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var epaInfos = multi.Read<EPAModel, EPAStatusModel, EPAEmployeeModel, EPAPeriodModel, EPAModel>(
                    (epaInfo, status, emp, period) =>
                    {
                        epaInfo.Status = status;
                        epaInfo.Employee = emp;
                        epaInfo.Period = period;

                        return epaInfo;
                    },
                    "StatusId, EmployeeId, PeriodId");

                    var processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(epaInfos, processes);

                    foreach (EPAModel epaInfo in epaInfos)
                    {
                        epaInfo.Decrypt();
                    }

                    return epaInfos;
                }
            }
        }

        public IList<EPAModel> GetEPAToUpdateFinalScore(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IList<EPAModel> epaInfos = multi.Read<EPAModel, EPAStatusModel, EPAEmployeeModel, EPAPeriodModel, EPAModel>(
                    (epaInfo, status, emp, period) =>
                    {
                        epaInfo.Status = status;
                        epaInfo.Employee = emp;
                        epaInfo.Period = period;

                        return epaInfo;
                    },
                    "StatusId, EmployeeId, PeriodId").ToList();

                    var processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(epaInfos, processes);

                    foreach (EPAModel epaInfo in epaInfos)
                    {
                        epaInfo.Decrypt();
                    }

                    return epaInfos;
                }
            }
        }

        public DashboardModel GetDashboard(ICriteria criteria)
        {
            var result = new DashboardModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    result.MyEPAs = multi.Read<EPAModel, EPAStatusModel, EPAPeriodModel, EPAModel>(
                        (epa, status, period) =>
                        {
                            epa.Status = status;
                            epa.Period = period;

                            return epa;
                        },
                        "StatusId, PeriodId");

                    var processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(result.MyEPAs, processes);

                    foreach (EPAModel epaInfo in result.MyEPAs)
                    {
                        epaInfo.Decrypt();
                    }
                }
            }

            return result;
        }

        #region Rating By KPI

        public IEnumerable<TemporaryKPIModel> GetKPIToView(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                    (kpi, gr) =>
                    {
                        kpi.KPIGroup = gr;

                        return kpi;
                    },
                    "KPIGroupId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var epaInfos = multi.Read<RatingByKPIModel, EPAEmployeeModel, RatingTypeModel, TemporaryKPIGroupModel, RatingByKPIModel>(
                        (epaInfo, employee, type, kgr) =>
                        {
                            epaInfo.Employee = employee;
                            epaInfo.RatingType = type;
                            epaInfo.KPIGroup = kgr;
                            return epaInfo;
                        },
                        "EmployeeId, RatingTypeId, KPIGroupId");

                    var kpis = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                        (kpi, kgr) =>
                        {
                            kpi.KPIGroup = kgr;
                            return kpi;
                        },
                        "KPIGroupId");

                    var kpisBySelf = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                       (kpi, kgr) =>
                       {
                           kpi.KPIGroup = kgr;
                           return kpi;
                       },
                       "KPIGroupId");

                    GroupKPIByEPA(epaInfos, kpis);
                    GroupKPIBySelf(epaInfos, kpisBySelf);
                    DecodeAllScore(epaInfos);

                    return epaInfos;
                }
            }
        }

        #endregion Rating By KPI

        private static void GroupEPAProcessByEPA(EPAModel epaInfo, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes.Where(x => x.EPAId.Equals(epaInfo.EPAId)))
            {
                switch (process.ProcessId)
                {
                    case (int)ProcessType.HumanResource:
                        epaInfo.Process.HumanResources.Add(process);
                        break;

                    case (int)ProcessType.Self:
                        epaInfo.Process.Self = process;
                        break;

                    case (int)ProcessType.Evaluator:
                        epaInfo.Process.Evaluators.Add(process);
                        break;

                    case (int)ProcessType.Reviewer:
                        epaInfo.Process.Reviewers.Add(process);
                        break;

                    default:
                        continue;
                }
            }
        }

        private static void GroupEPAProcessByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                if (processes != null && processes.Any())
                {
                    GroupEPAProcessByEPA(epaInfo, processes);
                }
            }
        }

        private static void GroupKPIByKPIGroup(IEnumerable<EPAKPIGroupModel> kpiGroups, IEnumerable<EPAKPIModel> kpis)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                foreach (EPAKPIModel kpi in kpis.Where(x => x.EPAId.Equals(kpiGroup.EPAId) && x.KPIGroupId.Equals(kpiGroup.KPIGroupId)))
                {
                    kpiGroup.KPIs.Add(kpi);
                }
            }
        }

        private static void GroupEvaluationOnEachKPI(IEnumerable<EPAKPIModel> kpis, IEnumerable<EvaluationModel> evaluations)
        {
            foreach (EPAKPIModel kpi in kpis)
            {
                foreach (EvaluationModel evaluation in evaluations.Where(x => x.EPAId.Equals(kpi.EPAId) && x.KPIId.Equals(kpi.KPIId)))
                {
                    switch (evaluation.ProcessId)
                    {
                        case (int)ProcessType.Self:
                            kpi.EvaluationBySelf = evaluation;
                            break;

                        case (int)ProcessType.Evaluator:
                            kpi.EvaluationByEvaluators.Add(evaluation);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        private static void GroupKPIByEPA(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                foreach (TemporaryKPIModel kpi in kpis.Where(x => x.EPAId.Equals(epaInfo.EPAId) && x.KPIGroup.KPIGroupId.Equals(epaInfo.KPIGroup.KPIGroupId)))
                {
                    epaInfo.KPIGroup.KPIs.Add(kpi);
                }
            }
        }

        private static void GroupKPIBySelf(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                foreach (TemporaryKPIModel kpi in kpis.Where(x => x.EPAId.Equals(epaInfo.EPAId) && x.KPIGroup.KPIGroupId.Equals(epaInfo.KPIGroup.KPIGroupId)))
                {
                    epaInfo.KPIGroup.KPIsBySelf.Add(kpi);
                }
            }
        }

        private static void DecodeAllScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                epaInfo.Decrypt();
            }
        }
    }
}