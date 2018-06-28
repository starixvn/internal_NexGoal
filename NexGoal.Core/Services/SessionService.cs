namespace NexGoal.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Constants;
    using Enumerations;
    using Microsoft.AspNetCore.Http;
    using Models;

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

        public IEnumerable<MenuItem> MenuItems
        {
            get => _session.Get<List<MenuItem>>(SessionKey.InternalMenuItemList);
            set => _session.Set(SessionKey.InternalMenuItemList, value);
        }

        public IEnumerable<FunctionModel> Functions
        {
            get => _session.Get<List<FunctionModel>>(SessionKey.InternalFunctionList);
            set => _session.Set(SessionKey.InternalFunctionList, value);
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            var result = false;

            if (Functions != null && Functions.Any())
            {
                switch (policy)
                {
                    case Policy.Read:
                        result = Functions.Any(x => x.Policy == Policy.Read || x.Policy == Policy.Host);
                        break;

                    case Policy.Host:
                        result = Functions.Any(x => x.Policy == Policy.Host);
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