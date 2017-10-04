namespace NexGoal.Model.PAFocus
{
    using System;

    public class PAFocusMailModel
    {
        public int PeriodId { get; set; }

        public string PeriodName { get; set; }

        public int EvaluatorId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MailMode { get; set; }
    }
}