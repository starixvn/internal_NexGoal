namespace NexGoal.AutoService.Repositories
{
    using System;
    using System.Collections;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models;

    public class BaseRepository : IBaseRepository
    {
        public CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
                CommandResult commandResult = GetCommandResult(criteria.GetSpParams());

                return commandResult;
            }
        }

        private CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
    }
}