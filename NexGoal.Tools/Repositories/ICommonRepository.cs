namespace NexGoal.Tools
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;

    public interface ICommonRepository
    {
        IList<EPAModel> GetEPAInfoToChangeCertificate(ICriteria criteria);
    }
}