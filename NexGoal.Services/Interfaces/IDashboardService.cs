namespace NexGoal.Services.Interfaces
{
    using Model.Dashboard;

    public interface IDashboardService
    {
        ToDoListModel GetToDoList(int employeeId);
    }
}