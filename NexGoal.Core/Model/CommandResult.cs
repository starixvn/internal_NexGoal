namespace NexGoal.Core.Model
{
    using NexGoal.Resources;

    public class CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = Lang.InvalidInputData;
        }

        public int Id { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful
        {
            get
            {
                return Code == 0;
            }
        }
    }
}