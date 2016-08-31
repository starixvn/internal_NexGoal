namespace NexGoal.Tools
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CommonRepository : ICommonRepository
    {
        public IList<EPAModel> GetEPAInfoToChangeCertificate(IDbSetting setting, ICriteria criteria)
        {
            using (var db = new ObjectDb(setting))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IList<EPAModel> epaInfos = multi.Read<EPAModel>().ToList();

                    if (epaInfos.Any())
                    {
                        IList<EPAProcessModel> epaProcesses = multi.Read<EPAProcessModel>().ToList();

                        IList<EPAGroupDetailModel> epaGroupDetails = multi.Read<EPAGroupDetailModel>().ToList();

                        IList<EPADetailModel> epaDetails = multi.Read<EPADetailModel>().ToList();

                        foreach (EPAGroupDetailModel detail in epaGroupDetails)
                        {
                            detail.EPADetails = epaDetails.Where(x => x.EPAId == detail.EPAId
                                && x.ProcessId == detail.ProcessId
                                && x.EvaluatorId == detail.EvaluatorId
                                && x.KPIGroupId == detail.KPIGroupId).ToList();
                        }

                        foreach (EPAProcessModel process in epaProcesses)
                        {
                            process.EPAGroupDetails = epaGroupDetails.Where(x => x.EPAId == process.EPAId
                                && x.ProcessId == process.ProcessId
                                && x.EvaluatorId == process.EvaluatorId).ToList();
                        }

                        foreach (EPAModel epaInfo in epaInfos)
                        {
                            epaInfo.EPAProcesses = epaProcesses.Where(x => x.EPAId == epaInfo.EPAId).ToList();
                        }
                    }

                    return epaInfos;
                }
            }
        }
    }
}