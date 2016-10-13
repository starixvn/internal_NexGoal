namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;
    using NexGoal.Model.Report;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;

    public class MasterExportService : IMasterExportService
    {
        private ExcelTemplateWriter writer;

        public void Export(IEnumerable<MasterReportModel> evaluations, string templatePath, string periodName)
        {
            var reportName = string.Format("{0}_{1}_{2}", Lang.MasterReportFileName, periodName, DateTime.Now.ToString("yyyyMMddhhmmss"));
            var reportTitle = string.Format("{0} - {1}", Lang.MasterReportTitleFile, periodName);
            this.writer = new ExcelTemplateWriter(templatePath, reportName);

            // Fill report title.
            this.writer.WriteRow(0, new object[] { reportTitle });
            this.writer.CopyRows(1, 1);

            // Fill report header.
            this.writer.CopyRows(2, 2);
            FillContentRow(evaluations);
            this.writer.SyncColumnsWidth(0, 18);

            this.writer.Render();
        }

        private void FillContentRow(IEnumerable<MasterReportModel> employeeEvaluations)
        {
            var indexRow = 1;
            var indexOnGroup = 1;
            foreach (var employee in employeeEvaluations)
            {
                FillRow(employee, indexOnGroup++, ref indexRow);
            }
        }

        private void FillRow(MasterReportModel employee, int indexOnGroup, ref int indexRow)
        {
            int numberRowOfEmployee = GetNumberRowOfEmployee(employee);
            int rowIndexBeginMerger = this.writer.RowId;
            for (int i = 0; i < numberRowOfEmployee; i++)
            {
                object record = CreateRecord(employee, i, indexOnGroup);
                if (indexRow % 2 == 1)
                {
                    this.writer.WriteRow(4, record);
                }
                else
                {
                    this.writer.WriteRow(5, record);
                }
            }

            indexRow++;
            for (int i = 0; i < 14; i++)
            {
                this.writer.WriteCellRange(rowIndexBeginMerger, numberRowOfEmployee, i, 1);
            }
        }

        private object CreateRecord(MasterReportModel evaluation, int index, int rowIndex)
        {
            var bonusGiven = evaluation.PAFocus.BonusGiven.HasValue ? Enumerations.GetDescription((PAFocusType)evaluation.PAFocus.BonusGiven.Value) : Lang.Dash;
            var contribution = evaluation.PAFocus.Contribution.HasValue ? Enumerations.GetDescription((PAFocusType)evaluation.PAFocus.Contribution.Value) : Lang.Dash;
            var promotion = evaluation.PAFocus.IsReadyPromotion.HasValue ? evaluation.PAFocus.IsReadyPromotion.Value ? Lang.Yes : Lang.No : Lang.Dash;
            var performance = evaluation.PAFocus.IsPoorPerformance.HasValue ? evaluation.PAFocus.IsPoorPerformance.Value ? Lang.Yes : Lang.No : Lang.Dash;
            var lastObjective = evaluation.PAFocus.LastCompletedPercentObjective;
            var score = evaluation.PAFocus.FinalScore.HasValue && evaluation.PAFocus.FinalScore.Value != 0 ? evaluation.PAFocus.FinalScore.ToString() : Lang.Dash;
            var comment = FormatEvaluatorComment(evaluation.EvaluatorComments);
            var record = new
            {
                RowIndex = rowIndex.ToString(),
                NickName = evaluation.PAFocus.Employee.NickName,
                FullName = evaluation.PAFocus.Employee.FullName,
                Jobtitle = evaluation.PAFocus.Employee.JobTitle.JobTitleName,
                GroupName = evaluation.PAFocus.Employee.Group.GroupName,
                ReviewerAdjustment = index == 0 ? evaluation.ReviewerAdjustment : Lang.Dash,
                Score = index == 0 ? score : Lang.Dash,
                EvaluatorComment = index == 0 ? comment : Lang.Dash,
                Contribution = index == 0 ? contribution : Lang.Dash,
                BonusGiven = index == 0 ? bonusGiven : Lang.Dash,
                Promotion = index == 0 ? promotion : Lang.Dash,
                Performance = index == 0 ? performance : Lang.Dash,
                LastObjective = index == 0 ? lastObjective : Lang.Dash,
                NextObjective = index == 0 ? evaluation.PAFocus.NextDescriptionObjective : Lang.Dash,
                Award = evaluation.Nominations.Count > index ? evaluation.Nominations[index].Title : Lang.Dash,
                AwardStatus = evaluation.Nominations.Count > index ? evaluation.Nominations[index].StatusToString : Lang.Dash,
                ProjectName = evaluation.EmployeeWorkings.Count > index ? evaluation.EmployeeWorkings[index].ProjectName : Lang.Dash,
                ProjectScore = evaluation.EmployeeWorkings.Count > index ? evaluation.EmployeeWorkings[index].DecryptedScore : Lang.Dash,
                ProjectComment = evaluation.EmployeeWorkings.Count > index ? evaluation.EmployeeWorkings[index].Comment : Lang.Dash
            };

            return record;
        }

        private string FormatEvaluatorComment(IEnumerable<EvaluatorCommentReportModel> comments)
        {
            var result = string.Empty;
            foreach (var evaluator in comments)
            {
                var comment = string.Format("{0}: {1} ({2}) \n {3} \n \n", Lang.Evaluator, evaluator.EvaluatorFullName, evaluator.EvaluatorNickName, evaluator.Comment);
                result += comment;
            }

            return result;
        }

        private int GetNumberRowOfEmployee(MasterReportModel evaluation)
        {
            int count = evaluation.Nominations.Count;
            if (evaluation.EmployeeWorkings.Count > count)
            {
                count = evaluation.EmployeeWorkings.Count;
            }

            return count > 1 ? count : 1;
        }
    }
}