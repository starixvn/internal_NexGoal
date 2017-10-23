namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;

    public interface IMenuService
    {
        IEnumerable<MenuModel> GetMenusByUserId(int userId);

        IEnumerable<MenuViewModel> GetMenus();

        MenuModel GetMenusById(int id);
    }
}