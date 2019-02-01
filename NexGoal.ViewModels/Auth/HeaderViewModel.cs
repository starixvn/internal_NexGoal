namespace NexGoal.ViewModels.Auth
{
    using System.Collections.Generic;
    using NexGoal.Models.Auth;
    using NexGoal.Models.Common;

    public class HeaderViewModel
    {
        public IEnumerable<MenuItemSession> MenuItems { get; set; }

        public Employee CurrentUserInfo { get; set; }

        public int UnreadNotification { get; set; }

        public bool IsHasMember { get; set; }
    }
}