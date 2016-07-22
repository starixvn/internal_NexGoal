namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.Employee;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class MenuController : BaseController
    {
        private readonly IMenuService menuService;

        public MenuController()
            : this(null)
        {
        }

        public MenuController(IMenuService menuService)
        {
            this.menuService = menuService ?? new MenuService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.MenuManagement, Policy.Read)]
        public IEnumerable<MenuModel> GetMenusByEmployeeId()
        {
            var menus = this.menuService.GetMenusByUserId(CurrentUser.EmployeeId);

            return menus;
        }
    }
}