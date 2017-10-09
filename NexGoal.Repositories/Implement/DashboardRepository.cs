namespace NexGoal.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Interfaces;
    using Model.Dashboard;

    public class DashboardRepository : IDashboardRepository
    {
        public ToDoListModel GetToDoList(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var result = new ToDoListModel();

                    result.Nominations = multi.Read<NominationModel>();
                    result.PAFocuses = multi.Read<PAFocusModel>();

                    return result;
                }
            }
        }
    }
}