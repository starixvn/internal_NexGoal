namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Core.Utilities;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Criteria.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.TargetTemplate;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;

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
                commandResult = Repository.ExecuteNonQuery(criteria);
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
            if (criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
                SpreadKPIScore(epaInfo.KPIGroups);
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

        public IEnumerable<Employee> GetMemberByEvaluatorOrReviewer(GetMemberByEvaluatorOrReviewerCriteria criteria)
        {
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<Employee>(criteria);
            }

            return employees;
        }

        public IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerCriteria criteria)
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
            if (criteria.IsValid())
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

        public string ConvertScoreToGrade(double score, IEnumerable<StandardGradeModel> standardGrades)
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

        public int CheckEmployeePermission(int epaId, int actorUserId, int? processId = null)
        {
            var criteria = new GetEmployeePermissionCriteria
            {
                ActorUserId = actorUserId,
                EPAId = epaId,
                ProcessId = processId
            };
            int result = -1;

            if (criteria.IsValid())
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

        public IEnumerable<EPAKPIGroupModel> GetLastRating(int employeeId, int targetTypeId, int epaId)
        {
            IEnumerable<EPAKPIGroupModel> groups = new List<EPAKPIGroupModel>();
            var criteria = new GetLastRatingByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                TargetTypeId = targetTypeId,
                EpaId = epaId
            };

            if (criteria.IsValid())
            {
                groups = _employeeEPARepository.GetLastRating(criteria);
                foreach (var group in groups)
                {
                    group.Decrypt();
                }

                SpreadKPIScore(groups.ToList());
            }

            return groups;
        }

        private void SpreadKPIScore(IList<EPAKPIGroupModel> kpiGroups)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI && kpiGroup.Weight != null)
                {
                    double averageWeight = kpiGroup.Weight.Value / kpiGroup.KPIs.Count;

                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.Weight = averageWeight;
                    }
                }
            }
        }
    }
}