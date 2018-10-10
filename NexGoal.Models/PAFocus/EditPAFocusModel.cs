namespace NexGoal.Models.PAFocus
{
    using System;
    using NexGoal.Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class EditPAFocusModel
    {
        public int PAFocusId { get; set; }
        public string IsReadyPromotion { get; set; }
        public string Contribution { get; set; }
        public string BonusGiven { get; set; }
        public string CurrentObjective { get; set; }
        public string NextObjective { get; set; }
    }
}