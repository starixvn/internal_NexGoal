namespace NexGoal.Tools
{
    public class CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = "Invalid input";
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