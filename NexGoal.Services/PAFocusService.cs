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

    public class PAFocusService : IPAFocusService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;

        public PAFocusService(
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
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

        public async Task<IEnumerable<EmployeeBaseModel>> GetMemberAsync(int actorId)
        {
            var criteria = new GetMemberCriteria
            {
                ActorId = actorId
            };

            return await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
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

            var paFocuses = await baseService.GetDataAsync<PAFocusModel>(criteria).ConfigureAwait(false);
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync().ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            foreach (PAFocusModel paFocus in paFocuses)
            {
                paFocus.Employee = employees.FirstOrDefault(x => x.EmployeeId == paFocus.EmployeeId);

                paFocus.IsReadyPromotion = cryptoService.Decrypt(paFocus.IsReadyPromotion);
                paFocus.Contribution = cryptoService.Decrypt(paFocus.Contribution);
                paFocus.BonusGiven = cryptoService.Decrypt(paFocus.BonusGiven);

                paFocus.GradeId = cryptoService.Decrypt(paFocus.GradeId);
                int.TryParse(paFocus.GradeId, out int gradeId);
                paFocus.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
            }

            return paFocuses;
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