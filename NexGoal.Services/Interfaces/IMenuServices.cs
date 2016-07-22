namespace NexGoal.Services.Interfaces
{
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Menu;
    using NexGoal.Model.Employee;
    using System.Collections.Generic;

    public interface IMenuService
    {
        IEnumerable<MenuModel> GetMenusByUserId(int userId);

        IEnumerable<MenuViewModel> GetMenus();

        CommandResult Insert(InsertMenuCriteria criteria);

        MenuModel GetMenusById(int id);

        CommandResult Update(UpdateMenuCriteria criteria);

        CommandResult Delete(int id);
    }
}
