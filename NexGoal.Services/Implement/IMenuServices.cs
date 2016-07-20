namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Menu;
    using NexGoal.Model.Employee;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;

    public class MenuService : BaseService, IMenuService
    {
        public IEnumerable<MenuGroupModel> GetMenusByUserId(int employeeId)
        {
            var criteria = new GetMenusByEmployeeIdCriteria { EmployeeId = employeeId };
            IEnumerable<MenuGroupModel> menus = new List<MenuGroupModel>();
            if (criteria.IsValid())
            {
                menus = GetData<MenuGroupModel>(criteria);
            }

            return menus;
        }
    }
}
