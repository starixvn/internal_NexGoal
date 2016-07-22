using Fanex.Data.Repository;
using NexGoal.Model.Employee;
using System.Collections.Generic;

namespace NexGoal.Repositories.Interfaces
{
    public interface IMenuRepository
    {
        IEnumerable<MenuViewModel> GetMenus(ICriteria criteria);
    }
}
