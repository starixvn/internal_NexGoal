using NexGoal.Core.Services;
using NexGoal.Model.Integration;
using NexGoal.Model.PAFocus;
using System.Collections.Generic;

namespace NexGoal.Model.Report
{
    public class MasterReportModel
    {
        public MasterReportModel()
        {
            Nominations = new List<NominationReportModel>();

            EmployeeWorkings = new List<EmployeeWorkingViewModel>();

            EvaluatorComments = new List<EvaluatorCommentReportModel>();
        }

        public string ReviewerAdjustment { get; set; }

        public List<EvaluatorCommentReportModel> EvaluatorComments { get; set; }

        public PAFocusModel PAFocus { get; set; }

        public List<NominationReportModel> Nominations { get; set; }

        public List<EmployeeWorkingViewModel> EmployeeWorkings { get; set; }

        public void Decrypt()
        {
            PAFocus.Decrypt();
            ICertificateService encryptService = new CertificateService();
            ReviewerAdjustment = string.IsNullOrEmpty(ReviewerAdjustment) ? ReviewerAdjustment : encryptService.Decrypt(ReviewerAdjustment);
        }
    }
}
