namespace NexGoal.Model.Department
{
    using NexGoal.Model.Enitites;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class DepartmentModel
    {
        public DepartmentModel()
        {
            Childrens = new List<DepartmentModel>();
        }

        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public bool IsActive { get; set; }

        public bool IsGroup { get; set; }

        public Employee Manager { get; set; }

        public List<DepartmentModel> Childrens { get; set; }
    }
}