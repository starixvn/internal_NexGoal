namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Menu;
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
        /// [ApiAuthorize(FunctionKey.MenuManagement, Policy.Read)]
        public IEnumerable<MenuModel> GetMenusByEmployeeId()
        {
            var menus = this.menuService.GetMenusByUserId(CurrentUser.EmployeeId);

            return menus;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.MenuManagement, Policy.Host)]
        public CommandResult Create(InsertMenuCriteria criteria)
        {
            var result = this.menuService.Insert(criteria);

            return result;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.MenuManagement, Policy.Host)]
        public CommandResult Edit(UpdateMenuCriteria criteria)
        {
            var result = this.menuService.Update(criteria);

            return result;
        }

        [ApiAuthorize(FunctionKey.MenuManagement, Policy.Host)]
        public CommandResult Delete(int id)
        {
            var result = this.menuService.Delete(id);

            return result;
        }
    }
}