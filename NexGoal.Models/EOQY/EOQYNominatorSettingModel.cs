namespace NexGoal.Model
{
    public class EOQYNominatorSettingModel
    {
        public int RoleId { get; set; }

        public EOQYModel Period { get; set; }

        public NomineeModel NominatorInfo { get; set; }
    }
}