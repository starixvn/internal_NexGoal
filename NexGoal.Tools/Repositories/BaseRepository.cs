namespace NexGoal.Tools
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class BaseRepository : IBaseRepository
    {
        public CommandResult Insert(IDbSetting setting, ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(setting, criteria, hasGetOutputParams);
        }

        public CommandResult Update(IDbSetting setting, ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(setting, criteria, hasGetOutputParams);
        }

        public CommandResult Delete(IDbSetting setting, ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(setting, criteria, hasGetOutputParams);
        }

        private CommandResult ExecuteNonQuery(IDbSetting setting, ICriteria criteria, bool hasGetOutputParams = false)
        {
            using (var db = new ObjectDb(setting))
            {
                Hashtable hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                int result = db.ExecuteNonQuery(hash);
                CommandResult commandResult = GetCommandResult(hash);

                if (result == -1 && commandResult.IsSuccessful && hasGetOutputParams)
                {
                    SetValueForOutputProperty(criteria, hash);
                }

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

        private void SetValueForOutputProperty(ICriteria criteria, Hashtable hash)
        {
            IEnumerable<PropertyInfo> properties = criteria.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(OutputAttribute)));

            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    var value = hash[property.Name.ToLower()];
                    property.SetValue(criteria, value, null);
                }
            }
        }
    }
}