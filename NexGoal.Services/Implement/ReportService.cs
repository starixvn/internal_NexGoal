namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Utilities;
    using NexGoal.Criteria.Report;
    using NexGoal.Model.Report;
    using NexGoal.Model.Report.Dashboard;
    using NexGoal.Model.Report.EPADetail;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;
    using NPOI.SS.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class ReportService : IReportService
    {
        private const int ExcelColumnWidth = 256;
        private readonly IReportRepository reportRepository;
        private readonly IEmployeeEPARepository employeeEPARepository;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IIntegrationService integrationService;

        public ReportService()
            : this(null, null, null, null)
        {
        }

        public ReportService(
            IReportRepository reportRepository,
            IEmployeeEPARepository employeeEPARepository,
            IEmployeeEPAService employeeEPAService,
            IIntegrationService integrationService)
        {
            this.reportRepository = reportRepository ?? new ReportRepository();
            this.employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.integrationService = integrationService ?? new IntegrationService();
        }

        public IEnumerable<MasterReportModel> GetMaster(GetMasterReportCriteria criteria)
        {
            IEnumerable<MasterReportModel> result = new List<MasterReportModel>();
            if (criteria.IsValid())
            {
                result = this.reportRepository.GetMaster(criteria);

                var nexPMs = this.integrationService.GetEmployeeWorking(criteria.EmployeeId, criteria.PeriodId);
                foreach (var employeeFocus in result)
                {
                    var nexPMsOfEmployee = nexPMs.Where(i => i.EmployeeId == employeeFocus.PAFocus.Employee.EmployeeId);
                    employeeFocus.EmployeeWorkings.AddRange(nexPMsOfEmployee);
                    employeeFocus.Decrypt();
                }
            }

            return result;
        }

        public DashboardModel GetDashboard(GetDashboardCriteria criteria)
        {
            DashboardModel result = new DashboardModel();

            if (criteria != null && criteria.IsValid())
            {
                result = employeeEPARepository.GetDashboard(criteria);
            }

            return result;
        }

        public IEnumerable<EPADetailModel> GetEPADetailReport(GetPAReportCriteria criteria)
        {
            IEnumerable<EPADetailModel> result = new List<EPADetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                result = reportRepository.GetEPADetail(criteria);
            }

            return result;
        }

        private void FillContentRowEPADetailReport(
            IEnumerable<EPADetailModel> epaDetails,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (EPADetailModel memberEPA in epaDetails)
            {
                FillRowEPADetailReport(memberEPA, ref writer, ref rowIndex);
            }
        }

        private void FillRowEPADetailReport(
            EPADetailModel memberEPA,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordEPADetailReport(memberEPA, ref rowIndex);

            if ((rowIndex - 1) % 2 == 0)
            {
                writer.WriteRow(4, record);
            }
            else
            {
                writer.WriteRow(3, record);
            }
        }

        private object CreateRecordEPADetailReport(
            EPADetailModel memberEPA,
            ref int rowIndex)
        {
            dynamic score = CalculateScore(memberEPA);

            var record = new
            {
                Count = rowIndex,
                Code = memberEPA.Employee.Code,
                FullName = memberEPA.Employee.FullName,
                NickName = memberEPA.Employee.NickName,
                Category = memberEPA.Employee.Category,
                Section = memberEPA.Employee.GroupName,
                JobTitle = memberEPA.Employee.JobTitleName,
                JoinedDate = memberEPA.Employee.JoinedDate.ToString(Constants.DateFormat),
                TargetType = memberEPA.Period.TargetTypeName,
                SelfScore = score.SelfScore,
                EvaluatorScore = score.EvaluatorScore,
                ReviewerScore = score.ReviewerScore,
                ManagementScore = score.ManagementScore.ToString(),
                DisciplineScore = score.DisciplineScore,
                FinalScore = score.FinalScore,
                Grade = score.Grade
            };
            rowIndex++;

            return record;
        }

        private string GetScoreByPhaseSelected(
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            string score,
            int phaseId)
        {
            var result = "-";

            if (phaseSelecteds.Count(x => x.PhaseId == phaseId).Equals(1))
            {
                result = score;
            }

            return result;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private dynamic CalculateScore(EPADetailModel memberEPA)
        {
            string disciplineScore = string.Empty,
                selfScore = string.Empty,
                evaluatorScore = string.Empty,
                reviewerScore = string.Empty,
                managementScore = string.Empty,
                finalScore = string.Empty,
                grade = string.Empty;

            if (memberEPA.EPAId > 0)
            {
                float score = memberEPA.Process.Self.TotalScore.Length > 0 ? float.Parse(memberEPA.Process.Self.TotalScore) : 0;

                selfScore = Math.Round(score, 2).ToString();
            }

            if (memberEPA.EPAId > 0 && memberEPA.Process.Evaluators != null && memberEPA.Process.Evaluators.Any())
            {
                evaluatorScore = Math.Round(memberEPA.Process.AverageScoreByEvaluators, 2).ToString();
            }

            if (memberEPA.EPAId > 0 && memberEPA.Process.Reviewers != null && memberEPA.Process.Reviewers.Any())
            {
                reviewerScore = Math.Round(memberEPA.Process.AverageScoreByReviewers, 2).ToString();
            }

            if (memberEPA.EPAId > 0 && memberEPA.Process.Managements != null && memberEPA.Process.Managements.Any())
            {
                managementScore = Math.Round(memberEPA.Process.AverageScoreByManagements, 2).ToString();
            }

            if (memberEPA.Period != null && memberEPA.Period.IsEndPeriod)
            {
                if (string.IsNullOrEmpty(memberEPA.FinalScore))
                {
                    finalScore = Lang.NotAvailable;
                    grade = Lang.NotAvailable;
                }
                else
                {
                    finalScore = Math.Round(float.Parse(memberEPA.FinalScore), 2).ToString();
                    grade = memberEPA.Grade;
                }
            }
            else
            {
                finalScore = Lang.Pending;
                grade = Lang.Pending;
            }

            return new
            {
                SelfScore = selfScore,
                EvaluatorScore = evaluatorScore,
                ReviewerScore = reviewerScore,
                ManagementScore = managementScore,
                DisciplineScore = disciplineScore,
                FinalScore = finalScore,
                Grade = grade
            };
        }

        private void SetColumnWidth(ref ISheet sheet, int columnIndex, int width)
        {
            sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }
    }
}