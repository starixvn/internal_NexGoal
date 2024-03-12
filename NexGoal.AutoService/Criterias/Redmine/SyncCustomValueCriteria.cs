namespace NexGoal.AutoService.Criterias.Redmine
{
    using Fanex.Data;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using NexGoal.AutoService.Models.Redmine;
    using System.Linq;

    public class SyncCustomValueCriteria : CriteriaBase
    {
        private const string _CustomValueTpl = "<CustomValue Id='{0}' CustomizedType='{1}' CustomizedId='{2}' CustomFieldId='{3}' Value='{4}'></CustomValue>";

        [SpParam(Ignored = true)]
        public IList<CustomValue> CustomValues { get; set; }

        public string CustomValueXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (CustomValues != null && CustomValues.Any())
                {
                    foreach (var customValue in CustomValues)
                    {
                        value.AppendFormat(
                            _CustomValueTpl,
                            customValue.Id,
                            customValue.CustomizedType,
                            customValue.CustomizedId,
                            customValue.CustomFieldId,
                            customValue.Value);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey() => "CustomValues_Sync";

        public override bool IsValid() => CustomValues != null && CustomValues.Any();
    }
}