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
            get => _session.Get<int>(SessionKey.InternalEmployeeId);
            set => _session.Set(SessionKey.InternalEmployeeId, value);
        }

        public string NickName
        {
            get => _session.Get<string>(SessionKey.InternalNickName);
            set => _session.Set(SessionKey.InternalNickName, value);
        }

        public string FullName
        {
            get => _session.Get<string>(SessionKey.InternalFullName);
            set => _session.Set(SessionKey.InternalFullName, value);
        }

        public bool IsInProbation
        {
            get => _session.Get<bool>(SessionKey.InternalIsInProbation);
            set => _session.Set(SessionKey.InternalIsInProbation, value);
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