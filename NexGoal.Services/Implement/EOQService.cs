namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public class EOQService : BaseService, IEOQService
    {
        private readonly IEOQRepository eoqRepository;

        public EOQService()
            : this(null)
        {
        }

        public EOQService(IEOQRepository eoqRepository)
        {
            this.eoqRepository = eoqRepository ?? new EOQRepository();
        }

        public IEnumerable<EOQYModel> GetEOQ(GetEOQCriteria criteria)
        {
            IEnumerable<EOQYModel> list = new List<EOQYModel>();

            if (criteria != null && criteria.IsValid())
            {
                list = this.GetData<EOQYModel>(criteria);
            }

            return list;
        }

        public EOQYModel GetEOQById(GetEOQByIdCriteria criteria)
        {
            var result = new EOQYModel();

            if (criteria != null && criteria.IsValid())
            {
                result = this.GetDataEntity<EOQYModel>(criteria);
            }

            return result;
        }

        public IEnumerable<EOQYNominatorSettingModel> GetEOQNominatorSettingById(GetEOQNominatorSettingByIdCriteria criteria)
        {
            IEnumerable<EOQYNominatorSettingModel> nominators = new List<EOQYNominatorSettingModel>();

            if (criteria != null && criteria.IsValid())
            {
                nominators = eoqRepository.GetEOQNominatorSettingById(criteria);
            }

            return nominators;
        }

        public EOQYNominatorSettingModel GetCurrentEOQByNominatorId(int nominatorId)
        {
            var criteria = new GetCurrentEOQByNominatorIdCriteria
            {
                NominatorId = nominatorId
            };

            var result = new EOQYNominatorSettingModel();

            if (criteria.IsValid())
            {
                result = eoqRepository.GetCurrentEOQByNominatorId(criteria);
            }

            return result;
        }

        public IEnumerable<EOQYNominationViewModel> GetEOQNominationByNominatorId(GetEOQNominationByNominatorIdCriteria criteria)
        {
            IEnumerable<EOQYNominationViewModel> result = new List<EOQYNominationViewModel>();

            if (criteria != null && criteria.IsValid())
            {
                IEnumerable<EOQYNominationModel> nominations = eoqRepository.GetNominationByNominatorId(criteria);

                result = nominations.GroupBy(
                    x => new { x.Period, x.Nominee },
                    (key, g) => new EOQYNominationViewModel
                    {
                        Period = key.Period,
                        Nominee = key.Nominee,
                        Nominations = g.Select(x => new EOQYNominationDetailViewModel
                            {
                                NominationId = x.NominationId,
                                Nominator = x.Nominator
                            }).ToList(),
                        RoleId = g.First().RoleId,
                        StatusId = g.First().StatusId
                    });
            }

            return result;
        }

        public CommandResult CreateEOQ(CreateEOQCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEOQ(UpdateEOQCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEOQNominatorSettingById(UpdateEOQNominatorSettingByIdCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteEOQ(DeleteEOQCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Delete(criteria);
            }

            return commandResult;
        }

        public IEnumerable<EOQYQuestionModel> GetEOQQuestion()
        {
            var criteria = new GetEOQQuestionCriteria();
            IEnumerable<EOQYQuestionModel> questions = this.GetData<EOQYQuestionModel>(criteria);

            return questions;
        }

        public CommandResult CreateEOQNomination(CreateEOQNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Insert(criteria);
            }

            return commandResult;
        }

        public EOQYNominationModel GetEOQNominationById(int nominationId)
        {
            var nomination = new EOQYNominationModel();
            var criteria = new GetEOQNominationByIdCriteria
            {
                NominationId = nominationId
            };

            if (criteria.IsValid())
            {
                nomination = eoqRepository.GetEOQNominationById(criteria);
            }

            return nomination;
        }

        public CommandResult UpdateEOQNomination(UpdateEOQNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteEOQNomination(DeleteEOQNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Delete(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateStatusEOQNomination(UpdateStatusEOQNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        #region Export Excel

        public MemoryStream ExportEOQ(IEnumerable<EOQYNominationViewModel> nominations, string periodName)
        {
            IList<string> titleHeaders = new List<string>
            {
                Lang.SymbolCount,
                Lang.Nominee,
                Lang.Nominator,
                Lang.Quarter,
                Lang.Team,
                Lang.Status
            };

            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(Lang.Nomination);

            IRow rowTitle = sheet.CreateRow(0);

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(periodName);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));

            RenderHeaderEOQ(ref workbook, ref sheet, titleHeaders);
            RenderContentRowEOQ(ref workbook, ref sheet, nominations);

            MemoryStream output = null;
            try
            {
                output = new MemoryStream();
                workbook.Write(output);

                return output;
            }
            finally
            {
                if (output != null)
                {
                    output.Dispose();
                }
            }
        }

        private void RenderHeaderEOQ(ref HSSFWorkbook workbook, ref ISheet sheet, IList<string> titleHeaders)
        {
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);

            IRow row = sheet.CreateRow(2);
            for (int i = 0; i < titleHeaders.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(titleHeaders[i]);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void RenderContentRowEOQ(ref HSSFWorkbook workbook, ref ISheet sheet, IEnumerable<EOQYNominationViewModel> nominations)
        {
            var rowIndex = 3;

            ICellStyle bodyStyle = workbook.CreateCellStyle();

            ICellStyle bodyStyle1 = workbook.CreateCellStyle();
            bodyStyle1.Alignment = HorizontalAlignment.Center;
            bodyStyle1.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle bodyStyle2 = workbook.CreateCellStyle();
            bodyStyle2.Alignment = HorizontalAlignment.Center;
            bodyStyle2.VerticalAlignment = VerticalAlignment.Center;
            bodyStyle2.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle2.FillPattern = FillPattern.SolidForeground;
            bodyStyle2.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            ICellStyle bodyStyle3 = workbook.CreateCellStyle();
            bodyStyle3.Alignment = HorizontalAlignment.Left;

            ICellStyle bodyStyle4 = workbook.CreateCellStyle();
            bodyStyle4.Alignment = HorizontalAlignment.Left;
            bodyStyle4.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle4.FillPattern = FillPattern.SolidForeground;
            bodyStyle4.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;
            ICell cellBody;

            foreach (EOQYNominationViewModel nomination in nominations)
            {
                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                IRow row = sheet.CreateRow(rowIndex);
                cellBody = row.CreateCell(0);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(rowIndex - 2);

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle4;
                }
                else
                {
                    bodyStyle = bodyStyle3;
                }

                cellBody = row.CreateCell(1);
                cellBody.CellStyle = bodyStyle;
                string nominee = nomination.Nominee.FullName + " (" + nomination.Nominee.NickName + ")";
                if (nomination.Nominations.Count() > 1)
                {
                    nominee += " - (" + nomination.Nominations.Count() + " votes)";
                }

                cellBody.SetCellValue(nominee);

                cellBody = row.CreateCell(2);
                cellBody.CellStyle = bodyStyle;
                string nominator = nomination.Nominations.First().Nominator.FullName + " (" + nomination.Nominations.First().Nominator.FullName + ")";
                cellBody.SetCellValue(nominator);

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                cellBody = row.CreateCell(3);
                cellBody.CellStyle = bodyStyle;
                string period = nomination.Period.QuarterToString + " " + nomination.Period.Year;
                cellBody.SetCellValue(period);

                cellBody = row.CreateCell(4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(nomination.Nominee.Group.GroupName);

                cellBody = row.CreateCell(5);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(nomination.StatusToString);

                rowIndex++;
            }
        }

        #endregion Export Excel
    }
}