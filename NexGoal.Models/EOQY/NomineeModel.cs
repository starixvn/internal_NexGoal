namespace NexGoal.Model
{
    using NexGoal.Model.Department;
    using NexGoal.Model.Enitites;

    public class NomineeModel
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public JobTitle JobTitle { get; set; }

        public DepartmentModel Department { get; set; }

        public Group Group { get; set; }
    }
}