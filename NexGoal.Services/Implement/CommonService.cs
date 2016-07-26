namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Criteria.Department;
    using NexGoal.Criteria.Employee;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Criteria.EPAHistory;
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

    public class CommonService : ICommonService
    {
        private readonly ITargetTypeService targetTypeService;
        private readonly IKPIService kpiService;
        private readonly IKPIGroupService kpiGroupService;
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;
        private readonly IPeriodService periodService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IEPAHistoryService epaHistoryService;
        private readonly ITargetTemplateService targetTemplateService;

        public CommonService()
            : this(null, null, null, null, null, null, null, null, null)
        {
        }

        public CommonService(
            ITargetTypeService targetTypeService,
            IKPIService kpiService,
            IKPIGroupService kpiGroupService,
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IPeriodService periodService,
            IEmployeeEPAService employeeEPAService,
            IEPAHistoryService epaHistoryService,
            ITargetTemplateService targetTemplateService)
        {
            this.targetTypeService = targetTypeService ?? new TargetTypeService();
            this.kpiService = kpiService ?? new KPIService();
            this.kpiGroupService = kpiGroupService ?? new KPIGroupService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.departmentService = departmentService ?? new DepartmentService();
            this.periodService = periodService ?? new PeriodService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.epaHistoryService = epaHistoryService ?? new EPAHistoryService();
            this.targetTemplateService = targetTemplateService ?? new TargetTemplateService();
        }

        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            return targetTypeService.GetTargetType(criteria);
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

            IList<DepartmentModel> departments = departmentService.GetDepartment(criteria);
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

        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            return employeeEPAService.GetStandardGrade();
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            return epaHistoryService.GetAllPeriodIncludeImport(criteria);
        }

        public IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria)
        {
            return employeeEPAService.GetOtherViolationDetailByEPAId(criteria);
        }

        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            return targetTemplateService.GetAllTargetTemplate(criteria);
        }
    }
}