namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model;
    using NexGoal.Model.Department;
    using NexGoal.Model.Enitites;
    using NexGoal.Repositories.Interfaces;

    public class NominationRepository : INominationRepository
    {
        public NominatorSettingModel GetLatestNominationByNominatorId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<NominatorSettingModel, EOQYModel, NominatorSettingModel>(
                    (setting, period) =>
                    {
                        setting.Period = period;
                        return setting;
                    },
                    "EOQYId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<NominationModel> GetNominationByNominatorId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<NominationModel> nominations =
                    db.Query<NominationModel, EOQYModel, NomineeModel, DepartmentModel, Group, JobTitle, NomineeModel, NominationModel>(
                        (nomination, period, nominee, dept, gr, jb, nominator) =>
                        {
                            nomination.Period = period;
                            nominee.Department = dept;
                            nominee.Group = gr;
                            nominee.JobTitle = jb;
                            nomination.Nominee = nominee;
                            nomination.Nominator = nominator;

                            return nomination;
                        },
                        "EOQYId, EmployeeId, DepartmentId, GroupId, JobTitleId, NominatorId",
                        criteria.GetSpParams());

                return nominations;
            }
        }

        public IEnumerable<NominationEPAModel> GetNominationByPeriodId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<NominationEPAModel> nominations = db.Query<NominationEPAModel>(criteria.GetSpParams());

                return nominations;
            }
        }

        public NominationModel GetNominationById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    NominationModel nomination =
                        multi.Read<NominationModel, EOQYModel, NomineeModel, DepartmentModel, Group, JobTitle, NomineeModel, NominationModel>(
                            (nt, period, nominee, dept, gr, jb, nominator) =>
                            {
                                nt.Period = period;
                                nominee.Department = dept;
                                nominee.Group = gr;
                                nominee.JobTitle = jb;
                                nt.Nominee = nominee;
                                nt.Nominator = nominator;

                                return nt;
                            },
                            "EOQYId, EmployeeId, DepartmentId, GroupId, JobTitleId, NominatorId").FirstOrDefault();

                    nomination.NominationDetails = multi.Read<QuestionModel>().ToList();

                    return nomination;
                }
            }
        }
    }
}