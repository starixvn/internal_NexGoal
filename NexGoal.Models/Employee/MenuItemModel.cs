namespace NexGoal.Model.Employee
{
    public class MenuItemModel
    {
        public int MenuId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public int? ParentId { get; set; }

        public int FunctionId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}