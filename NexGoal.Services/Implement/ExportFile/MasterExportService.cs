namespace NexGoal.Services.Implement
{
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
            var rowIndexOfEmployee = 1;
            var groupName = string.Empty;
            var rowIndexOfGroup = this.writer.RowId;
            foreach (var employee in employeeEvaluations)
            {
                if (groupName == employee.PAFocus.Employee.Group.GroupName) rowIndexOfGroup++;
                //else 
                //{

                //}
                groupName = groupName == employee.PAFocus.Employee.Group.GroupName ? groupName : employee.PAFocus.Employee.Group.GroupName;

                FillRow(employee, groupName, ref rowIndexOfEmployee);
            }
        }

        private void FillRow(MasterReportModel employee, string groupName, ref int rowIndexOfEmployee)
        {
            int numberRowOfEmployee = GetNumberRowOfEmployee(employee);
            int rowIndexBeginMerger = this.writer.RowId;
            for (int i = 0; i < numberRowOfEmployee; i++)
            {
                object record = CreateRecord(employee, groupName, i, rowIndexOfEmployee);
                if (rowIndexOfEmployee % 2 == 1)
                {
                    this.writer.WriteRow(4, record);
                }
                else
                {
                    this.writer.WriteRow(5, record);
                }
            }

            rowIndexOfEmployee++;
            for (int i = 0; i < 13; i++)
            {
                this.writer.WriteCellRange(rowIndexBeginMerger, numberRowOfEmployee, i, 1);
            }

        }

        private object CreateRecord(MasterReportModel evaluation, string groupName, int index, int rowIndex)
        {
            var record = new
            {
                GroupName = evaluation.PAFocus.Employee.Group.GroupName,
                RowIndex = index == 0 ? rowIndex.ToString() : string.Empty,
                NickName = index == 0 ? evaluation.PAFocus.Employee.NickName : string.Empty,
                FullName = index == 0 ? evaluation.PAFocus.Employee.FullName : string.Empty,
                Jobtitle = index == 0 ? evaluation.PAFocus.Employee.JobTitle.JobTitleName : string.Empty,
                ReviewerAdjustment = index == 0 ? evaluation.ReviewerAdjustment : string.Empty,
                EvaluatorComment = index == 0 ? evaluation.EvaluatorComment : string.Empty,
                Contribution = index == 0 ? evaluation.PAFocus.Contribution.ToString() : string.Empty,
                Promotion = index == 0 ? evaluation.PAFocus.IsReadyPromotion.ToString() : string.Empty,
                BonusGiven = index == 0 ? evaluation.PAFocus.BonusGiven.ToString() : string.Empty,
                BonusRanking = index == 0 ? evaluation.PAFocus.BonusRanking.ToString() : string.Empty,
                LastObjective = index == 0 ? evaluation.PAFocus.LastCompletedPercentObjective : string.Empty,
                NextObjective = index == 0 ? evaluation.PAFocus.NextDescriptionObjective : string.Empty,
                Award = evaluation.Nominations.Count > index ? evaluation.Nominations[index].ToString : string.Empty,
                AwardStatus = evaluation.Nominations.Count > index ? evaluation.Nominations[index].StatusId.ToString() : string.Empty,
                ProjectName = evaluation.EmployeeWorkings.Count > index ? evaluation.EmployeeWorkings[index].ProjectName : string.Empty,
                ProjectScore = evaluation.EmployeeWorkings.Count > index ? evaluation.EmployeeWorkings[index].DecryptedScore : string.Empty,
                ProjectComment = evaluation.EmployeeWorkings.Count > index ? evaluation.EmployeeWorkings[index].Comment : string.Empty
            };

            return record;
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