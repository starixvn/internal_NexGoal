namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Menu;
    using NexGoal.Model.Employee;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;

    public class MenuService : BaseService, IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService()
            : this(null)
        {
        }

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository ?? new MenuRepository();
        }

        public IEnumerable<MenuModel> GetMenusByUserId(int userId)
        {
            var criteria = new GetMenusByEmployeeIdCriteria { EmployeeId = userId };
            IEnumerable<MenuModel> menus = new List<MenuModel>();
            if (criteria.IsValid())
            {
                menus = GetData<MenuModel>(criteria);
            }

            return menus;
        }

        public IEnumerable<MenuViewModel> GetMenus()
        {
            var criteria = new GetMenusCriteria();
            IEnumerable<MenuViewModel> menus = new List<MenuViewModel>();
            if (criteria.IsValid())
            {
                menus = _menuRepository.GetMenus(criteria);
            }

            return menus;
        }

        public MenuModel GetMenusById(int id)
        {
            var menu = new MenuModel();
            var criteria = new GetMenuByIdCriteria { MenuId = id };
            if (criteria.IsValid())
            {
                menu = GetDataEntity<MenuModel>(criteria);
            }

            return menu;
        }
    }
}