namespace NexGoal.Models.PerformanceRecord
{
    using System;
    using NexGoal.Core.Enumerations;

    public class PerformanceRecordModel
    {
        public int PerformanceRecordId { get; set; }
        public string GradeId { get; set; }
        public string Comment { get; set; }
        public PerformanceRecordStatus StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string Base64Image { get; set; }

        public int CreatorId { get; set; }
        public string NickNameOfCreator { get; set; }
        public string FullNameOfCreator { get; set; }
        public string CodeOfCreator { get; set; }
        public string Base64ImageOfCreator { get; set; }
    }
}