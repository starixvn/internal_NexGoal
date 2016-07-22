using Fanex.Data;
using Fanex.Data.Repository;
using NexGoal.Core.Model;
using NexGoal.Model.Employee;
using NexGoal.Repositories.Interfaces;
using System.Collections.Generic;

namespace NexGoal.Repositories.Implement
{
    public class MenuRepository : IMenuRepository
    {
        public IEnumerable<MenuViewModel> GetMenus(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var menus = db.Query<MenuViewModel, FunctionModel, MenuModel, MenuViewModel>(
                    (m, f, mP) =>
                    {
                        m.Function = f;
                        m.ParentMenu = mP;

                        return m;
                    },
                    splitOn: "FunctionId,MenuId",
                        parameterValues: criteria.GetSpParams());

                return menus;
            }
        }
    }
}
