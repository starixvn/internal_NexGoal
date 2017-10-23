namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Model;
    using Core.Services;
    using Criteria.ScoreRatingType;
    using Fanex.Data.Repository;
    using Model.ScoreRatingType;
    using NexGoal.Criteria.Department;
    using NexGoal.Criteria.Employee;
    using NexGoal.Criteria.KPI;
    using NexGoal.Criteria.KPIGroup;
    using NexGoal.Criteria.Period;
    using NexGoal.Criteria.TargetTemplate;
    using NexGoal.Criteria.TargetType;
    using NexGoal.Model.Department;
    using NexGoal.Model.Employee;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.KPI;
    using NexGoal.Model.KPIGroup;
    using NexGoal.Model.Period;
    using NexGoal.Model.TargetTemplate;
    using NexGoal.Model.TargetType;
    using NexGoal.Services.Interfaces;
    using Repositories.Implement;
    using Repositories.Interfaces;

    public class CommonService : BaseService, ICommonService
    {
        private readonly IKPIService kpiService;
        private readonly IKPIGroupService kpiGroupService;
        private readonly IEmployeeService employeeService;
        private readonly IPeriodService periodService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ITargetTemplateService targetTemplateService;
        private readonly IDepartmentRepository departmentRepository;

        public CommonService()
            : this(null, null, null, null, null, null, null)
        {
        }

        public CommonService(
            IKPIService kpiService,
            IKPIGroupService kpiGroupService,
            IEmployeeService employeeService,
            IPeriodService periodService,
            IEmployeeEPAService employeeEPAService,
            ITargetTemplateService targetTemplateService,
            IDepartmentRepository departmentRepository)
        {
            this.kpiService = kpiService ?? new KPIService();
            this.kpiGroupService = kpiGroupService ?? new KPIGroupService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.periodService = periodService ?? new PeriodService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.targetTemplateService = targetTemplateService ?? new TargetTemplateService();
            this.departmentRepository = departmentRepository ?? new DepartmentRepository();
        }

        public CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            IEnumerable<TargetTypeModel> kpis = new List<TargetTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = GetData<TargetTypeModel>(criteria);
            }

            return kpis;
        }

        public IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria)
        {
            return kpiService.GetKPI(criteria);
        }

        public IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria)
        {
            return kpiGroupService.GetKPIGroup(criteria);
        }

        public IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            return employeeService.GetAllEmployee(criteria);
        }

        public IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(GetEmployeeByDepartmentIdCriteria criteria)
        {
            return employeeService.GetEmployeeByDepartmentId(criteria);
        }

        public IEnumerable<EmployeeModel> GetEmployeeByGroupId(GetEmployeeByGroupIdCriteria criteria)
        {
            return employeeService.GetEmployeeByGroupId(criteria);
        }

        public EmployeeModel GetEmployeeById(int employeeId)
        {
            return employeeService.GetEmployeeById(employeeId);
        }

        public IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria)
        {
            return employeeService.GetAllGroup(criteria);
        }

        public DepartmentInfoModel GetDepartment(GetDepartmentCriteria criteria)
        {
            DepartmentModel treeList = null;
            IList<DepartmentModel> departments = new List<DepartmentModel>();

            if (criteria != null && criteria.IsValid())
            {
                departments = departmentRepository.GetDepartment(criteria);
            }

            if (departments != null && departments.Any())
            {
                treeList = new DepartmentModel();
                treeList.Childrens.Add(departments.SingleOrDefault(x => x.ParentId == null));
            }

            var departmentInfo = new DepartmentInfoModel
            {
                TreeDepartment = treeList,
                Departments = departments
            };

            return departmentInfo;
        }

        public IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria)
        {
            return periodService.GetPeriod(criteria);
        }

        public IEnumerable<PeriodModel> GetReadyPeriod(GetReadyPeriodCriteria criteria)
        {
            return periodService.GetReadyPeriod(criteria);
        }

        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            return employeeEPAService.GetStandardGrade();
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            return periodService.GetAllPeriodIncludeImport(criteria);
        }

        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            return targetTemplateService.GetAllTargetTemplate(criteria);
        }

        public IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria)
        {
            IEnumerable<ScoreRatingTypeModel> scoreRatingTypes = new List<ScoreRatingTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                scoreRatingTypes = GetData<ScoreRatingTypeModel>(criteria);
            }

            return scoreRatingTypes;
        }
    }
}