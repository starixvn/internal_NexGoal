namespace NexGoal.Model.FlowManagement
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;

    public class InsertFlowItemModel
    {
        public int Duration { get; set; }

        public bool HasSendMail { get; set; }

        public int RemindDay { get; set; }

        public bool IncludeSelfEvaluation { get; set; }

        public List<EmployeeModel> Employees { get; set; }
    }
}