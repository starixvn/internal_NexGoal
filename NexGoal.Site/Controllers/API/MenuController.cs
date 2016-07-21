namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.Employee;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
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

        [ApiAuthorize(EPAFunctionKey.MenuManagement, Policy.Read)]
        public IEnumerable<MenuItemModel> GetMenusByEmployeeId()
        {
            var menus = this.menuService.GetMenusByUserId(CurrentUser.EmployeeId);

            return menus;
        }
    }
}