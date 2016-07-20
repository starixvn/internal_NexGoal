namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.AttendanceSummary;
    using NexGoal.Model.AttendanceSummary;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web;
    using NexGoal.Web.ApiAuthorizeAttribute;
    using NexGoal.Web.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    public class AttendanceSummaryController : BaseController
    {
        private readonly IAttendanceSummaryService _attendanceSummaryService;
        private readonly IWebCommonService _webCommonService;
        private readonly IRoleService roleService;

        public AttendanceSummaryController()
            : this(null, null, null)
        {
        }

        public AttendanceSummaryController(
            IAttendanceSummaryService attendanceSummaryService,
            IWebCommonService webCommonService,
            IRoleService roleService)
        {
            _attendanceSummaryService = attendanceSummaryService ?? new AttendanceSummaryService();
            _webCommonService = webCommonService ?? new WebCommonService();
            this.roleService = roleService ?? new RoleService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(
            DateTime startDate,
            DateTime endDate,
            bool showAll = false,
            int? departmentId = null,
            bool? isLocal = null,
            int? employeeId = null,
            int? reasonId = null)
        {
            var criteria = new GetAttendanceSummaryCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                EmployeeId = employeeId,
                ReasonId = reasonId,
                ShowAll = showAll,
                ApproverId = SessionManager.CurrentUserId
            };
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = _attendanceSummaryService.GetAttendanceSummary(criteria);

            return attendanceEmployees;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<OtherViolationModel> GetOtherViolations(
            string employeeIds,
            DateTime startDate,
            DateTime endDate,
            DateTime? workDate = null)
        {
            var criteria = new GetOtherViolationsOfEmployeeCriteria
            {
                EmployeeIds = employeeIds,
                WorkDate = workDate,
                StartDate = startDate,
                EndDate = endDate
            };

            IEnumerable<OtherViolationModel> violations = _attendanceSummaryService.GetOtherViolations(criteria);
            return violations;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Host)]
        public CommandResult InsertOtherViolation(InsertOtherViolationsCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _attendanceSummaryService.InsertOtherViolation(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Host)]
        public CommandResult DeleteOtherViolation(
            int employeeId,
            DateTime workDate,
            string name)
        {
            var criteria = new DeleteOtherViolationCriteria
            {
                EmployeeId = employeeId,
                WorkDate = workDate,
                Name = name,
                ModifiedBy = SessionManager.CurrentUserId
            };

            CommandResult commandResult = _attendanceSummaryService.DeleteOtherViolation(criteria);
            return commandResult;
        }

        public IEnumerable<ViolationModel> GetViolationType()
        {
            var criteria = new GetViolationTypeCriteria();
            IEnumerable<ViolationModel> violations = _attendanceSummaryService.GetViolationType(criteria);
            return violations;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<AttendanceDetailModel> GetAttendanceDetail(
            DateTime startDate,
            DateTime endDate,
            bool showAll,
            int? employeeId = null,
            int? departmentId = null,
            bool? isLocal = null,
            int? reasonId = null,
            bool? hasIgnored = null)
        {
            var criteria = new GetDetailAttendanceCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                EmployeeId = employeeId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                ReasonId = reasonId,
                ApproverId = SessionManager.CurrentUserId,
                HasIgnored = hasIgnored,
                ShowAll = showAll
            };

            IEnumerable<AttendanceDetailModel> attendanceViolations = _attendanceSummaryService.GetAttendanceDetail(criteria);

            return attendanceViolations;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Host)]
        public CommandResult UpdateIgnoredViolation(IEnumerable<IgnoredViolationModel> violations)
        {
            var criteria = new UpdateIgnoredViolationCriteria
            {
                ModifiedBy = SessionManager.CurrentUserId
            };

            criteria.IgnoredViolations = criteria.ConvertViolationItemToXml(violations);
            CommandResult commandResult = _attendanceSummaryService.UpdateIgnoredViolation(criteria);
            return commandResult;
        }

        [HttpGet]
        public IList<OtherViolationTypeModel> GetOtherViolationType(bool? isActive = null)
        {
            var criteria = new GetOtherViolationTypeCriteria
            {
                IsActive = isActive
            };

            IList<OtherViolationTypeModel> violationTypes = _attendanceSummaryService.GetOtherViolationType(criteria);
            return violationTypes;
        }

        public int GetUserPermissionOnAttendanceSummary()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.DisciplineSummary);
            return permission;
        }

        [HttpGet]
        public bool CheckUserPermissionIsAdmin()
        {
            IEnumerable<SystemRoleModel> roles = this.roleService.GetRolesByEmployeeId(SessionManager.CurrentUserId);
            return roles.Any(x => x.RoleId == (int)SystemRole.SystemAdmin);
        }
    }
}