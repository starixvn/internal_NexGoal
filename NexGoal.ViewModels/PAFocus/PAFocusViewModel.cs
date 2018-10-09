namespace NexGoal.ViewModels.PAFocus
{
    using System.Collections.Generic;
    using NexGoal.Models.PAFocus;

    public class PAFocusViewModel
    {
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public int MaxBonus { get; } = 100;
        public int TotalBonus { get; set; }
        public IEnumerable<PAFocusModel> Members { get; set; } = new List<PAFocusModel>();
    }
}