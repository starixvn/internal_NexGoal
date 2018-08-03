namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Criteria.Common;
    using NexGoal.Models;
    using NexGoal.Models.UserManagement;

    public class CommonService : ICommonService
    {
        private readonly IBaseService baseService;

        public CommonService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] refNames, bool? isActive = true)
        {
            IEnumerable<ReferenceResourceModel> refResources = new List<ReferenceResourceModel>();

            if (refNames != null)
            {
                var criteria = new GetRefResourceByNamesCriteria
                {
                    RefNames = string.Join(",", refNames),
                    IsActive = isActive
                };

                refResources = await baseService.GetDataAsync<ReferenceResourceModel>(criteria).ConfigureAwait(false);
            }

            return refResources;
        }

        public SelectList GetSelectList<T>(IEnumerable<T> items, string dataValueField, string dataTextField)
        {
            return new SelectList(items, dataValueField, dataTextField);
        }

        public async Task<IEnumerable<UserModel>> GetEmployeesAsync(
            string keyword = null,
            int? groupId = null,
            bool? isEnabled = true)
        {
            var criteria = new SearchUserCriteria
            {
                Keyword = keyword,
                GroupId = groupId,
                IsEnabled = isEnabled
            };

            return await baseService.GetDataAsync<UserModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserGroupModel>> GetGroupsAsync()
        {
            var criteria = new GetGroupsCriteria();

            return await baseService.GetDataAsync<UserGroupModel>(criteria).ConfigureAwait(false);
        }
    }
}