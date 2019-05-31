namespace NexGoal.Web.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Auth;

    public class SessionService : ISessionService
    {
        private readonly ISession _session;

        public SessionService(ISession session)
        {
            _session = session;
        }

        public SessionService(IHttpContextAccessor accessor)
        {
            _session = accessor.HttpContext.Session;
        }

        public int EmployeeId
        {
            get => _session?.GetInt32(SessionKey.InternalEmployeeId) ?? 0;
            set => _session?.SetInt32(SessionKey.InternalEmployeeId, value);
        }

        public string NickName
        {
            get => _session.GetString(SessionKey.InternalNickName) ?? string.Empty;
            set => _session.SetString(SessionKey.InternalNickName, value);
        }

        public string FullName
        {
            get => _session.GetString(SessionKey.InternalFullName) ?? string.Empty;
            set => _session.SetString(SessionKey.InternalFullName, value);
        }

        public DateTime JoinedDate
        {
            get => _session.Get<DateTime>(SessionKey.InternalJoinedDate);
            set => _session.Set(SessionKey.InternalJoinedDate, value);
        }

        public DateTime? ConfirmedDate
        {
            get => _session.Get<DateTime?>(SessionKey.InternalConfirmedDate);

            set => _session.Set(SessionKey.InternalConfirmedDate, value);
        }

        public IEnumerable<MenuItemSession> MenuItems
        {
            get => _session.Get<List<MenuItemSession>>(SessionKey.InternalMenuItemList);
            set => _session.Set(SessionKey.InternalMenuItemList, value);
        }

        public IEnumerable<FunctionSession> Functions
        {
            get => _session.Get<List<FunctionSession>>(SessionKey.InternalFunctionList);
            set => _session.Set(SessionKey.InternalFunctionList, value);
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            var result = false;

            if (Functions != null && Functions.Any(x => x.FunctionKey == functionKey))
            {
                switch (policy)
                {
                    case Policy.Read:
                        result = Functions.Any(x => x.FunctionKey == functionKey && (x.Policy == Policy.Read || x.Policy == Policy.Host));
                        break;

                    case Policy.Host:
                        result = Functions.Any(x => x.FunctionKey == functionKey && x.Policy == Policy.Host);
                        break;

                    default:
                        result = false;
                        break;
                }
            }

            return result;
        }

        public void ClearAll()
        {
            _session.Clear();
        }
    }
}