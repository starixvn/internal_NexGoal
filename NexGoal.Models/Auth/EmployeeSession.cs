namespace NexGoal.Models.Auth
{
    public class EmployeeSession
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public bool IsInProbation { get; set; }
        public string Password { get; set; }
    }
}