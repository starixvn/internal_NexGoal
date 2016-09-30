namespace NexGoal.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class EmployeeEPAController : BaseController
    {
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IEPAReviewerService reviewerService;
        private readonly IEPAEvaluatorService evaluatorService;
        private readonly IEPASelfService selfService;
        private readonly IRoleService roleService;
        private readonly ICertificateService certificateService;

        public EmployeeEPAController()
            : this(null, null, null, null, null, null)
        {
        }

        public EmployeeEPAController(
            IEmployeeEPAService employeeEPAService,
            IRoleService roleService,
            IEPAReviewerService reviewerService,
            IEPAEvaluatorService evaluatorService,
            IEPASelfService selfService,
            ICertificateService certificateService)
        {
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.roleService = roleService ?? new RoleService();
            this.reviewerService = reviewerService ?? new EPAReviewerService();
            this.evaluatorService = evaluatorService ?? new EPAEvaluatorService();
            this.selfService = selfService ?? new EPASelfService();
            this.certificateService = certificateService ?? new CertificateService();
        }

        #region Self

        [ApiAuthorize(FunctionKey.Self, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAModel> GetEPAByEmployee(string periodIds = null)
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = SessionService.EmployeeId,
                PeriodIds = periodIds
            };

            return this.selfService.GetEPA(criteria);
        }

        [ApiAuthorize(FunctionKey.Self, Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetEPAInfoByEmployee(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, SessionService.EmployeeId, (int)ProcessType.Self);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByEmployeeCriteria()
                    {
                        EPAId = epaId,
                        EmployeeId = SessionService.EmployeeId
                    };

                    epaInfo = this.selfService.GetEPAInfo(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [ApiAuthorize(FunctionKey.Self, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateEPAScoreByEmployee(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Self;
                criteria.EvaluatorId = SessionService.EmployeeId;
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = UpdateEPAScoreByEmployeeOrEvaluator(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnEmployee()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.Self);
        }

        #endregion Self

        #region Evaluator

        [ApiAuthorize(FunctionKey.Evaluator, Policy.Read)]
        [HttpGet]
        public IEnumerable<Group> GetGroupByEvaluator(string periodIds = null)
        {
            return GetGroupByEvaluatorOrReviewer((int)ProcessType.Evaluator, periodIds);
        }

        [ApiAuthorize(FunctionKey.Evaluator, Policy.Read)]
        [HttpGet]
        public IEnumerable<Employee> GetMemberByEvaluator(
            string groupIds = null,
            string periodIds = null)
        {
            return GetMemberByEvaluatorOrReviewer((int)ProcessType.Evaluator, groupIds, periodIds);
        }

        [ApiAuthorize(FunctionKey.Evaluator, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAModel> GetEPAByEvaluator(
            string periodIds = null,
            string groupIds = null,
            string employeeIds = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria()
            {
                EvaluatorId = SessionService.EmployeeId,
                GroupIds = groupIds,
                EmployeeIds = employeeIds,
                PeriodIds = periodIds
            };

            return this.evaluatorService.GetEPA(criteria);
        }

        /// [ApiAuthorize(FunctionKey.Evaluator, Policy.Read)]
        /// [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        /// [ApiAuthorize(FunctionKey.Management, Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetEPAInfoByEvaluator(int epaId, int? evaluatorId = null)
        {
            var processId = evaluatorId == null ? (int?)ProcessType.Evaluator : null;
            evaluatorId = evaluatorId ?? SessionService.EmployeeId;

            int permission = CheckEmployeePermission(epaId, evaluatorId.Value, processId);
            if (permission == (int)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }

            var criteria = new GetEPAInfoByEvaluatorCriteria()
            {
                EPAId = epaId,
                EvaluatorId = evaluatorId.Value
            };

            EPAModel epaInfo = this.evaluatorService.GetEPAInfo(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
        }

        [ApiAuthorize(FunctionKey.Evaluator, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateEPAScoreByEvaluator(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Evaluator;
                criteria.EvaluatorId = SessionService.EmployeeId;
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = UpdateEPAScoreByEmployeeOrEvaluator(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnEvaluator()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.Evaluator);
        }

        #endregion Evaluator

        #region Reviewer

        [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        [HttpGet]
        public IEnumerable<Group> GetGroupByReviewer(string periodIds = null)
        {
            return GetGroupByEvaluatorOrReviewer((int)ProcessType.Reviewer, periodIds);
        }

        [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        [HttpGet]
        public IEnumerable<Employee> GetMemberByReviewer(
            string groupIds = null,
            string periodIds = null)
        {
            return GetMemberByEvaluatorOrReviewer((int)ProcessType.Reviewer, groupIds, periodIds);
        }

        [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        [HttpGet]
        public IEnumerable<EPAModel> GetEPAByReviewer(
            string periodIds,
            string groupIds = null,
            string employeeIds = null)
        {
            var criteria = new GetEPAByReviewerCriteria()
            {
                ReviewerId = SessionService.EmployeeId,
                GroupIds = groupIds,
                PeriodIds = periodIds,
                EmployeeIds = employeeIds
            };

            return reviewerService.GetEPA(criteria);
        }

        [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        [HttpGet]
        public HttpResponseMessage GetEPAInfoByReviewer(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, SessionService.EmployeeId, (int)ProcessType.Reviewer);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByReviewerCriteria()
                    {
                        EPAId = epaId,
                        ReviewerId = SessionService.EmployeeId
                    };

                    epaInfo = reviewerService.GetEPAInfo(criteria);

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
                ModifiedBy = SessionService.EmployeeId
            };

            return AdjustScoreByReviewerOrManagement(criteria);
        }

        public int GetUserPermissionOnReviewer()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.Reviewer);
        }

        #endregion Reviewer

        [HttpGet]
        public HttpResponseMessage GetEPAInfoById(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, SessionService.EmployeeId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:

                    epaInfo = employeeEPAService.GetEPAInfoById(epaId);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        public CommandResult ConvertEPA()
        {
            var getEPASummaryImportCriteria = new GetEPASummaryImportCriteria();
            IEnumerable<EPASummaryImportModel> memberEPAs = employeeEPAService.GetMemberEPASummaryImport(getEPASummaryImportCriteria);
            var criteria = new UpdateEPASummaryImportCriteria()
            {
                MemberEPAs = memberEPAs
            };

            CommandResult result = employeeEPAService.UpdateEPASummaryImport(criteria);
            return result;
        }

        [HttpPut]
        public HttpResponseMessage ResetScore(int epaId)
        {
            HttpResponseMessage response = null;

            IEnumerable<SystemRoleModel> roles = this.roleService.GetRolesByEmployeeId(SessionService.EmployeeId);
            if (roles.Select(x => x.RoleId).Contains((int)SystemRole.SystemAdmin))
            {
                EPAModel epaInfo = employeeEPAService.GetEPAInfoById(epaId);

                var resetScoreCriteria = new ResetScoreCriteria()
                {
                    EPAId = epaInfo.EPAId,
                    ModifiedBy = SessionService.EmployeeId
                };

                CommandResult commandResult;
                if (epaInfo.HasMonthlyEPA
                    && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    if (epaInfo.Process.Reviewers.Any()
                        && epaInfo.Process.Reviewers.First().StartDate.Date <= DateTime.Now.Date)
                    {
                        resetScoreCriteria.EPAInfo = epaInfo;

                        commandResult = employeeEPAService.ResetScore(resetScoreCriteria);
                        response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                    }
                }
                else
                {
                    resetScoreCriteria.EPAInfo = epaInfo;

                    commandResult = employeeEPAService.ResetScore(resetScoreCriteria);
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
            IList<EPAPeriodModel> periods = employeeEPAService.GetPeriodToUpdateFinalScore(criteria);

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
            IList<EPAEmployeeModel> employees = employeeEPAService.GetEmployeeToUpdateFinalScore(criteria);

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
            IList<EPAModel> epaInfos = employeeEPAService.GetEPAToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [ApiAuthorize(FunctionKey.UpdateFinalScore, Policy.Host)]
        [HttpPut]
        public HttpResponseMessage UpdateFinalScore(IList<EPAModel> selectedEPAs)
        {
            IEnumerable<StandardGradeModel> standardGrades = employeeEPAService.GetStandardGrade();

            foreach (EPAModel epaInfo in selectedEPAs)
            {
                float averagePerformanceScore = GetFinalScoreOfEPA(epaInfo),
                    finalScore = averagePerformanceScore + epaInfo.DisciplineScore;

                finalScore = finalScore >= 0 ? finalScore : 0;
                epaInfo.FinalScore = certificateService.Encrypt(finalScore.ToString());
                string grade = employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);
                epaInfo.Grade = certificateService.Encrypt(grade);
            }

            var updateFinalScoreCriteria = new UpdateFinalScoreEPAEndEvaluationCriteria
            {
                ModifiedBy = SessionService.EmployeeId,
                EPAInfos = selectedEPAs
            };

            CommandResult commandResult = employeeEPAService.UpdateFinalScoreEPAEndEvaluation(updateFinalScoreCriteria);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, commandResult);

            return response;
        }

        #endregion Update Final Score

        public IEnumerable<EPAKPIGroupModel> GetLastRating(int employeeId, int targetTypeId)
        {
            return employeeEPAService.GetLastRating(employeeId, targetTypeId);
        }

        private CommandResult UpdateEPAScoreByEmployeeOrEvaluator(UpdateEPAScoreCriteria criteria)
        {
            return employeeEPAService.UpdateEPAScore(criteria);
        }

        private IEnumerable<Employee> GetMemberByEvaluatorOrReviewer(
            int processId,
            string groupIds = null,
            string periodIds = null)
        {
            var criteria = new GetMemberByEvaluatorOrReviewerCriteria()
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = processId,
                GroupIds = groupIds,
                PeriodIds = periodIds
            };

            return employeeEPAService.GetMemberByEvaluatorOrReviewer(criteria);
        }

        private IEnumerable<Group> GetGroupByEvaluatorOrReviewer(int processId, string periodIds = null)
        {
            var criteria = new GetGroupByEvaluatorOrReviewerCriteria()
            {
                ActorUserId = SessionService.EmployeeId,
                ProcessId = processId,
                PeriodIds = periodIds
            };

            return employeeEPAService.GetGroupByEvaluatorOrReviewerOrManagement(criteria);
        }

        private CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            return employeeEPAService.AdjustScoreByReviewerOrManagement(criteria);
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

            return employeeEPAService.CheckEmployeePermission(checkPermissionCriteria);
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