namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using NexGoal.Repositories;

    public class PAFocusService : IPAFocusService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;
        private readonly IPAFocusRepository focusRepository;

        public PAFocusService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService,
            IPAFocusRepository focusRepository)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
            this.focusRepository = focusRepository;
        }

        public async Task<IEnumerable<PeriodBaseModel>> GetPeriodAsync()
        {
            var criteria = new GetPeriodCriteria();

            return await baseService.GetDataAsync<PeriodBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupBaseModel>> GetGroupAsync(int actorId)
        {
            var criteria = new GetGroupCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<GroupBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Employee>> GetMemberAsync(int actorId, string groupIds = null)
        {
            var criteria = new GetMemberCriteria
            {
                ActorId = actorId,
                GroupIds = groupIds
            };

            return await baseService.GetDataAsync<Employee>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PAFocusModel>> GetPAFocusAsync(GetPAFocusParam param)
        {
            var criteria = new GetPAFocusCriteria
            {
                ActorId = param.ActorId,
                PeriodId = param.PeriodId,
                GroupIds = param.GroupIds,
                EmployeeId = param.EmployeeId
            };

            IEnumerable<PAFocusModel> pafocuses = await focusRepository.GetPAFocusAsync(criteria).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (PAFocusModel pafocus in pafocuses)
            {
                pafocus.IsReadyPromotion = cryptoService.Decrypt(pafocus.IsReadyPromotion);
                pafocus.Contribution = cryptoService.Decrypt(pafocus.Contribution);
                pafocus.BonusGiven = cryptoService.Decrypt(pafocus.BonusGiven);

                pafocus.GradeId = cryptoService.Decrypt(pafocus.GradeId);
                int.TryParse(pafocus.GradeId, out int gradeId);
                pafocus.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
            }

            return pafocuses;
        }

        public async Task<CommandResult> EditPAFocusAsync(int actorId, IEnumerable<EditPAFocusModel> paFocuses)
        {
            foreach (EditPAFocusModel paFocus in paFocuses)
            {
                paFocus.IsReadyPromotion = cryptoService.Encrypt(paFocus.IsReadyPromotion);
                paFocus.Contribution = cryptoService.Encrypt(paFocus.Contribution);
                paFocus.BonusGiven = cryptoService.Encrypt(paFocus.BonusGiven);
            }

            var criteria = new EditPAFocusCriteria
            {
                ActorId = actorId,
                Value = paFocuses.ToXML().ToString()
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}