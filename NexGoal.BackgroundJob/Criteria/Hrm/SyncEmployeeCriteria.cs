namespace NexGoal.BackgroundJob.Criteria.Hrm
{
    using Fanex.Data;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Criteria;
    using System.Text;
    using System.Linq;

    public class SyncEmployeeCriteria : CommandBaseCriteria
    {
        private const string _EmployeeTpl = "<Employee EmployeeId='{0}' DomainUsername='{1}' "
        + "Code='{2}' FullName='{3}' NickName='{4}' GroupId='{5}' Enabled='{6}' JoinedDate='{7}' "
        + "ConfirmedDate='{8}' JobTitleId='{9}' Email='{10}' ConversationId='{11}' IsDeleted='{12}' Password='{13}' ModifiedDate='{14}'></Employee>";

        [SpParam(Ignored = true)]
        public IList<Employee> Employees { get; set; }

        public string EmployeeXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");
                if (Employees != null)
                {
                    foreach (var emp in Employees)
                    {
                        value.AppendFormat(
                            _EmployeeTpl,
                            emp.EmployeeId,
                            emp.DomainUsername,
                            emp.Code,
                            emp.FullName,
                            emp.NickName,
                            emp.GroupId,
                            emp.Enabled,
                            emp.JoinedDate,
                            emp.ConfirmedDate,
                            emp.JobTitleId,
                            emp.Email,
                            emp.ConversationId,
                            emp.IsDeleted,
                            emp.Password,
                            emp.ModifiedDate);
                    }
                }
                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "SyncEmployee_Import";

        public override bool IsValid() => Employees != null && Employees.Any();
    }
}