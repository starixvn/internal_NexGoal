namespace NexGoal.Model.PAFocus
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Model.Employee;
    using System;

    public class PAFocusProcessModel
    {
        public int PeriodId { get; set; }

        public int ProcessId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ProcessStatus
        {
            get
            {
                if (StartDate.Date > DateTime.Now.Date)
                {
                    return (int)ProcessStatusType.Open;
                }
                else if (StartDate.Date <= DateTime.Now.Date && DateTime.Now.Date <= EndDate.Date)
                {
                    return (int)ProcessStatusType.InProgress;
                }
                else if (DateTime.Now.Date > EndDate.Date)
                {
                    return (int)ProcessStatusType.Closed;
                }

                return (int)ProcessStatusType.None;
            }
        }
    }
}
