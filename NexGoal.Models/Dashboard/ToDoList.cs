namespace NexGoal.Model.Dashboard
{
    using System.Collections.Generic;

    public class ToDoList
    {
        public bool IsReadyEOQ { get; set; }

        public bool IsReadyEOY { get; set; }

        public bool IsReadyPAFocus { get; set; }

        public IEnumerable<int> MyProperty { get; set; }
    }
}