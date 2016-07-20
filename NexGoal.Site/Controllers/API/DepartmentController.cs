namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.Department;
    using NexGoal.Model.Department;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Web.Http;

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

        /// [ApiAuthorize(EPAFunctionKey.Department, Policy.Read)]
        [HttpGet]
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