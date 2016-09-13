namespace NexGoal.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using NexGoal.Core.Constants;
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

    public class NominationService : BaseService, INominationService
    {
        private readonly INominationRepository nominationRepository;

        public NominationService()
            : this(null)
        {
        }

        public NominationService(INominationRepository nominationRepository)
        {
            this.nominationRepository = nominationRepository ?? new NominationRepository();
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
                result = nominationRepository.GetCurrentNominationByNominatorId(criteria);
            }

            return result;
        }

        public IEnumerable<NominationViewModel> GetEOQNominationByNominatorId(GetEOQNominationByNominatorIdCriteria criteria)
        {
            IEnumerable<NominationViewModel> result = new List<NominationViewModel>();

            if (criteria != null && criteria.IsValid())
            {
                IEnumerable<NominationModel> nominations = nominationRepository.GetNominationByNominatorId(criteria);

                result = nominations.GroupBy(
                    x => new { x.Period.EOQYId, x.Nominee.EmployeeId },
                    (key, g) => new NominationViewModel
                    {
                        Period = g.First().Period,
                        Nominee = g.First().Nominee,
                        Nominations = g.Select(x => new NominationDetailViewModel
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

        public IEnumerable<QuestionModel> GetEOQQuestion()
        {
            var criteria = new GetEOQQuestionCriteria();
            IEnumerable<QuestionModel> questions = this.GetData<QuestionModel>(criteria);

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

        public NominationModel GetEOQNominationById(int nominationId)
        {
            var nomination = new NominationModel();
            var criteria = new GetEOQNominationByIdCriteria
            {
                NominationId = nominationId
            };

            if (criteria.IsValid())
            {
                nomination = nominationRepository.GetNominationById(criteria);
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

        public MemoryStream ExportEOQ(IEnumerable<NominationViewModel> nominations, string periodName)
        {
            IList<string> titleHeaders = new List<string>
            {
                Lang.SymbolCount,
                Lang.Nominee,
                Lang.Nominator,
                Lang.Team,
                Lang.Status
            };

            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(periodName);

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
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));

            RenderHeaderEOQ(ref workbook, ref sheet, titleHeaders);
            RenderContentRowEOQ(ref workbook, ref sheet, nominations);
            sheet.CreateFreezePane(0, 3);

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
        private void RenderContentRowEOQ(ref HSSFWorkbook workbook, ref ISheet sheet, IEnumerable<NominationViewModel> nominations)
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
            bodyStyle3.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle bodyStyle4 = workbook.CreateCellStyle();
            bodyStyle4.Alignment = HorizontalAlignment.Left;
            bodyStyle4.VerticalAlignment = VerticalAlignment.Center;
            bodyStyle4.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle4.FillPattern = FillPattern.SolidForeground;
            bodyStyle4.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;
            ICell cellBody;

            foreach (NominationViewModel nomination in nominations)
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
                sheet.SetColumnWidth(0, 5 * Constants.ExcelColumnWidth);

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
                if (nomination.NumberOfNominators > 1)
                {
                    nominee += " - (" + nomination.NumberOfNominators + " votes)";
                }

                cellBody.SetCellValue(nominee);
                sheet.SetColumnWidth(1, 30 * Constants.ExcelColumnWidth);

                cellBody = row.CreateCell(2);
                cellBody.CellStyle = bodyStyle;
                var nominator = string.Empty;

                if (nomination.NumberOfNominators > 1)
                {
                    IEnumerable<string> nominators = nomination.Nominations.Select(x => "- " + x.Nominator.FullName + " (" + x.Nominator.NickName + ")");
                    nominator = string.Join(Environment.NewLine, nominators);
                }
                else
                {
                    nominator = nomination.Nominations.First().Nominator.FullName + " (" + nomination.Nominations.First().Nominator.NickName + ")";
                }

                cellBody.SetCellValue(nominator);
                sheet.SetColumnWidth(2, 30 * Constants.ExcelColumnWidth);

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
                cellBody.SetCellValue(nomination.Nominee.Group.GroupName);
                sheet.SetColumnWidth(3, 20 * Constants.ExcelColumnWidth);

                cellBody = row.CreateCell(4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(nomination.StatusToString);
                sheet.SetColumnWidth(4, 15 * Constants.ExcelColumnWidth);

                rowIndex++;
            }
        }

        #endregion Export Excel
    }
}