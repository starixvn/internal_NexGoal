namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Core.Utilities;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Criteria.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.ScoreRatingType;
    using NexGoal.Model.TargetTemplate;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EmployeeEPAService : BaseService, IEmployeeEPAService
    {
        private readonly IEmployeeEPARepository _employeeEPARepository;

        public EmployeeEPAService()
            : this(null)
        {
        }

        public EmployeeEPAService(IEmployeeEPARepository employeeEPARepository)
        {
            _employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
        }

        #region Rating By KPI

        public IEnumerable<TargetTemplateModel> GetTargetTemplateToView(GetTargetTemplateToViewCriteria criteria)
        {
            IEnumerable<TargetTemplateModel> targetTemplates = new List<TargetTemplateModel>();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplates = GetData<TargetTemplateModel>(criteria);
            }

            return targetTemplates;
        }

        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria)
        {
            IEnumerable<TemporaryKPIModel> kpis = new List<TemporaryKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = _employeeEPARepository.GetKPIByTargetTemplate(criteria);
            }

            return kpis;
        }

        public IEnumerable<TemporaryKPIModel> GetKPIToView(GetKPIToViewCriteria criteria)
        {
            IEnumerable<TemporaryKPIModel> kpis = new List<TemporaryKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = _employeeEPARepository.GetKPIToView(criteria);
            }

            return kpis;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(GetEmployeeEPAToViewCriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epas = new List<RatingByKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEmployeeEPAToView(criteria);
            }

            return epas;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(GetEmployeeEPAByKPICriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epas = new List<RatingByKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEmployeeEPAByKPI(criteria);
            }

            return epas;
        }

        public CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<Employee> GetEvaluatorsByTargetTemplate(GetEvaluatorsByTargetTemplateCriteria criteria)
        {
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<Employee>(criteria);
            }

            return employees;
        }

        #endregion Rating By KPI

        public IEnumerable<EPAPeriodModel> GetPeriodToView(GetPeriodToViewCriteria criteria)
        {
            IEnumerable<EPAPeriodModel> periods = new List<EPAPeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = GetData<EPAPeriodModel>(criteria);
            }

            return periods;
        }

        public EPAModel GetEPAInfoById(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            var criteria = new GetEPAInfoByIdCriteria { EPAId = epaId };
            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
                SpreadKPIScore(epaInfo.KPIGroups);

                if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
        }

        public IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria)
        {
            IEnumerable<EPAModel> epaInfos = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epaInfos = _employeeEPARepository.GetEPAEndEvaluationPeriod(criteria);
            }

            return epaInfos;
        }

        public CommandResult UpdateEPAScore(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(GetMemberByEvaluatorOrReviewerOrManagementCriteria criteria)
        {
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<Employee>(criteria);
            }

            return employees;
        }

        public IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerOrManagementCriteria criteria)
        {
            IEnumerable<Group> groups = new List<Group>();

            if (criteria != null && criteria.IsValid())
            {
                groups = GetData<Group>(criteria);
            }

            return groups;
        }

        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            IEnumerable<StandardGradeModel> standardGrades = new List<StandardGradeModel>();
            var criteria = new GetStandardGradeCriteria();
            if (criteria != null && criteria.IsValid())
            {
                standardGrades = GetData<StandardGradeModel>(criteria);
            }

            return standardGrades;
        }

        public IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport(GetEPASummaryImportCriteria criteria)
        {
            IEnumerable<EPASummaryImportModel> memberEPAs = new List<EPASummaryImportModel>();

            if (criteria != null && criteria.IsValid())
            {
                memberEPAs = GetData<EPASummaryImportModel>(criteria);
            }

            return memberEPAs;
        }

        public CommandResult UpdateEPASummaryImport(UpdateEPASummaryImportCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public string ConvertScoreToGrade(float score, IEnumerable<StandardGradeModel> standardGrades)
        {
            string grade = string.Empty;

            if (standardGrades != null && standardGrades.Any())
            {
                foreach (StandardGrade gradeEnum in standardGrades.Where(x => x.LeftScore <= score && x.RightScore >= score).Select(x => (StandardGrade)x.GradeId))
                {
                    grade = Enumerations.GetDescription(gradeEnum);
                    break;
                }
            }

            return grade;
        }

        public int CheckEmployeePermission(GetEmployeePermissionCriteria criteria)
        {
            int result = -1;

            if (criteria != null && criteria.IsValid())
            {
                result = GetDataEntity<int>(criteria);
            }

            return result;
        }

        public IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria)
        {
            IEnumerable<EmployeeOtherViolationDetailModel> otherViolationDetails = new List<EmployeeOtherViolationDetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                otherViolationDetails = GetData<EmployeeOtherViolationDetailModel>(criteria);
            }

            return otherViolationDetails;
        }

        public IList<EPAModel> GetMonthlyEPAOfSemi(string epaIds)
        {
            IList<EPAModel> monthlyEPAs = new List<EPAModel>();

            var criteria = new GetMonthlyEPAOfSemiCriteria()
            {
                EPAIds = epaIds
            };

            monthlyEPAs = _employeeEPARepository.GetEPA(criteria).ToList();

            return monthlyEPAs;
        }

        public CommandResult ResetScore(ResetScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IList<EPAPeriodModel> GetPeriodToUpdateFinalScore(GetPeriodToUpdateFinalScoreCriteria criteria)
        {
            IList<EPAPeriodModel> periods = new List<EPAPeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = GetData<EPAPeriodModel>(criteria).ToList();
            }

            return periods;
        }

        public IList<EPAEmployeeModel> GetEmployeeToUpdateFinalScore(GetEmployeeToUpdateFinalScoreCriteria criteria)
        {
            IList<EPAEmployeeModel> employees = new List<EPAEmployeeModel>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<EPAEmployeeModel>(criteria).ToList();
            }

            return employees;
        }

        public IList<EPAModel> GetEPAToUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria)
        {
            IList<EPAModel> epaInfos = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epaInfos = _employeeEPARepository.GetEPAToUpdateFinalScore(criteria);
            }

            return epaInfos;
        }

        public CommandResult UpdateFinalScoreEPAEndEvaluation(UpdateFinalScoreEPAEndEvaluationCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public int GetNumberOfRow(GetNumberOfRowCriteria criteria)
        {
            var result = 0;

            if (criteria != null && criteria.IsValid())
            {
                result = GetDataEntity<int>(criteria);
            }

            return result;
        }

        public void GroupMonthlyEPABySemi(EPAModel semiEPA, IList<EPAModel> monthlyEPAs)
        {
            if (semiEPA != null && monthlyEPAs != null && monthlyEPAs.Count > 0)
            {
                foreach (EPAModel monthlyEPA in monthlyEPAs.Where(x => x.EPAParentId.Equals(semiEPA.EPAId)))
                {
                    semiEPA.MonthlyEPA.MonthlyEPAs.Add(monthlyEPA);
                }
            }
        }

        public void ConvertAverageMonthlyScore(MonthlyEPAModel monthlyEPA, ScoreRatingTypeModel scoreRatingType)
        {
            if (monthlyEPA.MonthlyEPAs.Any())
            {
                ScoreRatingTypeModel monthlyScoreRatingType = monthlyEPA.MonthlyEPAs.First().ScoreRatingType;

                float averageScore = (monthlyEPA.AverageSelfScore * scoreRatingType.SelfEvaluated) / monthlyScoreRatingType.SelfEvaluated;
                monthlyEPA.AverageSelfScoreConverted = (float)Math.Round(averageScore, 2);

                averageScore = (monthlyEPA.AverageEvaluatorScores * scoreRatingType.Evaluator) / monthlyScoreRatingType.Evaluator;
                monthlyEPA.AverageEvaluatorScoreConverted = (float)Math.Round(averageScore, 2);

                averageScore = (monthlyEPA.AverageReviewerScores * scoreRatingType.Reviewer) / monthlyScoreRatingType.Reviewer;
                monthlyEPA.AverageReviewerScoreConverted = (float)Math.Round(averageScore, 2);
            }
        }

        public IEnumerable<EvaluationModel> GetLastRating(int employeeId, int targetTypeId)
        {
            IEnumerable<EvaluationModel> result = new List<EvaluationModel>();
            var criteria = new GetLastRatingByEmployeeIdCriteria { EmployeeId = employeeId, TargetTypeId = targetTypeId };
            if (criteria != null && criteria.IsValid())
            {
                var kpis = GetData<EvaluationModel>(criteria);
                foreach (var kpi in kpis)
                {
                    kpi.Decode();
                }

                result = AveragedLastRating(kpis);
            }

            return result;
        }

        private IEnumerable<EvaluationModel> AveragedLastRating(IEnumerable<EvaluationModel> kpis)
        {
            var list = new List<EvaluationModel>();
            foreach (var kpi in kpis)
            {
                if (!list.Where(i => i.KPIId == kpi.KPIId).Any())
                {
                    var averagedRating = kpis.Where(i => i.KPIId == kpi.KPIId && !string.IsNullOrEmpty(i.RateEncode)).Average(i => i.Rate);
                    var averagedScore = kpis.Where(i => i.KPIId == kpi.KPIId && !string.IsNullOrEmpty(i.ScoreEncode)).Average(i => i.Score);

                    list.Add(new EvaluationModel { KPIId = kpi.KPIId, Rate = averagedRating, Score = averagedScore });
                }
            }

            return list;
        }

        private void SpreadKPIScore(IList<EPAKPIGroupModel> kpiGroups)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI && kpiGroup.Weight != null)
                {
                    double averageWeight = kpiGroup.Weight.Value / kpiGroup.KPIs.Count;
                    averageWeight = SplitNumber(averageWeight, Constants.NumberOfDecimals);

                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.Weight = averageWeight;
                    }
                }
            }
        }

        private double SplitNumber(double number, int numberOfDecimals)
        {
            double result = number;
            numberOfDecimals = numberOfDecimals <= 0 ? Constants.NumberOfDecimalDefault : numberOfDecimals;

            if ((number % 1) != 0)
            {
                int index = number.ToString().IndexOf('.');
                string naturalFraction = number.ToString().Substring(0, index),
                    decimalFraction = number.ToString().Substring(index + 1);

                result = double.Parse(naturalFraction + "." + decimalFraction.Substring(0, Math.Min(decimalFraction.Length, numberOfDecimals)));
            }

            return result;
        }
    }
}