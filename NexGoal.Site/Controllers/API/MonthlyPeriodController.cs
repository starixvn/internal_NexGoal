namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    public class MonthlyPeriodController : BaseController
    {
        private readonly IPeriodService _periodService;

        public MonthlyPeriodController()
            : this(null)
        {
        }

        public MonthlyPeriodController(
            IPeriodService periodService)
        {
            _periodService = periodService ?? new PeriodService();
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Read)]
        public IEnumerable<PeriodModel> GetMonthlyPeriod(
            DateTime fromDate,
            DateTime toDate)
        {
            var criteria = new GetMonthlyPeriodCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            IEnumerable<PeriodModel> periods = _periodService.GetMonthlyPeriod(criteria);

            return periods;
        }

        public IEnumerable<PeriodModel> GetAllMonthlyPeriod()
        {
            var criteria = new GetAllMonthlyPeriodCriteria();

            IEnumerable<PeriodModel> periods = _periodService.GetAllMonthlyPeriod(criteria);

            return periods;
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Read)]
        public PeriodModel GetMonthlyPeriodById(int periodId)
        {
            var criteria = new GetPeriodByIdCriteria
            {
                PeriodId = periodId
            };

            PeriodModel period = _periodService.GetPeriodById(criteria);

            return period;
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Host)]
        [HttpPost]
        public CommandResult CreateMonthlyPeriod(CreatePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.TargetTypeId = (int)TargetType.Monthly;
                criteria.IsActive = true;
                criteria.CreatedBy = CurrentUser.EmployeeId;
                commandResult = _periodService.CreatePeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateBasicInfoMonthlyPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.TargetTypeId = (int)TargetType.Monthly;
                criteria.IsActive = true;
                criteria.ModifiedBy = CurrentUser.EmployeeId;
                commandResult = _periodService.UpdateBasicInformationPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Read)]
        public IEnumerable<FlowModel> GetProcessSettingByPeriodId(int periodId)
        {
            var criteria = new GetProcessSettingPeriodCriteria
            {
                PeriodId = periodId
            };

            IEnumerable<FlowModel> processSettings = _periodService.GetProcessSettingByPeriodId(criteria);

            return processSettings;
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateProcessSettingMonthlyPeriod(UpdateProcessSettingPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;

                commandResult = _periodService.UpdateProcessSettingPeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteMonthlyPeriod(int periodId)
        {
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = periodId,
                ModifiedBy = CurrentUser.EmployeeId
            };

            CommandResult commandResult = _periodService.DeletePeriod(criteria);

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Read)]
        [HttpGet]
        public IEnumerable<PeriodEmployeeSettingModel> GetEmployeeSettingMonthlyPeriodById(
            int periodId,
            int? departmentId = null,
            bool? isLocal = null,
            bool? isExcludedEmployee = null,
            bool? isGeneratedEPA = null,
            int? isEvaluated = null,
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
                EPAStatus = epaStatus
            };

            IEnumerable<PeriodEmployeeSettingModel> employeeSettings = _periodService.GetPeriodEmployeeSettingById(criteria);
            employeeSettings = employeeSettings.OrderBy(x => x.Group.GroupCode).ThenBy(x => x.Employee.NickName);

            return employeeSettings;
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Host)]
        [HttpPost]
        public CommandResult AddEmployeeToMonthlyPeriod(AssignEmployeePeriodModel model)
        {
            var commandResult = new CommandResult();

            if (ModelState.IsValid && model != null)
            {
                var criteria = new AddEmployeeToPeriodCriteria
                {
                    PeriodId = model.PeriodId,
                    EmployeeId = model.EmployeeId,
                    Departments = model.ConvertToXml(),
                    ModifiedBy = CurrentUser.EmployeeId
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

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Host)]
        [HttpPut]
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
                    ModifiedBy = CurrentUser.EmployeeId
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

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Host)]
        [HttpPost]
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

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateIncludeStatusEmployeePeriod(UpdateIncludeStatusEmployeePeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
                commandResult = _periodService.UpdateIncludeStatusEmployeePeriod(criteria);
            }
            else
            {
                commandResult.Code = 1;
                commandResult.Message = Lang.ParameterInvalid;
            }

            return commandResult;
        }

        /// [ApiAuthorize(EPAFunctionKey.MonthlyPeriod, Policy.Host)]
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

        public int GetUserPermissionOnMonthlyPeriod()
        {
            var permission = Policy.None;
            var isRead = CurrentUser.HasPermission(EPAFunctionKey.MonthlyPeriod, Policy.Read);
            var isHost = CurrentUser.HasPermission(EPAFunctionKey.MonthlyPeriod, Policy.Host);
            if (isRead)
            {
                permission = Policy.Read;
            }
            else if (isHost)
            {
                permission = Policy.Host;
            }

            return (int)permission;
        }
    }
}