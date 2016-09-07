namespace NexGoal.Model
{
    using NexGoal.Model.Employee;

    public class EOQYNominatorSettingModel
    {
        public int ProcessId { get; set; }

        public int NominatorId { get; set; }

        public EmployeeModel NominatorInfo { get; set; }
    }
}