namespace NexGoal.Model.EmployeeEPA
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;

    public class EPAStatusModel
    {
        public int StatusId { get; set; }

        public string Name
        {
            get
            {
                return Enumerations.GetDescription((PAStatus)StatusId);
            }
        }
    }
}