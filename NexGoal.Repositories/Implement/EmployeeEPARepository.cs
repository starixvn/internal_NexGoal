namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.Report.Dashboard;
    using NexGoal.Model.ScoreRatingType;
    using NexGoal.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public IEnumerable<EPAKPIGroupModel> GetLastRating(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAKPIGroupModel> groups = multi.Read<EPAKPIGroupModel>();

                    IEnumerable<EvaluationModel> groupScore = multi.Read<EvaluationModel>();

                    IEnumerable<EPAKPIModel> kpis = multi.Read<EPAKPIModel>();

                    IEnumerable<EvaluationModel> scoreByKPIs = multi.Read<EvaluationModel>();

                    GroupKPIByKPIGroup(groups, kpis);

                    GroupEvaluatorScoreOnGroup(groups, groupScore);

                    foreach (EPAKPIGroupModel kpiGroup in groups)
                    {
                        GroupEvaluationOnEachKPI(kpiGroup.KPIs, scoreByKPIs);
                        kpiGroup.Decode();
                    }

                    return groups;
                }
            }
        }

        public IEnumerable<EPAModel> GetEPA(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAModel> epaInfos = multi.Read<EPAModel, EPAEmployeeModel, RatingTypeModel, EPAStatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
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

                    var kpiGroups = multi.Read<EPAKPIGroupModel>();

                    IEnumerable<EvaluationModel> groupScore = multi.Read<EvaluationModel>();

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    GroupKPIGroupOnEPA(epaInfos, kpiGroups);
                    GroupEPAProcessByEPA(epaInfos, processes);
                    foreach (EPAModel epaInfo in epaInfos)
                    {
                        GroupEvaluatorScoreOnGroup(epaInfo.KPIGroups, groupScore);
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
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    EPAModel epaInfo = new EPAModel();

                    epaInfo = multi.Read<EPAModel, EPAEmployeeModel, RatingTypeModel, EPAStatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
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
                        IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                        epaInfo.KPIGroups = multi.Read<EPAKPIGroupModel>().ToList();

                        IEnumerable<EvaluationModel> groupScore = multi.Read<EvaluationModel>();

                        IEnumerable<EPAKPIModel> kpis = multi.Read<EPAKPIModel>();

                        IEnumerable<EvaluationModel> evaluations = multi.Read<EvaluationModel>();

                        GroupEPAProcessByEPA(epaInfo, processes);
                        GroupKPIByKPIGroup(epaInfo.KPIGroups, kpis);
                        GroupEvaluatorScoreOnGroup(epaInfo.KPIGroups, groupScore);

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
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAModel> epaInfos = multi.Read<EPAModel, EPAStatusModel, EPAEmployeeModel, EPAPeriodModel, EPAModel>(
                    (epaInfo, status, emp, period) =>
                    {
                        epaInfo.Status = status;
                        epaInfo.Employee = emp;
                        epaInfo.Period = period;

                        return epaInfo;
                    },
                    "StatusId, EmployeeId, PeriodId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

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
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
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

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

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
            DashboardModel result = new DashboardModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    result.MyEPAs = multi.Read<EPAModel, EPAStatusModel, EPAPeriodModel, EPAModel>(
                        (epa, status, period) =>
                        {
                            epa.Status = status;
                            epa.Period = period;

                            return epa;
                        },
                        "StatusId, PeriodId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

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

        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(ICriteria criteria)
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

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(ICriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epaInfos = new List<RatingByKPIModel>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaInfos = multi.Read<RatingByKPIModel, EPAEmployeeModel, RatingTypeModel, TemporaryKPIGroupModel, RatingByKPIModel>(
                        (epaInfo, employee, type, kgr) =>
                        {
                            epaInfo.Employee = employee;
                            epaInfo.RatingType = type;
                            epaInfo.KPIGroup = kgr;
                            return epaInfo;
                        },
                        "EmployeeId, RatingTypeId, KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpis = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                        (kpi, kgr) =>
                        {
                            kpi.KPIGroup = kgr;
                            return kpi;
                        },
                        "KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpisBySelf = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                       (kpi, kgr) =>
                       {
                           kpi.KPIGroup = kgr;
                           return kpi;
                       },
                       "KPIGroupId");
                    GroupKPIByEPA(epaInfos, kpis);
                    GroupKPIBySelf(epaInfos, kpisBySelf);
                    DecodeAllScore(epaInfos);
                }
            }

            return epaInfos;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(ICriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epaInfos = new List<RatingByKPIModel>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaInfos = multi.Read<RatingByKPIModel, EPAEmployeeModel, RatingTypeModel, TemporaryKPIGroupModel, RatingByKPIModel>(
                        (epaInfo, employee, type, kgr) =>
                        {
                            epaInfo.Employee = employee;
                            epaInfo.RatingType = type;
                            epaInfo.KPIGroup = kgr;
                            return epaInfo;
                        },
                        "EmployeeId, RatingTypeId, KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpis = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                        (kpi, kgr) =>
                        {
                            kpi.KPIGroup = kgr;
                            return kpi;
                        },
                        "KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpisBySelf = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                       (kpi, kgr) =>
                       {
                           kpi.KPIGroup = kgr;
                           return kpi;
                       },
                       "KPIGroupId");
                    GroupKPIByEPA(epaInfos, kpis);
                    GroupKPIBySelf(epaInfos, kpisBySelf);
                    DecodeAllScore(epaInfos);
                }
            }

            return epaInfos;
        }

        #endregion Rating By KPI

        private void GroupKPIGroupOnEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAKPIGroupModel> kpiGroups)
        {
            foreach (var epa in epaInfos)
            {
                var group = kpiGroups.Where(i => i.EPAId.Equals(epa.EPAId));
                epa.KPIGroups = group.ToList();
            }
        }

        private void GroupEPAProcessByEPA(EPAModel epaInfo, IEnumerable<EPAProcessModel> processes)
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

                    case (int)ProcessType.Management:
                        epaInfo.Process.Managements.Add(process);
                        break;
                }
            }
        }

        private void GroupEPAProcessByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                if (processes != null && processes.Any())
                {
                    GroupEPAProcessByEPA(epaInfo, processes);
                }
            }
        }

        private void GroupKPIByKPIGroup(IEnumerable<EPAKPIGroupModel> kpiGroups, IEnumerable<EPAKPIModel> kpis)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                foreach (EPAKPIModel kpi in kpis.Where(x => x.EPAId.Equals(kpiGroup.EPAId) && x.KPIGroupId.Equals(kpiGroup.KPIGroupId)))
                {
                    kpiGroup.KPIs.Add(kpi);
                }
            }
        }

        private void GroupEvaluatorScoreOnGroup(IEnumerable<EPAKPIGroupModel> kpiGroups, IEnumerable<EvaluationModel> evaluations)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                var evaluatorScore = evaluations.Where(i => i.KPIGroupId.Equals(kpiGroup.KPIGroupId));
                if (evaluatorScore.Any())
                {
                    kpiGroup.EvaluatorsScore = evaluatorScore.ToList();
                }
            }
        }

        private void GroupEvaluationOnEachKPI(IEnumerable<EPAKPIModel> kpis, IEnumerable<EvaluationModel> evaluations)
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
                    }
                }
            }
        }

        private void GroupKPIByEPA(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                foreach (TemporaryKPIModel kpi in kpis.Where(x => x.EPAId.Equals(epaInfo.EPAId) && x.KPIGroup.KPIGroupId.Equals(epaInfo.KPIGroup.KPIGroupId)))
                {
                    epaInfo.KPIGroup.KPIs.Add(kpi);
                }
            }
        }

        private void GroupKPIBySelf(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                foreach (TemporaryKPIModel kpi in kpis.Where(x => x.EPAId.Equals(epaInfo.EPAId) && x.KPIGroup.KPIGroupId.Equals(epaInfo.KPIGroup.KPIGroupId)))
                {
                    epaInfo.KPIGroup.KPIsBySelf.Add(kpi);
                }
            }
        }

        private void DecodeAllScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                epaInfo.Decrypt();
            }
        }

        private void DecodeTotalScore(IList<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                process.Decrypt();
            }
        }

        private void DecodeScore(IList<EvaluationModel> evaluations)
        {
            foreach (EvaluationModel evaluation in evaluations)
            {
                evaluation.Decrypt();
            }
        }
    }
}