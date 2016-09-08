namespace NexGoal.Model
{
    using NexGoal.Model.Employee;

    public class EOQYNominatorSettingModel
    {
        public int RoleId { get; set; }

        public int NominatorId { get; set; }

        public EmployeeModel NominatorInfo { get; set; }
    }
}