namespace NexGoal.Tools
{
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CommonRepository : ICommonRepository
    {
        public EPAViewModel GetEPAInfoToChangeCertificate(IDbSetting setting, ICriteria criteria)
        {
            using (var db = new ObjectDb(setting))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    EPAViewModel epaInfo = new EPAViewModel();
                    epaInfo.Epas = multi.Read<EPAModel>().ToList();

                    if (epaInfo.Epas.Any())
                    {
                        epaInfo.Processes = multi.Read<EPAProcessModel>().ToList();
                        epaInfo.EpaGroups = multi.Read<EPAGroupDetailModel>().ToList();
                        epaInfo.EpaDetails = multi.Read<EPADetailModel>().ToList();
                    }

                    return epaInfo;
                }
            }
        }
    }
}