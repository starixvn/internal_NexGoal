namespace NexGoal.AutoService.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NexGoal.AutoService.Criterias;
    using NexGoal.AutoService.Criterias.PA;
    using NexGoal.AutoService.Models;
    using NexGoal.AutoService.Models.PA;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class PAService : IPAService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;

        public PAService(
            IBaseService baseService,
            ICryptoService cryptoService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
        }

        public CommandResult EditGrade()
        {
            var commandResult = new CommandResult();
            IEnumerable<PARecordModel> records = GetDataToEditGrade();

            if (records != null && records.Any())
            {
                var xmlTemplate = "<Row PARecordId='{0}' GradeId='{1}' ToeicPolicy='{2}' />";
                var xml = new StringBuilder();
                xml.Append("<Root>");

                foreach (PARecordModel record in records)
                {
                    xml.AppendFormat(xmlTemplate, record.PARecordId, record.GradeId, record.ToeicPolicy);
                }

                xml.Append("</Root>");

                var criteria = new EditGradeCriteria { Value = xml.ToString() };

                commandResult = baseService.ExecuteNonQueryAsync(criteria).Result;
            }

            return commandResult;
        }

        public CommandResult EditWhenEvaluationFinished()
        {
            var criteria = new EditWhenEvaluationFinishedCriteria();

            return baseService.ExecuteNonQueryAsync(criteria).Result;
        }

        public CommandResult EditStatus()
        {
            var criteria = new EditStatusCriteria();

            return baseService.ExecuteNonQueryAsync(criteria).Result;
        }

        private IEnumerable<PARecordModel> GetDataToEditGrade()
        {
            var criteria = new GetDataToEditGradeCriteria();
            var records = baseService.GetDataAsync<PARecordModel>(criteria).Result;
            var grades = baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNameCriteria { RefNames = "PerformanceGrade" }).Result;
            foreach (PARecordModel record in records)
            {
                record.GradeId = cryptoService.Decrypt(record.GradeId);
                int.TryParse(record.GradeId, out int gradeId);
                gradeId += record.ToeicPolicy;

                if (!grades.Any(x => x.ResourceId == gradeId))
                {
                    gradeId = gradeId > grades.Max(x => x.ResourceId) ? grades.Max(x => x.ResourceId) : 0;
                }

                record.GradeId = gradeId == 0 ? null : cryptoService.Encrypt(gradeId.ToString());
            }

            return records;
        }
    }
}