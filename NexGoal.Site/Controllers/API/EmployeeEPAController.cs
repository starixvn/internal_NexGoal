namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Criteria.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Model.TargetTemplate;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class EmployeeEPAController : BaseController
    {
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly IRoleService roleService;

        public EmployeeEPAController()
            : this(null, null)
        {
        }

        public EmployeeEPAController(
            IEmployeeEPAService employeeEPAService,
            IRoleService roleService)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.roleService = roleService ?? new RoleService();
        }

        #region Self

        //[ApiAuthorize(EPAFunctionKey.Self, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAModel> GetEPAByEmployee()
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = CurrentUser.EmployeeId
            };

            return _employeeEPAService.GetEPAByEmployee(criteria);
        }

        //[ApiAuthorize(EPAFunctionKey.Self, Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetEPAInfoByEmployee(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, CurrentUser.EmployeeId, (int)ProcessType.Self);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByEmployeeCriteria()
                    {
                        EPAId = epaId,
                        EmployeeId = CurrentUser.EmployeeId
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoByEmployee(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        //[ApiAuthorize(EPAFunctionKey.Self, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateEPAScoreByEmployee(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Self;
                criteria.EvaluatorId = CurrentUser.EmployeeId;
                criteria.ModifiedBy = CurrentUser.EmployeeId;

                commandResult = UpdateEPAScoreByEmployeeOrEvaluator(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnEmployee()
        {
            return CurrentUser.GetPermission(FunctionKey.Self);
        }

        #endregion Self

        #region Evaluator

        //[ApiAuthorize(EPAFunctionKey.Evaluator, Policy.Read)]
        [HttpGet]
        public IEnumerable<Group> GetGroupByEvaluator()
        {
            return GetGroupByEvaluatorOrReviewerOrManagement((int)ProcessType.Evaluator);
        }

        //[ApiAuthorize(EPAFunctionKey.Evaluator, Policy.Read)]
        [HttpGet]
        public IEnumerable<Employee> GetMemberByEvaluator(
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            return GetMemberByEvaluatorOrReviewerOrManagement((int)ProcessType.Evaluator, groupId, isLocal, isMonthly);
        }

        //[ApiAuthorize(EPAFunctionKey.Evaluator, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAModel> GetEPAByEvaluator(
            int? groupId = null,
            bool? isLocal = null,
            bool isMonthly = false,
            string employeeIds = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEPAByEvaluator(criteria);
        }

        //[ApiAuthorize(EPAFunctionKey.Evaluator, Policy.Read)]
        //[ApiAuthorize(EPAFunctionKey.Reviewer, Policy.Read)]
        //[ApiAuthorize(EPAFunctionKey.Management, Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetEPAInfoByEvaluator(int epaId, int? evaluatorId = null)
        {
            var epaInfo = new EPAModel();
            var processId = evaluatorId == null ? (int?)ProcessType.Evaluator : null;
            evaluatorId = evaluatorId ?? CurrentUser.EmployeeId;

            int permission = CheckEmployeePermission(epaId, evaluatorId.Value, processId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByEvaluatorCriteria()
                    {
                        EPAId = epaId,
                        EvaluatorId = evaluatorId.Value
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoByEvaluator(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }
        
        [ApiAuthorize(FunctionKey.Evaluator, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateEPAScoreByEvaluator(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Evaluator;
                criteria.EvaluatorId = CurrentUser.EmployeeId;
                criteria.ModifiedBy = CurrentUser.EmployeeId;

                commandResult = UpdateEPAScoreByEmployeeOrEvaluator(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnEvaluator()
        {
            return CurrentUser.GetPermission(FunctionKey.Evaluator);
        }

        #endregion Evaluator

        #region Reviewer

        [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        [HttpGet]
        public IEnumerable<Group> GetGroupByReviewer()
        {
            return GetGroupByEvaluatorOrReviewerOrManagement((int)ProcessType.Reviewer);
        }

        [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        [HttpGet]
        public IEnumerable<Employee> GetMemberByReviewer(
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            return GetMemberByEvaluatorOrReviewerOrManagement((int)ProcessType.Reviewer, groupId, isLocal, isMonthly);
        }

        [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAModel> GetEPAByReviewer(
            int? groupId = null,
            bool? isLocal = null,
            bool isMonthly = false,
            string employeeIds = null)
        {
            var criteria = new GetEPAByReviewerCriteria()
            {
                ReviewerId = CurrentUser.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEPAByReviewer(criteria);
        }

        [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetEPAInfoByReviewer(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, CurrentUser.EmployeeId, (int)ProcessType.Reviewer);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByReviewerCriteria()
                    {
                        EPAId = epaId,
                        ReviewerId = CurrentUser.EmployeeId
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoByReviewer(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [ApiAuthorize(FunctionKey.Reviewer, Policy.Host)]
        [HttpPut]
        public CommandResult AdjustScoreByReviewer(IEnumerable<EPAModel> memberEPAs)
        {
            var criteria = new AdjustScoreByReviewerOrManagementCriteria()
            {
                ProcessId = (int)ProcessType.Reviewer,
                MemberEPAs = memberEPAs,
                ModifiedBy = CurrentUser.EmployeeId
            };

            return AdjustScoreByReviewerOrManagement(criteria);
        }

        public int GetUserPermissionOnReviewer()
        {
            return CurrentUser.GetPermission(FunctionKey.Reviewer);
        }

        #endregion Reviewer

        #region Management

        [ApiAuthorize(FunctionKey.Management, Policy.Read)]
        [HttpGet]
        public IEnumerable<Group> GetGroupByManagement()
        {
            return GetGroupByEvaluatorOrReviewerOrManagement((int)ProcessType.Management);
        }

        [ApiAuthorize(FunctionKey.Management, Policy.Read)]
        [HttpGet]
        public IEnumerable<Employee> GetMemberByManagement(
            int? groupId = null,
            bool? isLocal = null)
        {
            return GetMemberByEvaluatorOrReviewerOrManagement((int)ProcessType.Management, groupId, isLocal);
        }

        [ApiAuthorize(FunctionKey.Management, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAModel> GetEPAByManagement(
            int? groupId = null,
            bool? isLocal = null,
            string employeeIds = null)
        {
            var criteria = new GetEPAByManagementCriteria()
            {
                ManagementId = CurrentUser.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEPAByManagement(criteria);
        }

        [ApiAuthorize(FunctionKey.Management, Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetEPAInfoByManagement(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, CurrentUser.EmployeeId, (int)ProcessType.Management);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByManagementCriteria()
                    {
                        EPAId = epaId,
                        ManagementId = CurrentUser.EmployeeId
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoByManagement(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [ApiAuthorize(FunctionKey.Management, Policy.Host)]
        [HttpPut]
        public CommandResult AdjustScoreByManagement(IEnumerable<EPAModel> memberEPAs)
        {
            CommandResult commandResult = new CommandResult();

            if (memberEPAs != null && memberEPAs.Any())
            {
                var criteria = new AdjustScoreByReviewerOrManagementCriteria()
                {
                    ProcessId = (int)ProcessType.Management,
                    MemberEPAs = memberEPAs,
                    ModifiedBy = CurrentUser.EmployeeId
                };

                commandResult = AdjustScoreByReviewerOrManagement(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnManagement()
        {
            return CurrentUser.GetPermission(FunctionKey.Management);
        }

        #endregion Management

        #region Rating By KPI

        [ApiAuthorize(FunctionKey.RatingByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAPeriodModel> GetPeriodByEvaluator()
        {
            var criteria = new GetPeriodByEvaluatorCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeId
            };

            return _employeeEPAService.GetPeriodByEvaluator(criteria);
        }

        [ApiAuthorize(FunctionKey.RatingByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<TargetTemplateModel> GetTargetTemplateByEvaluator(int periodId)
        {
            var criteria = new GetTargetTemplateByEvaluatorCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeId,
                PeriodId = periodId
            };

            return _employeeEPAService.GetTargetTemplateByEvaluator(criteria);
        }

        [ApiAuthorize(FunctionKey.RatingByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAPeriodModel> GetPeriodToView()
        {
            var criteria = new GetPeriodToViewCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeId
            };

            return _employeeEPAService.GetPeriodToView(criteria);
        }

        [ApiAuthorize(FunctionKey.EPAByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<TargetTemplateModel> GetTargetTemplateToView(int periodId)
        {
            var criteria = new GetTargetTemplateToViewCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeId,
                PeriodId = periodId
            };

            return _employeeEPAService.GetTargetTemplateToView(criteria);
        }

        [ApiAuthorize(FunctionKey.EPAByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<Employee> GetEvaluatorsByTargetTemplate(
            int periodId,
            int targetTemplateId)
        {
            var criteria = new GetEvaluatorsByTargetTemplateCriteria()
            {
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return _employeeEPAService.GetEvaluatorsByTargetTemplate(criteria);
        }

        [ApiAuthorize(FunctionKey.EPAByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<TemporaryKPIModel> GetKPIToView(
            int periodId,
            int targetTemplateId,
            int evaluatorId)
        {
            var criteria = new GetKPIToViewCriteria()
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return _employeeEPAService.GetKPIToView(criteria);
        }

        [ApiAuthorize(FunctionKey.EPAByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(
            int periodId,
            int targetTemplateId,
            int kpiId,
            int evaluatorId)
        {
            var criteria = new GetEmployeeEPAToViewCriteria()
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId
            };

            return _employeeEPAService.GetEmployeeEPAToView(criteria);
        }

        [ApiAuthorize(FunctionKey.RatingByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(
            int periodId,
            int targetTemplateId)
        {
            var criteria = new GetKPIByTargetTemplateCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeId,
                TargetTemplateId = targetTemplateId,
                PeriodId = periodId
            };

            return _employeeEPAService.GetKPIByTargetTemplate(criteria);
        }

        [ApiAuthorize(FunctionKey.RatingByKPI, Policy.Read)]
        [HttpGet]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(
            int periodId,
            int targetTemplateId,
            int kpiId,
            int? groupId = null,
            string employeeIds = null)
        {
            var criteria = new GetEmployeeEPAByKPICriteria()
            {
                EvaluatorId = CurrentUser.EmployeeId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId,
                GroupId = groupId,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEmployeeEPAByKPI(criteria);
        }

        [ApiAuthorize(FunctionKey.RatingByKPI, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateKPIScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            var criteria = new UpdateKPIScoreCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeId,
                EpaInfos = epaInfos
            };

            return _employeeEPAService.UpdateKPIScore(criteria);
        }

        public int GetUserPermissionOnRatingByKPI()
        {
            return CurrentUser.GetPermission(FunctionKey.RatingByKPI);
        }

        #endregion Rating By KPI

        [HttpGet]
        public HttpResponseMessage GetEPAInfoById(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, CurrentUser.EmployeeId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByIdCriteria()
                    {
                        EPAId = epaId
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoById(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        public CommandResult ConvertEPA()
        {
            var getEPASummaryImportCriteria = new GetEPASummaryImportCriteria();
            IEnumerable<EPASummaryImportModel> memberEPAs = _employeeEPAService.GetMemberEPASummaryImport(getEPASummaryImportCriteria);
            var criteria = new UpdateEPASummaryImportCriteria()
            {
                MemberEPAs = memberEPAs
            };

            CommandResult result = _employeeEPAService.UpdateEPASummaryImport(criteria);
            return result;
        }

        [HttpPut]
        public HttpResponseMessage ResetScore(GetEPAInfoByIdCriteria criteria)
        {
            HttpResponseMessage response = null;

            IEnumerable<SystemRoleModel> roles = this.roleService.GetRolesByEmployeeId(CurrentUser.EmployeeId);
            if (roles.Select(x => x.RoleId).Contains((int)SystemRole.SystemAdmin))
            {
                EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(criteria);

                var resetScoreCriteria = new ResetScoreCriteria()
                {
                    EPAId = epaInfo.EPAId,
                    ModifiedBy = CurrentUser.EmployeeId
                };

                CommandResult commandResult;
                if (epaInfo.HasMonthlyEPA
                    && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    if (epaInfo.Process.Reviewers.Any()
                        && epaInfo.Process.Reviewers.First().StartDate.Date <= DateTime.Now.Date)
                    {
                        resetScoreCriteria.EPAInfo = epaInfo;

                        commandResult = _employeeEPAService.ResetScore(resetScoreCriteria);
                        response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                    }
                }
                else
                {
                    resetScoreCriteria.EPAInfo = epaInfo;

                    commandResult = _employeeEPAService.ResetScore(resetScoreCriteria);
                    response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }

            return response;
        }

        #region Update Final Score

        [ApiAuthorize(FunctionKey.UpdateFinalScore, Policy.Read)]
        public HttpResponseMessage GetPeriodToUpdateFinalScore()
        {
            var criteria = new GetPeriodToUpdateFinalScoreCriteria();
            IList<EPAPeriodModel> periods = _employeeEPAService.GetPeriodToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, periods);
        }

        [ApiAuthorize(FunctionKey.UpdateFinalScore, Policy.Read)]
        public HttpResponseMessage GetEmployeeToUpdateFinalScore(
            int periodId,
            int? groupId = null,
            string query = "")
        {
            var criteria = new GetEmployeeToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                Query = query
            };
            IList<EPAEmployeeModel> employees = _employeeEPAService.GetEmployeeToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, employees);
        }

        [ApiAuthorize(FunctionKey.UpdateFinalScore, Policy.Read)]
        public HttpResponseMessage GetEPAToUpdateFinalScore(
            int periodId,
            int? groupId = null,
            int? employeeId = null)
        {
            var criteria = new GetEPAToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                EmployeeId = employeeId
            };
            IList<EPAModel> epaInfos = _employeeEPAService.GetEPAToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [ApiAuthorize(FunctionKey.UpdateFinalScore, Policy.Host)]
        [HttpPut]
        public HttpResponseMessage UpdateFinalScore(IList<EPAModel> selectedEPAs)
        {
            var getStandardGradeCriteria = new GetStandardGradeCriteria();
            IEnumerable<StandardGradeModel> standardGrades = _employeeEPAService.GetStandardGrade(getStandardGradeCriteria);

            foreach (EPAModel epaInfo in selectedEPAs)
            {
                float averagePerformanceScore = GetFinalScoreOfEPA(epaInfo),
                    finalScore = averagePerformanceScore + epaInfo.DisciplineScore;

                finalScore = finalScore >= 0 ? finalScore : 0;
                epaInfo.FinalScore = EncodeService.Encode(finalScore.ToString());
                string grade = _employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);
                epaInfo.Grade = EncodeService.Encode(grade);
            }

            var updateFinalScoreCriteria = new UpdateFinalScoreEPAEndEvaluationCriteria
            {
                ModifiedBy = CurrentUser.EmployeeId,
                EPAInfos = selectedEPAs
            };

            CommandResult commandResult = _employeeEPAService.UpdateFinalScoreEPAEndEvaluation(updateFinalScoreCriteria);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, commandResult);

            return response;
        }

        #endregion Update Final Score

        private CommandResult UpdateEPAScoreByEmployeeOrEvaluator(UpdateEPAScoreCriteria criteria)
        {
            return _employeeEPAService.UpdateEPAScore(criteria);
        }

        private IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(
            int processId,
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            var criteria = new GetMemberByEvaluatorOrReviewerOrManagementCriteria()
            {
                ActorUserId = CurrentUser.EmployeeId,
                ProcessId = processId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly
            };

            return _employeeEPAService.GetMemberByEvaluatorOrReviewerOrManagement(criteria);
        }

        private IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(int processId)
        {
            var criteria = new GetGroupByEvaluatorOrReviewerOrManagementCriteria()
            {
                ActorUserId = CurrentUser.EmployeeId,
                ProcessId = processId
            };

            return _employeeEPAService.GetGroupByEvaluatorOrReviewerOrManagement(criteria);
        }

        private CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            return _employeeEPAService.AdjustScoreByReviewerOrManagement(criteria);
        }

        private int CheckEmployeePermission(
            int epaId,
            int actorUserId,
            int? processId = null)
        {
            var checkPermissionCriteria = new GetEmployeePermissionCriteria()
            {
                EPAId = epaId,
                ActorUserId = actorUserId,
                ProcessId = processId
            };

            return _employeeEPAService.CheckEmployeePermission(checkPermissionCriteria);
        }

        private float GetFinalScoreOfEPA(EPAModel epaInfo)
        {
            float finalScore = 0;

            if (epaInfo.Process.Managements.Any())
            {
                finalScore = epaInfo.Process.AverageScoreByManagements;
            }
            else if (epaInfo.Process.Reviewers.Any())
            {
                finalScore = epaInfo.Process.AverageScoreByReviewers;
            }

            return finalScore;
        }
    }
}