namespace NexGoal.Site.Controllers.API
{
    using System.Web.Http;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.Department;
    using NexGoal.Model.Department;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web.ApiAuthorizeAttribute;

    public class DepartmentController : BaseController
    {
        private readonly ICommonService commonService;

        public DepartmentController()
            : this(null)
        {
        }

        public DepartmentController(ICommonService commonService)
        {
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Department, Policy = Policy.Read)]
        public DepartmentInfoModel GetDepartment(
            int? parentId = null,
            bool? isActive = null)
        {
            var criteria = new GetDepartmentCriteria()
            {
                ParentId = parentId,
                IsActive = isActive
            };

            return commonService.GetDepartment(criteria);
        }
    }
}