namespace NexGoal.Model
{
    public class NominationDetailModel
    {
        public int NominationId { get; set; }

        public EOQYQuestionModel Question { get; set; }

        public string Comment { get; set; }
    }
}