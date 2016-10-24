namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Model;
    using NexGoal.Model.Employee;
    using NexGoal.Repositories.Interfaces;

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
                    "FunctionId,MenuId",
                    criteria.GetSpParams());

                return menus;
            }
        }
    }
}