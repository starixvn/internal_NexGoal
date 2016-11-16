namespace NexGoal.Tools
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CommonRepository : ICommonRepository
    {
        public IEnumerable<EPAModel> GetEPAInfoToChangeCertificate(IDbSetting setting, ICriteria criteria)
        {
            using (var db = new ObjectDb(setting))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAModel> epaInfos = multi.Read<EPAModel>();

                    if (epaInfos.Any())
                    {
                        IEnumerable<EPAProcessModel> epaProcesses = multi.Read<EPAProcessModel>();                        

                        IEnumerable<EPADetailModel> epaDetails = multi.Read<EPADetailModel>();  
                        foreach (EPAModel epaInfo in epaInfos)
                        {
                            epaInfo.EPAProcesses = epaProcesses.Where(x => x.EPAId == epaInfo.EPAId).ToList();
                        }
                    }

                    return epaInfos;
                }
            }
        }

        public IEnumerable<PAFocusModel> GetAllPAFocus(IDbSetting setting, ICriteria criteria)
        {
            using (var db = new ObjectDb(setting))
            {
                return db.Query<PAFocusModel>(criteria.GetSpParams());
            }
        }
    }
}