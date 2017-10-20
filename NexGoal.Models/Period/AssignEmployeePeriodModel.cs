namespace NexGoal.Model.Period
{
    using System.Collections.Generic;
    using System.Text;
    using Core.Constants;
    using Department;

    public class AssignEmployeePeriodModel
    {
        private const string DepartmentXmlTemplate = "<Department DepartmentId='{0}' IsGroup='{1}'></Department>";

        public int PeriodId { get; set; }

        public IEnumerable<DepartmentModel> Departments { get; set; }

        public int? EmployeeId { get; set; }

        public string ConvertToXml()
        {
            var departmentXml = new StringBuilder(Constants.BeginRootTag);

            if (Departments != null)
            {
                foreach (DepartmentModel department in Departments)
                {
                    departmentXml.AppendFormat(DepartmentXmlTemplate, department.DepartmentId, department.IsGroup ? 1 : 0);
                }
            }

            departmentXml.Append(Constants.EndRootTag);
            return departmentXml.ToString();
        }
    }
}