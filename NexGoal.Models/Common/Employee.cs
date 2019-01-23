namespace NexGoal.Models.Common
{
    using System;
    using NexGoal.Core.Constants;

    public class Employee
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

        public virtual DateTime JoinedDate { get; set; }
        public virtual bool IsProbation { get; set; }

        public int LinkObjectId { get; set; }

        public virtual string Avatar
        {
            get
            {
                return GetFilePath(Code);
            }
        }

        private string GetFilePath(string code)
        {
            var imgUrl = EmployeeId == Constants.NexGoalSystemId ? "/dist/avatars/nexgoal.jpg" : new Uri($"{Constants.ImageURL}/{code}.jpg").ToString();
            return imgUrl;
        }
    }
}