namespace NexGoal.Core.Constants
{
    public sealed class Constants
    {
        public const string CurrentSessionKey = "NexGoal.CurrentSession";

        public const string CurrentSessionUserId = "NexGoal.CurrentUserId";

        public const string DefaultPassword = "SMSep@1320$!";

        public const string ResetDefaultPassword = "123456";

        public const string KeyCryptography = "SMSEPA02072015";

        public const string EmployeeFlowSettingReport = "NexGoal.Report.EmployeeFlowSettingReport";

        public const string DepartmentFlowSettingReport = "NexGoal.Report.DepartmentFlowSettingReport";

        public const string FormatDateTimeEmail = "MMM dd, yyyy HH:mm";

        public const string DateFormat = "dd MMM, yyyy";

        public const string TimeFormat = "HH:mm";

        public const int DepartmentRoot = 1;

        public const int MaxFinalScore = 100;

        public const float MaxScoreSubtractDate = (float)3.5;

        public const int NumberEvaluatorTargetSetting = 3;

        public const string PeriodEmployeeSettingReportSessionKey = "NexGoal.PeriodEmployeeSeting";

        public const string DisciplineSummaryReportSessionKey = "NexGoal.DisciplineSummaryReport";

        public const string DisciplineDetailReportSessionKey = "NexGoal.DisciplineDetailReport";

        public const string FormatMonthlyPeriodName = "MMM, yyyy";

        public const int NumberOfDecimalDefault = 2;

        public const int NumberOfDecimals = 4;

        public const string ExportingFolder = @"~\ExportPDFFolder\";

        private Constants()
        {
        }
    }
}