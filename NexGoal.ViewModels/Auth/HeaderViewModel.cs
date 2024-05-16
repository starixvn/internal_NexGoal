namespace NexGoal.ViewModels.Auth
{
    using System;
    using System.Collections.Generic;
    using NexGoal.Models.Auth;
    using NexGoal.Models.Common;

    public class HeaderViewModel
    {
        public IEnumerable<MenuItemSession> MenuItems { get; set; }

        public Employee UserInfo { get; set; }

        public int UnreadNotification { get; set; }

        public bool HasMember { get; set; }
        
        public Uri ImageUrl { get; set; }
        public string LogoName { get; set; }
    }
}