namespace NexGoal.Models.UserManagement
{
    public class UserModel
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string Group { get; set; }
        public string JobTitle { get; set; }
        public string Roles { get; set; }
        public string Base64Image { get; set; }
    }
}