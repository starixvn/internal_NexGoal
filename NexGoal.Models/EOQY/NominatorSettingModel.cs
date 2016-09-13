namespace NexGoal.Model
{
    public class NominatorSettingModel
    {
        public int RoleId { get; set; }

        public EOQYModel Period { get; set; }

        public NomineeModel NominatorInfo { get; set; }
    }
}