namespace NexGoal.Model
{
    using NexGoal.Model.Employee;

    public class EOQYNominatorSettingModel
    {
        public int RoleId { get; set; }

        public EOQYModel Period { get; set; }

        public EmployeeModel NominatorInfo { get; set; }
    }
}