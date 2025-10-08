namespace NexGoal.ViewModels.Auth
{
    using System;
    using NexGoal.Models.Common;

    public class HeaderViewModel
    {
        public Employee UserInfo { get; set; }

        public int UnreadNotification { get; set; }

        public bool HasMember { get; set; }
        
        public Uri ImageUrl { get; set; }
        public string AppName { get; set; }
    }
}