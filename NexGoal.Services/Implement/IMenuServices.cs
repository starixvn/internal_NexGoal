namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Menu;
    using NexGoal.Model.Employee;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;

    public class MenuService : BaseService, IMenuService
    {
        public IEnumerable<MenuItemModel> GetMenusByUserId(int employeeId)
        {
            var criteria = new GetMenusByEmployeeIdCriteria { EmployeeId = employeeId };
            IEnumerable<MenuItemModel> menus = new List<MenuItemModel>();
            if (criteria.IsValid())
            {
                menus = GetData<MenuItemModel>(criteria);
            }

            return menus;
        }
    }
}
