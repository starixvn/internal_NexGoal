namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NexGoal.Models;

    public interface ICommonService
    {
        Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] refNames, bool? isActive = true);

        SelectList GetSelectList<T>(IEnumerable<T> items, string dataValueField, string dataTextField);
    }
}