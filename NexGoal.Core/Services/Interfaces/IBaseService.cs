namespace NexGoal.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Models;

    public interface IBaseService
    {
        Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria);

        Task<(IEnumerable<TFirst>, IEnumerable<TSecond>)> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria);

        Task<(IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>)> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria);

        Task<T> GetDataEntityAsync<T>(ICriteria criteria);

        Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria);
    }
}