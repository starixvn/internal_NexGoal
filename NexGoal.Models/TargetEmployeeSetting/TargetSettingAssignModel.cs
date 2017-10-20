namespace NexGoal.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using System.Text;
    using Core.Constants;
    using Department;

    public class TargetSettingAssignModel
    {
        private const string DepartmentXmlTemplate = "<Department DepartmentId='{0}' IsGroup='{1}'></Department>";

        public int TargetTemplateId { get; set; }

        public IEnumerable<DepartmentModel> DepartmentSettings { get; set; }

        public string ConvertToXml()
        {
            var departmentXml = new StringBuilder(Constants.BeginRootTag);

            if (DepartmentSettings != null)
            {
                foreach (DepartmentModel department in DepartmentSettings)
                {
                    departmentXml.AppendFormat(DepartmentXmlTemplate, department.DepartmentId, department.IsGroup ? 1 : 0);
                }
            }

            departmentXml.Append(Constants.EndRootTag);
            return departmentXml.ToString();
        }
    }
}