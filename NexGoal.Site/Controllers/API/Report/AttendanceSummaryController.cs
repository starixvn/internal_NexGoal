namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.AttendanceSummary;
    using NexGoal.Model.AttendanceSummary;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    public class AttendanceSummaryController : BaseController
    {
        private readonly IAttendanceSummaryService _attendanceSummaryService;
        private readonly IRoleService roleService;

        public AttendanceSummaryController()
            : this(null, null)
        {
        }

        public AttendanceSummaryController(
            IAttendanceSummaryService attendanceSummaryService,
            IRoleService roleService)
        {
            _attendanceSummaryService = attendanceSummaryService ?? new AttendanceSummaryService();
            this.roleService = roleService ?? new RoleService();
        }

        public IEnumerable<ViolationModel> GetViolationType()
        {
            var criteria = new GetViolationTypeCriteria();
            IEnumerable<ViolationModel> violations = _attendanceSummaryService.GetViolationType(criteria);
            return violations;
        }

        [HttpGet]
        public bool CheckUserPermissionIsAdmin()
        {
            IEnumerable<SystemRoleModel> roles = this.roleService.GetRolesByEmployeeId(SessionService.EmployeeId);
            return roles.Any(x => x.RoleId == (int)SystemRole.SystemAdmin);
        }
    }
}