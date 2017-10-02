namespace NexGoal.Model.Dashboard
{
    using System.Collections.Generic;

    public class ToDoListModel
    {
        public IEnumerable<EPAInfoModel> EPAInfos { get; set; }

        public IEnumerable<NominationModel> Nominations { get; set; }

        public IEnumerable<PAFocusModel> PAFocuses { get; set; }
    }
}