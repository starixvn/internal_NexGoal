namespace NexGoal.Models.Common
{
    public class EmployeeBaseModel
    {
        public virtual int EmployeeId { get; set; }
        public virtual string Code { get; set; }
        public virtual string NickName { get; set; }
        public virtual string FullName { get; set; }
        public virtual int GroupId { get; set; }
        public virtual string GroupCode { get; set; }
        public virtual string GroupName { get; set; }
        public virtual string JobTitle { get; set; }
        public virtual string Roles { get; set; }
        public virtual string Avatar { get; set; }
        public virtual bool IsProbation { get; set; }
    }
}