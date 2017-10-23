namespace NexGoal.Model.EmployeeEPA
{
    using System;
    using Core.Services;
    using Newtonsoft.Json;

    [Serializable]
    public class EPAProcessModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int StatusEndProcess { get; set; }

        public bool IsEndProcess { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

        public string Avatar
        {
            get
            {
                IAvatarService service = new AvatarService();

                return service.GetAvatar(Code);
            }
        }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int Priority { get; set; }

        public string Comment { get; set; }

        public double MaxTotalScore { get; set; }

        [JsonIgnore]
        public string TotalScoreEncode { get; set; }

        public string TotalScore { get; set; }

        public string TotalScoreOriginal => TotalScore;

        [JsonIgnore]
        public string AdjustScoreEncode { get; set; }

        public string AdjustScore { get; set; }

        public string AdjustScoreOriginal => AdjustScore;

        public bool Evaluated { get; set; }

        public DateTime? EvaluateDate { get; set; }

        public double MaxPerformanceScore { get; set; }

        public double MaxSubtractScore { get; set; }

        public double MaxAdditionScore { get; set; }

        public void Decrypt()
        {
            ICertificateService certificateService = new CertificateService();

            TotalScore = certificateService.Decrypt(TotalScoreEncode);
            AdjustScore = certificateService.Decrypt(AdjustScoreEncode);
        }
    }
}