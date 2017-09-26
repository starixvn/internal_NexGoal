namespace NexGoal.Model.Period
{
    using NexGoal.Model.TargetType;
    using System;

    public class PeriodModel
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? AttendanceStartDate { get; set; }

        public DateTime? AttendanceEndDate { get; set; }

        public DateTime? PAFocusStartDate { get; set; }

        public int TotalEmployees { get; set; }

        public int TotalEmployeesUnfinished { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public TargetTypeModel TargetType { get; set; }

        public int? Year
        {
            get
            {
                int? year = null;

                if (StartDate != null && TargetType != null && TargetType.TargetTypeId != (int)Core.Enumerations.TargetType.Probation)
                {
                    year = StartDate.Value.Year;
                }

                return year;
            }
        }

        public bool IsEndProcess
        {
            get
            {
                return EndDate < DateTime.Now;
            }
        }
    }
}