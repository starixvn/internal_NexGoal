namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Model.Dashboard;

    public interface IDashboardRepository
    {
        ToDoListModel GetToDoList(ICriteria criteria);
    }
}