namespace NexGoal.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Fanex.Data.Repository;
    using Model.Nomination;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Core.Utilities;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;

    public class NominationService : BaseService, INominationService
    {
        private const int TotalRows = 5;
        private const int RowStartHeader = 2;
        private const int RowStartBody = 3;

        private readonly INominationRepository nominationRepository;
        private readonly ISessionService sessionService;

        public NominationService()
            : this(null, null)
        {
        }

        public NominationService(INominationRepository nominationRepository, ISessionService sessionService)
        {
            this.nominationRepository = nominationRepository ?? new NominationRepository();
            this.sessionService = sessionService ?? new SessionService();
        }

        public NominatorSettingModel GetLatestEOQByNominatorId(int nominatorId)
        {
            var criteria = new GetLatestEOQByNominatorIdCriteria
            {
                NominatorId = nominatorId
            };

            var result = new NominatorSettingModel();

            if (criteria.IsValid())
            {
                result = nominationRepository.GetLatestNominationByNominatorId(criteria);
            }

            return result;
        }

        public NominatorSettingModel GetLatestEOYByNominatorId(int nominatorId)
        {
            var criteria = new GetLatestEOYByNominatorIdCriteria
            {
                NominatorId = nominatorId
            };

            var result = new NominatorSettingModel();

            if (criteria.IsValid())
            {
                result = nominationRepository.GetLatestNominationByNominatorId(criteria);
            }

            return result;
        }

        public IEnumerable<NominationViewModel> GetNominationByNominatorId(GetNominationByNominatorIdCriteria criteria)
        {
            IEnumerable<NominationViewModel> result = new List<NominationViewModel>();

            if (criteria != null && criteria.IsValid())
            {
                IEnumerable<NominationModel> nominations = nominationRepository.GetNominationByNominatorId(criteria);

                result = nominations.GroupBy(
                    x => new { x.Period.EOQYId, x.Nominee.EmployeeId },
                    (key, g) => new NominationViewModel
                    {
                        Period = g.First().Period,
                        Nominee = g.First().Nominee,
                        Nominations = g.Select(x => new NominationDetailViewModel
                        {
                            NominationId = x.NominationId,
                            Nominator = x.Nominator
                        }).ToList(),
                        RoleId = g.First().RoleId,
                        StatusId = g.First().StatusId
                    });
            }

            return result;
        }

        public IEnumerable<NominationEPAModel> GetNominationByPeriodId(int nomineeId, int periodId)
        {
            IEnumerable<NominationEPAModel> result = new List<NominationEPAModel>();
            var policyToView = CheckPermissionToViewNomination(nomineeId);
            if (policyToView != Policy.None)
            {
                var criteria = new GetNominationByPeriodIdCriteria
                {
                    PeriodId = periodId,
                    NomineeId = nomineeId
                };

                if (criteria.IsValid())
                {
                    result = nominationRepository.GetNominationByPeriodId(criteria);
                }
            }

            return result;
        }

        public IEnumerable<QuestionModel> GetEOQQuestion()
        {
            var criteria = new GetEOQQuestionCriteria();
            IEnumerable<QuestionModel> questions = this.GetData<QuestionModel>(criteria);

            return questions;
        }

        public IEnumerable<QuestionModel> GetEOYQuestion()
        {
            var criteria = new GetEOYQuestionCriteria();
            IEnumerable<QuestionModel> questions = this.GetData<QuestionModel>(criteria);

            return questions;
        }

        public CommandResult CreateEOQNomination(CreateEOQNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult CreateEOYNomination(CreateEOYNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Insert(criteria);
            }

            return commandResult;
        }

        public NominationModel GetEOQNominationById(int nominationId)
        {
            var nomination = new NominationModel();
            var criteria = new GetEOQNominationByIdCriteria
            {
                NominationId = nominationId
            };

            if (criteria.IsValid())
            {
                nomination = nominationRepository.GetNominationById(criteria);
            }

            return nomination;
        }

        public NominationModel GetEOYNominationById(int nominationId)
        {
            var nomination = new NominationModel();
            var criteria = new GetEOYNominationByIdCriteria
            {
                NominationId = nominationId
            };

            if (criteria.IsValid())
            {
                nomination = nominationRepository.GetNominationById(criteria);
            }

            return nomination;
        }

        public CommandResult UpdateNomination(UpdateNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteNomination(DeleteNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Delete(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateStatusNomination(UpdateStatusNominationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = this.Repository.Update(criteria);
            }

            return commandResult;
        }

        #region Export Excel

        public void ExportEOQ(IEnumerable<NominationViewModel> nominations, string periodName, string templatePath)
        {
            string fileName = periodName.Replace(" ", "_");
            var writer = new ExcelTemplateWriter(templatePath, fileName, periodName);

            /// Fill report title.
            string title = "Employee of the " + periodName;
            writer.WriteRow(0, new object[] { title });
            writer.CopyRows(1, 1);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 1);

            FillContentRowEOQY(ref writer, nominations);
            writer.SyncColumnsWidth(TotalRows);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        public void ExportEOY(IEnumerable<NominationViewModel> nominations, string periodName, string templatePath)
        {
            string fileName = "EOY_" + periodName;
            var writer = new ExcelTemplateWriter(templatePath, fileName, periodName);

            /// Fill report title.
            string title = "Employee of the year " + periodName;
            writer.WriteRow(0, new object[] { title });
            writer.CopyRows(1, 1);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 1);

            FillContentRowEOQY(ref writer, nominations);
            writer.SyncColumnsWidth(TotalRows);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowEOQY(ref ExcelTemplateWriter writer, IEnumerable<NominationViewModel> nominations)
        {
            var rowIndex = 1;

            foreach (NominationViewModel nomination in nominations)
            {
                FillRowEOQY(nomination, ref writer, ref rowIndex);
            }
        }

        private void FillRowEOQY(NominationViewModel nomination, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordEOQY(nomination, rowIndex);

            if (rowIndex % 2 == 1)
            {
                writer.WriteRow(RowStartBody, record);
            }
            else
            {
                writer.WriteRow(RowStartBody + 1, record);
            }

            rowIndex++;
        }

        private object CreateRecordEOQY(NominationViewModel nomination, int rowIndex)
        {
            string nominator = string.Empty,
                nominee = nomination.Nominee.FullName + " (" + nomination.Nominee.NickName + ")";

            if (nomination.NumberOfNominators > 1)
            {
                nominee += " - (" + nomination.NumberOfNominators + " votes)";
                IEnumerable<string> nominators = nomination.Nominations.Select(x => "- " + x.Nominator.FullName + " (" + x.Nominator.NickName + ")");
                nominator = string.Join(Environment.NewLine, nominators);
            }
            else
            {
                nominator = nomination.Nominations.First().Nominator.FullName + " (" + nomination.Nominations.First().Nominator.NickName + ")";
            }

            var record = new
            {
                Count = rowIndex,
                Nominee = nominee,
                Nominator = nominator,
                Team = nomination.Nominee.Group.GroupName,
                Status = nomination.StatusToString
            };

            return record;
        }

        #endregion Export Excel

        private Policy CheckPermissionToViewNomination(int employeeId)
        {
            if (employeeId != sessionService.EmployeeId)
            {
                var policy = sessionService.CurrentUser.GetPermission(FunctionKey.Evaluator) + sessionService.CurrentUser.GetPermission(FunctionKey.Reviewer);

                return policy > (int)Policy.None ? Policy.Read : Policy.None;
            }

            return Policy.Read;
        }

        public IEnumerable<EOQYEmailModel> GetToSendEmail(ICriteria criteria)
        {
            return nominationRepository.GetToSendEmail(criteria);
        }
    }
}