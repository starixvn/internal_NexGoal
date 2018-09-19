namespace NexGoal.AutoService.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Models;

    public interface IBaseService
    {
        IEnumerable<T> GetData<T>(ICriteria criteria);

        Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>> FetchMultiple<TFirst, TSecond>(ICriteria criteria);

        T GetDataEntity<T>(ICriteria criteria);

        CommandResult ExecuteNonQuery(ICriteria criteria);
    }
}