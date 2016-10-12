namespace NexGoal.Model.FlowManagement
{
    using NexGoal.Model.Employee;
    using System.Collections.Generic;

    public class InsertFlowItemModel
    {
        public int Duration { get; set; }

        public bool HasSendMail { get; set; }

        public int RemindDay { get; set; }

        public bool IncludeSelfEvaluation { get; set; }

        public List<EmployeeModel> Employees { get; set; }
    }
}