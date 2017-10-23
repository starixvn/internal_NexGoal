namespace NexGoal.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Menu;
    using NexGoal.Model.Employee;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class MenuController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly ICommonService commonService;

        public MenuController()
            : this(null, null)
        {
        }

        public MenuController(IMenuService menuService, ICommonService commonService)
        {
            this.menuService = menuService ?? new MenuService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        public IEnumerable<MenuModel> GetMenusByEmployeeId()
        {
            var menus = this.menuService.GetMenusByUserId(SessionService.EmployeeId);

            return menus;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = FunctionKey.MenuManagement, Policy = Policy.Host)]
        public CommandResult Create(InsertMenuCriteria criteria)
        {
            var result = commonService.ExecuteNonQuery(criteria);

            return result;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = FunctionKey.MenuManagement, Policy = Policy.Host)]
        public CommandResult Edit(UpdateMenuCriteria criteria)
        {
            var result = commonService.ExecuteNonQuery(criteria);

            return result;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.MenuManagement, Policy = Policy.Host)]
        public CommandResult Delete(int id)
        {
            var criteria = new DeleteMenuCriteria
            {
                MenuId = id
            };
            var result = commonService.ExecuteNonQuery(criteria);

            return result;
        }
    }
}