using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexGoal.Model.Nomination
{
    public class NominatorReportModel
    {
        public int NomineeId { get; set; }

        public int NominatorId { get; set; }

        public string NominatorNickName { get; set; }

        public string NominatorFullName { get; set; }
    }
}
