namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model.Employee;
    using System.Collections.Generic;

    public interface IMenuService
    {
        IEnumerable<MenuItemModel> GetMenusByUserId(int userId);
    }
}
