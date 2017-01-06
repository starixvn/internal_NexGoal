namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Menu;
    using NexGoal.Model.Employee;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;

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

        public CommandResult Insert(InsertMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public MenuModel GetMenusById(int id)
        {
            var criteria = new GetMenuByIdCriteria { MenuId = id };
            if (criteria.IsValid())
            {
                return GetDataEntity<MenuModel>(criteria);
            }

            return null;
        }

        public CommandResult Update(UpdateMenuCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteMenuCriteria { MenuId = id };
            var result = new CommandResult();
            if (criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }
    }
}
