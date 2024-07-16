namespace NexGoal.BackgroundJob.Criterias.Redmine
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models.Redmine;

    public class SyncProjectCriteria : CriteriaBase
    {
        private const string _ProjectTpl = "<Project Id='{0}' Name='{1}' UpdatedOn='{2}' Status='{3}'></Project>";

        [SpParam(Ignored = true)]
        public IList<Project> Projects { get; set; }

        public string ProjectXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (Projects != null && Projects.Any())
                {
                    foreach (var project in Projects)
                    {
                        value.AppendFormat(
                            _ProjectTpl,
                            project.Id,
                            project.Name,
                            project.UpdatedOn,
                            project.Status);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "Projects_Sync";

        public override bool IsValid() => Projects != null && Projects.Any();
    }
}