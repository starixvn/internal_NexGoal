namespace NexGoal.Criteria.Employee
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ChangePasswordCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string NewPassword { get; set; }

        [SpParam(Ignored = true)]
        public string OldPassword { get; set; }

        [SpParam(Ignored = true)]
        public string ConfirmPassword { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
#pragma warning disable S1067 // Expressions should not be too complex
            return EmployeeId > 0
                && !string.IsNullOrWhiteSpace(NewPassword)
                && !string.IsNullOrWhiteSpace(OldPassword)
                && !string.IsNullOrWhiteSpace(ConfirmPassword)
                && NewPassword.Equals(ConfirmPassword, StringComparison.OrdinalIgnoreCase);
#pragma warning restore S1067 // Expressions should not be too complex
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_ChangePassword";
        }
    }
}