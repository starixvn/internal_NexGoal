namespace NexGoal.Model.PAFocus
{
    using NexGoal.Core.Services;
    using NexGoal.Model.Employee;
    using System;

    public class PAFocusModel
    {
        public int PAFocusId { get; set; }

        public EmployeeModel Employee { get; set; }

        public int PeriodId { get; set; }

        public float? FinalScore { get; set; }

        public string EncryptedFinalScore { get; set; }

        public string LastCompletedPercentObjective { get; set; }

        public string NextDescriptionObjective { get; set; }

        public string EncryptedContribution { get; set; }

        public int? Contribution { get; set; }

        public string EncryptedBonusGiven { get; set; }

        public int? BonusGiven { get; set; }

        public bool? IsReadyPromotion { get; set; }

        public string EncryptedIsReadyPromotion { get; set; }

        public bool? IsPoorPerformance { get; set; }

        public string EncryptedIsPoorPerformance { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsReadOnly
        {
            get
            {
                return DateTime.Now.Date < StartDate.Date || DateTime.Now.Date > EndDate.Date;
            }
        }

        public void Decrypt()
        {
            ICertificateService encryptService = new CertificateService();
            IsReadyPromotion = string.IsNullOrEmpty(EncryptedIsReadyPromotion) ? IsReadyPromotion : bool.Parse(encryptService.Decrypt(EncryptedIsReadyPromotion));
            IsPoorPerformance = string.IsNullOrEmpty(EncryptedIsPoorPerformance) ? IsPoorPerformance : bool.Parse(encryptService.Decrypt(EncryptedIsPoorPerformance));
            Contribution = string.IsNullOrEmpty(EncryptedContribution) ? Contribution : int.Parse(encryptService.Decrypt(EncryptedContribution));
            BonusGiven = string.IsNullOrEmpty(EncryptedBonusGiven) ? BonusGiven : int.Parse(encryptService.Decrypt(EncryptedBonusGiven));
            FinalScore = string.IsNullOrEmpty(EncryptedFinalScore) ? FinalScore : float.Parse(encryptService.Decrypt(EncryptedFinalScore));
        }

        public void Encrypt()
        {
            ICertificateService encryptService = new CertificateService();
            EncryptedIsReadyPromotion = !IsReadyPromotion.HasValue ? string.Empty : encryptService.Encrypt(IsReadyPromotion.ToString());
            EncryptedIsPoorPerformance = !IsPoorPerformance.HasValue ? string.Empty : encryptService.Encrypt(IsPoorPerformance.ToString());
            EncryptedContribution = !Contribution.HasValue ? string.Empty : encryptService.Encrypt(Contribution.ToString());
            EncryptedBonusGiven = !BonusGiven.HasValue ? string.Empty : encryptService.Encrypt(BonusGiven.ToString());
        }
    }
}
