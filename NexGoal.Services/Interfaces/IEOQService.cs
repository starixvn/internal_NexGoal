namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.EOQY;

    public interface IEOQService
    {
        Task<IEnumerable<EOQYModel>> GetAllEOQAsync(int? year = null, int? statusId = null);

        Task<EOQYModel> GetEOQByIdAsync(int eoqId);

        Task<IEnumerable<int>> GetYearEOQAsync();
    }
}