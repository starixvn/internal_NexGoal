using System.Collections.Generic;

namespace NexGoal.Models.PRTracking
{
    public class PRTrackingModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string JobTitle { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public IEnumerable<PRTrackingDetailModel> Details { get; set; }
    }
}