namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
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

    public interface ICommonService
    {
        IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria);

        IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria);

        IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria);

        IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(GetEmployeeByDepartmentIdCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByGroupId(GetEmployeeByGroupIdCriteria criteria);

        EmployeeModel GetEmployeeById(int employeeId);

        IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria);

        DepartmentInfoModel GetDepartment(GetDepartmentCriteria criteria);

        IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria);

        IEnumerable<StandardGradeModel> GetStandardGrade();

        IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria);

        IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria);

        IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria);
    }
}