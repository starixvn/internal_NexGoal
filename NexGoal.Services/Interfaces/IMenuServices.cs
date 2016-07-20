namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model.Employee;
    using System.Collections.Generic;

    public interface IMenuService
    {
        IEnumerable<MenuGroupModel> GetMenusByUserId(int userId);
    }
}
