namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.Employee;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web.ApiAuthorizeAttribute;
    using System.Collections.Generic;

    public class MenuController : BaseController
    {
        private IMenuService menuService;

        public MenuController()
            : this(null)
        {
        }

        public MenuController(IMenuService menuService)
        {
            this.menuService = menuService ?? new MenuService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MenuManagement, Policy = Policy.Read)]
        public IEnumerable<MenuGroupModel> GetMenusByEmployeeId()
        {
            IEnumerable<MenuGroupModel> menus = this.menuService.GetMenusByUserId(CurrentUser.EmployeeInfo.EmployeeId);

            return menus;
        }
    }
}