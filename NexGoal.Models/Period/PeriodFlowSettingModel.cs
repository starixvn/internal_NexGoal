namespace NexGoal.Models.Period
{
    using System;
    using System.Collections.Generic;

    public class PeriodFlowSettingModel
    {
        public int FlowSettingId { get; set; }

        public int SettingId { get; set; }

        public int ProcessId { get; set; }

        public string Process { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<PeriodFlowEvaluatorSettingModel> Evaluators { get; set; }
    }
}