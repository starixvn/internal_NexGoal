namespace NexGoal.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model;
    using NexGoal.Model.Department;
    using NexGoal.Model.Enitites;
    using NexGoal.Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

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
                var nominations = db.Query<NominationModel, EOQYModel, NomineeModel, DepartmentModel, Group, JobTitle, NomineeModel, NominationModel>(
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
                return db.Query<NominationEPAModel>(criteria.GetSpParams());
            }
        }

        public NominationModel GetNominationById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var nomination = multi.Read<NominationModel, EOQYModel, NomineeModel, DepartmentModel, Group, JobTitle, NomineeModel, NominationModel>(
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

        public IEnumerable<EOQYEmailModel> GetToSendEmail(ICriteria criteria)
        {
            List<EOQYEmailModel> nominationEmail = new List<EOQYEmailModel>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var nominations = multi.Read<EOQYModel>();
                    var email = multi.Read<EmailNominatorModel>();

                    foreach (var nomination in nominations)
                    {
                        nominationEmail.Add(new EOQYEmailModel { Nomination = nomination, EmailNominator = email.Where(i => i.EOQYId == nomination.EOQYId) });
                    }
                }
            }

            return nominationEmail;
        }
    }
}