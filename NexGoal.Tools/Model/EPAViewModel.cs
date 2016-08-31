namespace NexGoal.Tools
{
    using System.Collections.Generic;

    public class EPAViewModel
    {
        public EPAViewModel()
        {
            Epas = new List<EPAModel>();
            Processes = new List<EPAProcessModel>();
            EpaGroups = new List<EPAGroupDetailModel>();
            EpaDetails = new List<EPADetailModel>();
        }

        public IList<EPAModel> Epas { get; set; }

        public IList<EPAProcessModel> Processes { get; set; }

        public IList<EPAGroupDetailModel> EpaGroups { get; set; }

        public IList<EPADetailModel> EpaDetails { get; set; }
    }
}