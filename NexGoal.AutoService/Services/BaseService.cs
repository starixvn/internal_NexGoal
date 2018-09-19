namespace NexGoal.AutoService.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Models;
    using Repositories;

    public class BaseService : IBaseService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBaseRepository baseRepository;

        public BaseService(
            IDynamicRepository dynamicRepository,
            IBaseRepository baseRepository)
        {
            this.dynamicRepository = dynamicRepository;
            this.baseRepository = baseRepository;
        }

        public IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Fetch<T>(criteria);
            }

            return new List<T>();
        }

        public Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>> FetchMultiple<TFirst, TSecond>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.FetchMultiple<TFirst, TSecond>(criteria);
            }

            return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>(new List<TFirst>(), new List<TSecond>());
        }

        public T GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Get<T>(criteria);
            }

            return default(T);
        }

        public CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = baseRepository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }
    }
}