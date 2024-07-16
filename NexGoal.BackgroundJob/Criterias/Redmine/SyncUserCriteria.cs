namespace NexGoal.BackgroundJob.Criterias.Redmine
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models.Redmine;

    public class SyncUserCriteria : CriteriaBase
    {
        private const string _UserTpl = "<User Id='{0}' Login='{1}' UpdatedOn='{2}'></User>";

        [SpParam(Ignored = true)]
        public IList<User> Users { get; set; }

        public string UserXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (Users != null && Users.Any())
                {
                    foreach (var user in Users)
                    {
                        value.AppendFormat(
                            _UserTpl,
                            user.Id,
                            user.Login,
                            user.UpdatedOn);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "Users_Sync";

        public override bool IsValid() => Users != null && Users.Any();
    }
}