namespace NexGoal.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Period;
    using NexGoal.Criteria.Period.GenerationEvaluation;
    using NexGoal.Model.FlowManagement;
    using NexGoal.Model.Period;
    using NexGoal.Resources;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web;
    using NexGoal.Web.ApiAuthorizeAttribute;
    using NexGoal.Web.Services;

    public class PeriodController : BaseController
    {
        private readonly IPeriodService _periodService;
        private readonly IWebCommonService _webCommonService;

        public PeriodController()
            : this(null, null)
        {
        }

        public PeriodController(
            IPeriodService periodService,
            IWebCommonService webCommonService)
        {
            _periodService = periodService ?? new PeriodService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
        public IEnumerable<PeriodModel> GetPeriod(int? targetTypeId = null, bool? isActive = true)
        {
            var criteria = new GetPeriodCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            IEnumerable<PeriodModel> periods = _periodService.GetPeriod(criteria);
            IEnumerable<PeriodModel> probationPeriods = periods.Where(x => x.TargetType.TargetTypeId == (int)TargetType.Probation);
            IEnumerable<PeriodModel> promotionPeriods = periods.Where(
                x => x.TargetType.TargetTypeId == (int)TargetType.Promotion)
                .OrderByDescending(x => x.StartDate);
            IEnumerable<PeriodModel> semiPeriods = periods.Where(
                x => x.TargetType.TargetTypeId == (int)TargetType.SemiAnnual)
                .OrderByDescending(x => x.StartDate);

            IEnumerable<PeriodModel> periodList = new List<PeriodModel>();

            if (probationPeriods != null && probationPeriods.Any())
            {
                periodList = periodList.Concat(probationPeriods);
            }

            if (promotionPeriods != null && promotionPeriods.Any())
            {
                periodList = periodList.Concat(promotionPeriods);
            }

            if (semiPeriods != null && semiPeriods.Any())
            {
                periodList = periodList.Concat(semiPeriods);
            }

            return periodList;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult CreatePeriod(CreatePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.CreatePeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.UpdateBasicInformationPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
        public PeriodModel GetPeriodById(int periodId)
        {
            var criteria = new GetPeriodByIdCriteria
            {
                PeriodId = periodId
            };

            PeriodModel period = _periodService.GetPeriodById(criteria);
            return period;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.UpdateStatusPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult DeletePeriod(int periodId)
        {
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = periodId,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            CommandResult commandResult = _periodService.DeletePeriod(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(
            int periodId,
            int? departmentId = null,
            bool? isLocal = null,
            bool? isExcludedEmployee = null,
            bool? isGeneratedEPA = null,
            int? isEvaluated = null,
            bool? hasMonthlyEPA = null,
            int? epaStatus = null)
        {
            var criteria = new GetPeriodEmployeeSettingCriteria
            {
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                IsExcludedEmployee = isExcludedEmployee,
                IsGeneratedEPA = isGeneratedEPA,
                IsEvaluated = isEvaluated,
                HasMonthlyEPA = hasMonthlyEPA,
                EPAStatus = epaStatus
            };

            IEnumerable<PeriodEmployeeSettingModel> periodEmployeeSettings = _periodService.GetPeriodEmployeeSettingById(criteria);

            if (periodEmployeeSettings.Any())
            {
                PeriodEmployeeSettingModel periodEmployeeSetting = periodEmployeeSettings.FirstOrDefault(x => x.TargetTemplate != null);
                int targetTypeId = periodEmployeeSetting != null ? periodEmployeeSetting.TargetTemplate.TargetTypeId : 0;

                switch (targetTypeId)
                {
                    case (int)TargetType.Probation:
                    case (int)TargetType.Promotion:
                        periodEmployeeSettings = periodEmployeeSettings.OrderByDescending(x => x.SelfEvaluators.First().StartDate);
                        break;

                    case (int)TargetType.Monthly:
                    case (int)TargetType.SemiAnnual:
                        periodEmployeeSettings = periodEmployeeSettings.OrderBy(x => x.Group.GroupCode).ThenBy(x => x.Employee.NickName);
                        break;
                }
            }

            return periodEmployeeSettings;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult AddEmployeToPeriod(AssignEmployeePeriodModel model)
        {
            var commandResult = new CommandResult();

            if (ModelState.IsValid && model != null)
            {
                var criteria = new AddEmployeeToPeriodCriteria
                {
                    PeriodId = model.PeriodId,
                    EmployeeId = model.EmployeeId,
                    Departments = model.ConvertToXml(),
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
                };

                commandResult = _periodService.AddEmployeeToPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdateEmployeeSettingOfPeriod(UpdatedEmployeeSettingPeriodModel model)
        {
            var commandResult = new CommandResult();

            if (ModelState.IsValid && model != null)
            {
                var criteria = new UpdateEmployeeSettingOfPeriodCriteria
                {
                    PeriodId = model.PeriodId,
                    EmployeeIds = model.EmployeeIds,
                    TargetTemplateId = model.TargetTemplateId,
                    EmployeeAssignments = model.ConvertToXml(),
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
                };

                commandResult = _periodService.UpdateEmployeeSettingOfPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdateIncludeStatusEmployeePeriod(UpdateIncludeStatusEmployeePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.UpdateIncludeStatusEmployeePeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult GenerateEvaluation(AutoGenerateEvaluationCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                commandResult = _periodService.GenerateEvaluation(criteria);

                if (commandResult.IsSuccessful)
                {
                    _periodService.SendEmailNoticeAtBegin(criteria.PeriodId);
                }
            }

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetProcessSettingByPeriodId(int periodId)
        {
            var criteria = new GetProcessSettingPeriodCriteria
            {
                PeriodId = periodId
            };

            IEnumerable<FlowModel> processSettings = _periodService.GetProcessSettingByPeriodId(criteria);
            return processSettings;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Period, Policy = Policy.Host)]
        public CommandResult UpdatePeriodProcessSetting(UpdateProcessSettingPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                commandResult = _periodService.UpdateProcessSettingPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        [HttpGet]
        public bool CheckSelfProcessStarted(int periodId, string employeeIds)
        {
            var criteria = new CheckSelfProcessStartedCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds
            };

            bool hasStarted = _periodService.CheckSelfProcessStarted(criteria);
            return hasStarted;
        }

        public int GetUserPermissionOnPeriod()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.Period);
            return permission;
        }
    }
}