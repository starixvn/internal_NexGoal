namespace NexGoal.Model.Enitites
{
    using System;
    using Core.Services;

    [Serializable]
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string DomainUsername { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Avatar
        {
            get
            {
                IAvatarService service = new AvatarService();

                return service.GetAvatar(Code);
            }
        }

        public string Category { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsLocal { get; set; }

        public DateTime JoinedDate { get; set; }
    }
}