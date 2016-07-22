namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Menu;
    using NexGoal.Model.Employee;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class MenuService : BaseService, IMenuService
    {
        public IEnumerable<MenuModel> GetMenusByUserId(int employeeId)
        {
            var criteria = new GetMenusByEmployeeIdCriteria { EmployeeId = employeeId };
            IEnumerable<MenuModel> menus = new List<MenuModel>();
            if (criteria.IsValid())
            {
                menus = GetData<MenuModel>(criteria);
            }

            return menus;
        }
    }
}
